using ApplicationCore.Entities.Concrete;
using DataAccess.DbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WEB.Areas.Admin.Models;
using WEB.Utils;

namespace WEB.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Author")]
    [Route("admin/[controller]/[action]/{id?}")]
    public class PostsController : Controller
    {
        private readonly BlogDbContext _db;
        public PostsController(BlogDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var posts = await _db.Posts
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            return View(posts);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new PostVM
            {
                Categories = await _db.Categories.AsNoTracking()
                    .OrderBy(c => c.Name)
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToListAsync(),
                Tags = await _db.Tags.AsNoTracking()
                    .OrderBy(t => t.Name)
                    .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name })
                    .ToListAsync()
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(PostVM vm)
        {
            await FillSelectLists(vm);

            if (string.IsNullOrWhiteSpace(vm.Slug))
                vm.Slug = SlugHelper.ToSlug(vm.Title);

            var slugExists = await _db.Posts.AnyAsync(p => p.Slug == vm.Slug);
            if (slugExists) ModelState.AddModelError(nameof(vm.Slug), "Slug zaten mevcut.");

            if (!ModelState.IsValid) return View(vm);

            var post = new Post
            {

                Id = Guid.NewGuid(),
                Title = vm.Title.Trim(),
                Slug = vm.Slug,
                Content = vm.Content,
                CategoryId = vm.CategoryId!.Value, 
                IsPublished = vm.IsPublished,
                CreatedDate = DateTime.UtcNow
            };

            if (vm.SelectedTagIds?.Count > 0)
            {
                post.PostTags = vm.SelectedTagIds
                    .Distinct()
                    .Select(tagId => new PostTag
                    {
                        PostId = post.Id,
                        TagId = tagId
                    }).ToList();
            }

            _db.Posts.Add(post);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Post oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(Guid id)
        {
            var post = await _db.Posts
                .Include(p => p.PostTags)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return NotFound();

            var vm = new PostVM
            {
                Id = post.Id,
                Title = post.Title,
                Slug = post.Slug,
                Content = post.Content,
                CategoryId = post.CategoryId,
                IsPublished = post.IsPublished,
                SelectedTagIds = post.PostTags.Select(pt => pt.TagId).ToList()
            };
            await FillSelectLists(vm);
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PostVM vm)
        {
            await FillSelectLists(vm);

            if (string.IsNullOrWhiteSpace(vm.Slug))
                vm.Slug = SlugHelper.ToSlug(vm.Title);

            var slugExists = await _db.Posts
                .AnyAsync(p => p.Slug == vm.Slug && p.Id != vm.Id);
            if (slugExists) ModelState.AddModelError(nameof(vm.Slug), "Slug zaten mevcut.");

            if (!ModelState.IsValid) return View(vm);

            var post = await _db.Posts
                .Include(p => p.PostTags)
                .FirstOrDefaultAsync(p => p.Id == vm.Id);
            if (post == null) return NotFound();

            post.Title = vm.Title.Trim();
            post.Slug = vm.Slug;
            post.Content = vm.Content;
            post.CategoryId = vm.CategoryId!.Value;
            post.IsPublished = vm.IsPublished;

            _db.PostTags.RemoveRange(post.PostTags);
            if (vm.SelectedTagIds?.Count > 0)
            {
                post.PostTags = vm.SelectedTagIds
                    .Distinct()
                    .Select(tagId => new ApplicationCore.Entities.Concrete.PostTag
                    {
                        PostId = post.Id,
                        TagId = tagId
                    }).ToList();
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = "Post güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var post = await _db.Posts.FindAsync(id);
            if (post == null) return NotFound();

            try
            {
                _db.Posts.Remove(post);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Post silindi.";
            }
            catch
            {
                TempData["Error"] = "Post silinemedi.";
            }
            return RedirectToAction(nameof(Index));
        }

 
        private async Task FillSelectLists(PostVM vm)
        {
            vm.Categories = await _db.Categories.AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToListAsync();

            vm.Tags = await _db.Tags.AsNoTracking()
                .OrderBy(t => t.Name)
                .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name })
                .ToListAsync();
        }
    }
}
