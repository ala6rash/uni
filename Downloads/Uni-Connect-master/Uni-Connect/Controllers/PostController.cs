using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Uni_Connect.Models;
using Uni_Connect.ViewModels;

namespace Uni_Connect.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("UserID")?.Value;
            return int.TryParse(idClaim, out int id) ? id : 0;
        }

        [HttpGet]
        public async Task<IActionResult> SinglePost(int id)
        {
            var currentUserId = GetCurrentUserId();
            var currentUser = await _context.Users.FindAsync(currentUserId);

            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .Include(p => p.Answers)
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(p => p.PostID == id);

            if (post == null) return NotFound();

            post.ViewsCount++;
            await _context.SaveChangesAsync();

            var viewModel = new SinglePostViewModel
            {
                CurrentUser = currentUser,
                Post = post,
                Answers = post.Answers.Select(a => new AnswerViewModel
                {
                    AnswerID = a.AnswerID,
                    Content = a.Content,
                    IsAccepted = a.IsAccepted,
                    Upvotes = a.Upvotes,
                    CreatedAt = a.CreatedAt,
                    User = a.User
                }).OrderByDescending(a => a.IsAccepted).ThenByDescending(a => a.Upvotes).ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = GetCurrentUserId();
            var user = await _context.Users.FindAsync(userId);
            var categories = await _context.Categories.ToListAsync();

            ViewBag.CurrentUser = user;
            ViewBag.Categories = categories;
            ViewBag.UnreadNotificationsCount = await _context.Notifications.CountAsync(n => n.UserID == userId && !n.IsRead);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string title, string content, int categoryId)
        {
            var userId = GetCurrentUserId();
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return RedirectToAction("Login", "Login");

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
            {
                ViewBag.Error = "Title and content are required.";
                ViewBag.CurrentUser = user;
                ViewBag.Categories = await _context.Categories.ToListAsync();
                return View();
            }

            // Deduct 10 points for asking a question
            user.Points = Math.Max(0, user.Points - 10);
            _context.Users.Update(user);

            var post = new Post
            {
                Title = title,
                Content = content,
                CategoryID = categoryId,
                UserID = userId,
                CreatedAt = DateTime.Now,
                ViewsCount = 0,
                Upvotes = 0
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("SinglePost", new { id = post.PostID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostAnswer(SinglePostViewModel model, int postId)
        {
            if (string.IsNullOrWhiteSpace(model.NewAnswerContent))
                return RedirectToAction("SinglePost", new { id = postId });

            var currentUserId = GetCurrentUserId();
            var currentUser = await _context.Users.FindAsync(currentUserId);

            var answer = new Answer
            {
                PostID = postId,
                UserID = currentUserId,
                Content = model.NewAnswerContent,
                CreatedAt = DateTime.Now,
                IsAccepted = false,
                Upvotes = 0
            };

            _context.Answers.Add(answer);

            if (currentUser != null)
            {
                currentUser.Points += 5;
                _context.Users.Update(currentUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("SinglePost", new { id = postId });
        }

        [HttpPost]
        public async Task<IActionResult> UpvoteAnswer([FromBody] int answerId)
        {
            var answer = await _context.Answers.Include(a => a.User).FirstOrDefaultAsync(a => a.AnswerID == answerId);
            if (answer == null)
                return Json(new { success = false, message = "Answer not found" });

            answer.Upvotes++;
            if (answer.User != null)
                answer.User.Points += 10;

            await _context.SaveChangesAsync();
            return Json(new { success = true, newUpvotes = answer.Upvotes });
        }
    }
}
