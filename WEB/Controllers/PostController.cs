using System;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.DbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.UserEntites.Concrete;
using ApplicationCore.Entities.Concrete;

namespace WEB.Controllers
{
   
    [AllowAnonymous]
    public class PostController : Controller
    {
        private readonly BlogDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public PostController(BlogDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet("/posts")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _db.Posts
                .AsNoTracking()
                .Where(p => p.IsPublished)
                .Include(p => p.Category)
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .OrderByDescending(p => p.CreatedDate);

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Total = total;

            return View(items); 
        }


        [HttpGet("/posts/{slug}")]
        public async Task<IActionResult> Detail(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return NotFound();

            var post = await _db.Posts
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.Comments).ThenInclude(c => c.Author) 
                .FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished);

            if (post == null) return NotFound();

            return View(post); 
        }

        [HttpGet("/category/{slug}")]
        public async Task<IActionResult> ByCategory(string slug, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(slug)) return NotFound();
            if (page < 1) page = 1; if (pageSize < 1) pageSize = 10;

            var query = _db.Posts
                .AsNoTracking()
                .Where(p => p.IsPublished && p.Category.Slug == slug)
                .Include(p => p.Category)
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .OrderByDescending(p => p.CreatedDate);

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.Page = page; ViewBag.PageSize = pageSize; ViewBag.Total = total;
            ViewBag.CategorySlug = slug;

            return View("Index", items);
        }

        
        [HttpGet("/tag/{slug}")]
        public async Task<IActionResult> ByTag(string slug, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(slug)) return NotFound();
            if (page < 1) page = 1; if (pageSize < 1) pageSize = 10;


            var tagExists = await _db.Tags.AsNoTracking().AnyAsync(t => t.Slug == slug);
            if (!tagExists) return NotFound();

            var query = _db.Posts
                .AsNoTracking()
                .Where(p => p.IsPublished && p.PostTags.Any(pt => pt.Tag.Slug == slug))
                .Include(p => p.Category)
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .OrderByDescending(p => p.CreatedDate);

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.Page = page; ViewBag.PageSize = pageSize; ViewBag.Total = total;
            ViewBag.TagSlug = slug;

            return View("Index", items);
        }

        
        [HttpGet("/search")]
        public async Task<IActionResult> Search(string q, int page = 1, int pageSize = 10)
        {
            q = (q ?? string.Empty).Trim();
            if (page < 1) page = 1; if (pageSize < 1) pageSize = 10;

            var query = _db.Posts
                .AsNoTracking()
                .Where(p => p.IsPublished &&
                            (q == string.Empty
                             || EF.Functions.Like(p.Title, $"%{q}%")
                             || EF.Functions.Like(p.Content, $"%{q}%")))
                .Include(p => p.Category)
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .OrderByDescending(p => p.CreatedDate);

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.Page = page; ViewBag.PageSize = pageSize; ViewBag.Total = total;
            ViewBag.Query = q;

            return View("Index", items);
        }

        
        [Authorize]
        [HttpPost("/posts/{slug}/comments")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment(string slug, [FromForm] string content, string? returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(slug)) return NotFound();
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Yorum boş olamaz.";
                return RedirectToAction(nameof(Detail), new { slug });
            }

            var post = await _db.Posts.FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished);
            if (post == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var comment = new Comment
            {
                PostId = post.Id,
                AuthorId = user.Id,        
                Content = content,
                CreatedDate = DateTime.UtcNow,
             
            };

            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();

            if (!string.IsNullOrEmpty(returnUrl)) return LocalRedirect(returnUrl);
            return RedirectToAction(nameof(Detail), new { slug });
        }

        
        [Authorize]
        [HttpPost("/comments/{id}/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(Guid id, string? returnUrl = null)
        {
            var comment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == id);
            if (comment == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var isOwner = comment.AuthorId == user.Id;
            var isModerator = User.IsInRole("Admin") || User.IsInRole("Author");
            if (!isOwner && !isModerator) return Forbid();

            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();

            if (!string.IsNullOrEmpty(returnUrl)) return LocalRedirect(returnUrl);

            var postSlug = await _db.Posts
                .Where(p => p.Id == comment.PostId)
                .Select(p => p.Slug)
                .FirstAsync();

            return RedirectToAction(nameof(Detail), new { slug = postSlug });
        }
    }
}
