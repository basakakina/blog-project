using DataAccess.DbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WEB.Areas.Admin.Models;
using WEB.Areas.Admin.Models.ViewModels.Tag;
using WEB.Utils;

namespace WEB.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Author")]
    [Route("admin/[controller]/[action]/{id?}")]
    public class TagsController : Controller
    {
        private readonly BlogDbContext _db;
        public TagsController(BlogDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var items = await _db.Tags.AsNoTracking().OrderBy(t => t.Name).ToListAsync();
            return View(items);
        }

        public IActionResult Create() => View(new TagVM());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TagVM vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Slug))
                vm.Slug = SlugHelper.ToSlug(vm.Name);

            var exists = await _db.Tags.AnyAsync(t => t.Slug == vm.Slug);
            if (exists) ModelState.AddModelError(nameof(vm.Slug), "Slug zaten mevcut.");

            if (!ModelState.IsValid) return View(vm);

            _db.Tags.Add(new ApplicationCore.Entities.Concrete.Tag
            {
                Id = Guid.NewGuid(),
                Name = vm.Name.Trim(),
                Slug = vm.Slug
            });
            await _db.SaveChangesAsync();
            TempData["Success"] = "Etiket oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var t = await _db.Tags.FindAsync(id);
            if (t == null) return NotFound();

            return View(new TagVM { Id = t.Id, Name = t.Name, Slug = t.Slug });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TagVM vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Slug))
                vm.Slug = SlugHelper.ToSlug(vm.Name);

            var exists = await _db.Tags.AnyAsync(t => t.Slug == vm.Slug && t.Id != vm.Id);
            if (exists) ModelState.AddModelError(nameof(vm.Slug), "Slug zaten mevcut.");

            if (!ModelState.IsValid) return View(vm);

            var t = await _db.Tags.FindAsync(vm.Id);
            if (t == null) return NotFound();

            t.Name = vm.Name.Trim();
            t.Slug = vm.Slug;
            await _db.SaveChangesAsync();

            TempData["Success"] = "Etiket güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var t = await _db.Tags.FindAsync(id);
            if (t == null) return NotFound();

            try
            {
                _db.Tags.Remove(t);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Etiket silindi.";
            }
            catch
            {
                TempData["Error"] = "Bu etiket kullanılıyor";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
