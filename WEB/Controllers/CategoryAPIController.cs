using DataAccess.DbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WEB.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryAPIController : ControllerBase
    {
        private readonly BlogDbContext _db;
        public CategoryAPIController(BlogDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Categories
                .OrderBy(x => x.Name)
                .Select(x => new { x.Id, x.Name, x.Slug })
                .ToListAsync();
            return Ok(list);
        }

    }
}
