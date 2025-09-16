using DataAccess.DbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace WEB.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly BlogDbContext _db;
        public HomeController(BlogDbContext db) => _db = db;

        // Published postlarý listele
        public async Task<IActionResult> Index()
        {
            var posts = await _db.Posts.AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.CreatedDate)
            .Take(5)
            .ToListAsync();

            return View(posts); // Views/Home/Index.cshtml -> @model IEnumerable<Post>
        }
    }
}
