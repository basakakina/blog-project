using DataAccess.DbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEB.Areas.Admin.Models.ViewModels.Category;
using WEB.Utils;

namespace WEB.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Author")]
    [Route("admin/[controller]/[action]/{id?}")]
    public class CategoriesController : Controller
    {
        private readonly BlogDbContext _db;
        public CategoriesController(BlogDbContext db) => _db = db;

        public async Task<IActionResult> Index(int page = 1, int size = 10)
        {
            var q = _db.Categories.AsNoTracking().OrderBy(c => c.Name);
            var items = await q.Skip((page - 1) * size).Take(size).ToListAsync();
            return View(items);
        }

        public IActionResult Create() => View(new CategoryVM());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryVM vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Slug)) vm.Slug = SlugHelper.ToSlug(vm.Name);

            var exists = await _db.Categories.AnyAsync(c => c.Slug == vm.Slug);
            if (exists) ModelState.AddModelError(nameof(vm.Slug), "Slug zaten mevcut.");

            if (!ModelState.IsValid) return View(vm);

            _db.Categories.Add(new ApplicationCore.Entities.Concrete.Category
            {
                Id = Guid.NewGuid(),
                Name = vm.Name.Trim(),
                Slug = vm.Slug
            });
            await _db.SaveChangesAsync();
            TempData["Success"] = "Kategori oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var c = await _db.Categories.FindAsync(id);
            if (c == null) return NotFound();

            return View(new CategoryVM { Id = c.Id, Name = c.Name, Slug = c.Slug });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryVM vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Slug)) vm.Slug = SlugHelper.ToSlug(vm.Name);

            var exists = await _db.Categories.AnyAsync(c => c.Slug == vm.Slug && c.Id != vm.Id);
            if (exists) ModelState.AddModelError(nameof(vm.Slug), "Slug zaten mevcut.");

            if (!ModelState.IsValid) return View(vm);

            var c = await _db.Categories.FindAsync(vm.Id);
            if (c == null) return NotFound();

            c.Name = vm.Name.Trim();
            c.Slug = vm.Slug;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Kategori güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var c = await _db.Categories.FindAsync(id);
            if (c == null) return NotFound();

            try
            {
                _db.Categories.Remove(c);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Kategori silindi.";
            }
            catch
            {
                TempData["Error"] = "Bu kategori kullanılıyor (Post ilişkisi).";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
