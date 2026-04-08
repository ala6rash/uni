using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Uni_Connect.Models;

namespace Uni_Connect.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<int> GetCurrentUserIdAsync()
        {
            var str = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(str, out int id) ? id : 0;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == 0) return RedirectToAction("Login", "Login");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserID == userId);
            if (user == null) return RedirectToAction("Login", "Login");

            // --- AUTO SEEDING: adds sample data on first login ---
            if (!await _context.Posts.AnyAsync())
            {
                var cat1 = new Category { Name = "CS302 – Data Structures", Faculty = "IT Faculty" };
                var cat2 = new Category { Name = "MATH201 – Calculus", Faculty = "Engineering Faculty" };
                _context.Categories.AddRange(cat1, cat2);
                await _context.SaveChangesAsync();

                _context.Posts.AddRange(
                    new Post { Title = "How does AVL tree rotation actually work?", Content = "I understand BST insertion but I'm confused about when and how AVL performs rotations. Specifically the 4 cases: LL, LR, RR, RL. Can someone explain with a concrete example?", CategoryID = cat1.CategoryID, UserID = userId, CreatedAt = DateTime.Now.AddHours(-2), ViewsCount = 124, Upvotes = 4 },
                    new Post { Title = "What is the difference between an Interface and Abstract Class in C#?", Content = "Can someone explain this clearly? I have an exam on OOP design patterns next week and I keep confusing these two.", CategoryID = cat1.CategoryID, UserID = userId, CreatedAt = DateTime.Now.AddDays(-1), ViewsCount = 89, Upvotes = 12 },
                    new Post { Title = "How do I solve a double integral using polar coordinates?", Content = "I'm stuck on converting Cartesian coordinates to polar and then applying the Jacobian. Any step-by-step guidance would help!", CategoryID = cat2.CategoryID, UserID = userId, CreatedAt = DateTime.Now.AddDays(-2), ViewsCount = 47, Upvotes = 6 },
                    new Post { Title = "What is Big-O notation and why does it matter?", Content = "My professor keeps mentioning time complexity but I still don't understand how to calculate the Big-O of an algorithm. Any beginner-friendly explanation?", CategoryID = cat1.CategoryID, UserID = userId, CreatedAt = DateTime.Now.AddHours(-5), ViewsCount = 211, Upvotes = 18 }
                );
                await _context.SaveChangesAsync();
            }
            // -------------------------------------------------------

            var recentPosts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .Include(p => p.Answers)
                .OrderByDescending(p => p.CreatedAt)
                .Take(20)
                .ToListAsync();

            var topContributors = await _context.Users
                .OrderByDescending(u => u.Points)
                .Take(5)
                .ToListAsync();

            int notificationsCount = await _context.Notifications
                .CountAsync(n => n.UserID == userId && !n.IsRead);

            var viewModel = new DashboardViewModel
            {
                CurrentUser = user,
                RecentPosts = recentPosts,
                TopContributors = topContributors,
                UnreadNotificationsCount = notificationsCount
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Profile()
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == 0) return RedirectToAction("Login", "Login");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserID == userId);
            if (user == null) return RedirectToAction("Login", "Login");

            var myPosts = await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.Answers)
                .Where(p => p.UserID == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var myAnswers = await _context.Answers
                .Include(a => a.Post)
                .Where(a => a.UserID == userId)
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .ToListAsync();

            ViewBag.CurrentUser = user;
            ViewBag.MyPosts = myPosts;
            ViewBag.MyAnswers = myAnswers;
            ViewBag.UnreadNotificationsCount = await _context.Notifications.CountAsync(n => n.UserID == userId && !n.IsRead);

            return View();
        }

        public async Task<IActionResult> Leaderboard()
        {
            var userId = await GetCurrentUserIdAsync();
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserID == userId);

            var allUsers = await _context.Users
                .OrderByDescending(u => u.Points)
                .ToListAsync();

            ViewBag.CurrentUser = currentUser;
            ViewBag.AllUsers = allUsers;
            ViewBag.UnreadNotificationsCount = currentUser == null ? 0 :
                await _context.Notifications.CountAsync(n => n.UserID == userId && !n.IsRead);

            return View();
        }

        public IActionResult Notifications()
        {
            return View();
        }
    }
}
