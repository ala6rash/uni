using Microsoft.AspNetCore.Mvc;
using Uni_Connect.Models;
using Microsoft.EntityFrameworkCore;

namespace Uni_Connect.Controllers
{
    public class DatabaseViewerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DatabaseViewerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DatabaseViewerViewModel
            {
                Users = await _context.Users.ToListAsync(),
                Posts = await _context.Posts.ToListAsync(),
                Categories = await _context.Categories.ToListAsync(),
                Tags = await _context.Tags.ToListAsync(),
                UserCount = await _context.Users.CountAsync(),
                PostCount = await _context.Posts.CountAsync(),
                CategoryCount = await _context.Categories.CountAsync(),
                TagCount = await _context.Tags.CountAsync(),
            };

            return View(viewModel);
        }
    }

    public class DatabaseViewerViewModel
    {
        public List<User> Users { get; set; } = new();
        public List<Post> Posts { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<Tag> Tags { get; set; } = new();
        
        public int UserCount { get; set; }
        public int PostCount { get; set; }
        public int CategoryCount { get; set; }
        public int TagCount { get; set; }
    }
}
