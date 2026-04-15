using Microsoft.AspNetCore.Mvc;
using Uni_Connect.Models;
using Uni_Connect.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Uni_Connect.Controllers
{

    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Dashboard()
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login_Page", "Login");

            int userId = int.Parse(userIdStr);
            var user = await _context.Users
                .Include(u => u.Notifications)
                .Include(u => u.Posts)
                .FirstOrDefaultAsync(u => u.UserID == userId);

            if (user == null) return RedirectToAction("Login_Page", "Login");

            // Fetch all posts with related data
            var posts = await _context.Posts
                .Where(p => !p.IsDeleted)
                .Include(p => p.User)
                .Include(p => p.Category)
                .Include(p => p.Answers)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            ViewBag.Posts = posts;
            return View(user);
        }
        public async Task<IActionResult> Profile()
        {
            var user = await GetCurrentUser();
            if (user == null) return RedirectToAction("Login_Page", "Login");
            
            // Ensure navigation properties are loaded
            await _context.Entry(user)
                .Collection(u => u.Posts)
                .LoadAsync();
            await _context.Entry(user)
                .Collection(u => u.Answers)
                .LoadAsync();
            
            return View(user);
        }
        public async Task<IActionResult> Notifications()
        {
            var user = await GetCurrentUser();
            if (user == null) return RedirectToAction("Login_Page", "Login");
            return View(user);
        }
        public async Task<IActionResult> Leaderboard()
        {
            var user = await GetCurrentUser();
            if (user == null) return RedirectToAction("Login_Page", "Login");
            
            // Fetch all users ranked by points
            var leaderboardUsers = await _context.Users
                .Where(u => !u.IsDeleted)
                .OrderByDescending(u => u.Points)
                .Take(100)
                .ToListAsync();

            ViewBag.Leaderboard = leaderboardUsers;
            return View(user);
        }
        public async Task<IActionResult> CreatePost()
        {
            var user = await GetCurrentUser();
            if (user == null) return RedirectToAction("Login_Page", "Login");
            return View(new ViewModels.CreatePostViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(ViewModels.CreatePostViewModel model)
        {
            // Validate model
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                    }
                }
                return View(model);
            }

            // Get current user
            var user = await GetCurrentUser();
            if (user == null) return RedirectToAction("Login_Page", "Login");

            // Check if user has enough points (posting costs 10 points)
            if (user.Points < 10)
            {
                ModelState.AddModelError("", "You need at least 10 points to post a question. Earn more points by answering questions or requesting help.");
                return View(model);
            }

            // Map faculty string to category
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Faculty == model.Faculty);
            
            if (category == null)
            {
                // If category doesn't exist, create it
                category = new Category { Faculty = model.Faculty, Name = model.Faculty };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }

            // Create new Post
            var post = new Post
            {
                Title = model.Title,
                Content = model.Content,
                UserID = user.UserID,
                CategoryID = category.CategoryID,
                CreatedAt = DateTime.UtcNow,
                ViewsCount = 0,
                Upvotes = 0,
                IsDeleted = false
            };

            // Add post to database first to get PostID
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            // Process tags if provided
            if (!string.IsNullOrWhiteSpace(model.Tags))
            {
                // Split tags by comma or space
                var tagNames = model.Tags
                    .Split(new[] { ',', ' ' }, System.StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .Distinct()
                    .Take(5); // Max 5 tags

                foreach (var tagName in tagNames)
                {
                    // Find or create tag
                    var tag = await _context.Tags
                        .FirstOrDefaultAsync(t => t.Name.ToLower() == tagName.ToLower());
                    
                    if (tag == null)
                    {
                        tag = new Tag { Name = tagName };
                        _context.Tags.Add(tag);
                        await _context.SaveChangesAsync();
                    }

                    // Create PostTag relationship
                    var postTag = new PostTag
                    {
                        PostID = post.PostID,
                        TagID = tag.TagID
                    };
                    _context.PostTags.Add(postTag);
                }
            }

            // Deduct 10 points from user
            user.Points -= 10;
            _context.Users.Update(user);

            // Save all changes
            await _context.SaveChangesAsync();

            // Redirect to SinglePost page
            return RedirectToAction("SinglePost", new { id = post.PostID });
        }

        public async Task<IActionResult> Sessions()
        {
            var user = await GetCurrentUser();
            if (user == null) return RedirectToAction("Login_Page", "Login");
            return View(user);
        }

        public async Task<IActionResult> Points()
        {
            var user = await GetCurrentUser();
            if (user == null) return RedirectToAction("Login_Page", "Login");
            return View(user);
        }

        public async Task<IActionResult> ChatPage()
        {
            var user = await GetCurrentUser();
            if (user == null) return RedirectToAction("Login_Page", "Login");
            return View(user);
        }

        public async Task<IActionResult> SinglePost()
        {
            var user = await GetCurrentUser();
            if (user == null) return RedirectToAction("Login_Page", "Login");
            return View(user);
        }

        private async Task<User> GetCurrentUser()
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return null;

            int userId = int.Parse(userIdStr);
            return await _context.Users
                .Include(u => u.Notifications)
                .FirstOrDefaultAsync(u => u.UserID == userId);
        }

        [HttpGet("/api/messages/{roomId}")]
        public async Task<IActionResult> GetMessages(int roomId)
        {
            var messages = await _context.Messages
                .Where(m => m.SessionID == roomId)
                .OrderBy(m => m.SentAt)
                .Select(m => new
                {
                    m.SenderID,
                    m.MessageText,
                    Time = m.SentAt.ToString("HH:mm")
                })
                 .ToListAsync();
            return Ok(messages);


        }
    }
}
