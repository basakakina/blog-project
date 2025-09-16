using ApplicationCore.Entities.Concrete;
using ApplicationCore.UserEntites.Concrete;
using DataAccess.DbContext;
using DTO.Concrete.PostDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEB.Utils;

namespace WEB.Controllers
{
    public class PostAPIController : Controller
    {

        [ApiController]
        [Route("api/posts")]           
        public class PostApiController : ControllerBase
        {
            private readonly BlogDbContext _db;
            private readonly UserManager<AppUser> _users;

            public PostApiController(BlogDbContext db, UserManager<AppUser> users)
            {
                _db = db;
                _users = users;
            }

            [HttpGet]
            [AllowAnonymous]
            public async Task<ActionResult<IEnumerable<PostListDTO>>> GetAll(
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 10,
                [FromQuery] string? search = null,
                [FromQuery] Guid? categoryId = null)
            {
                if (page <= 0) page = 1;
                if (pageSize <= 0 || pageSize > 100) pageSize = 10;

                IQueryable<Post> q = _db.Posts
                    .Include(p => p.Category)
                    .Include(p => p.Author);

                if (!string.IsNullOrWhiteSpace(search))
                    q = q.Where(p => p.Title.Contains(search) || (p.Summary ?? "").Contains(search));

                if (categoryId.HasValue)
                    q = q.Where(p => p.CategoryId == categoryId.Value);

                q = q.OrderByDescending(p => p.PublishDate ?? p.CreatedDate);

                var list = await q.Skip((page - 1) * pageSize)
                                  .Take(pageSize)
                                  .Select(p => new PostListDTO
                                  {
                                      Id = p.Id,
                                      Title = p.Title,
                                      Slug = p.Slug,
                                      Summary = p.Summary,
                                      ImageUrl = p.ImageUrl,
                                      ReadTime = p.ReadTime,
                                      IsPublished = p.IsPublished,
                                      CreatedDate = p.CreatedDate,
                                      PublishDate = p.PublishDate,
                                      Category = p.Category.Name,
                                      CategoryId = p.CategoryId,
                                      Author = p.Author != null
                                          ? (string.IsNullOrWhiteSpace(p.Author.FullName) ? p.Author.UserName! : p.Author.FullName)
                                          : null
                                  })
                                  .ToListAsync();

                return Ok(list);
            }


            [HttpGet("{id:guid}")]
            [AllowAnonymous]
            public async Task<ActionResult<PostDetailDTO>> GetById(Guid id)
            {
                var p = await _db.Posts
                    .Include(x => x.Category)
                    .Include(x => x.Author)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (p is null) return NotFound();

                var dto = new PostDetailDTO
                {
                    Id = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
                    Summary = p.Summary,
                    Content = p.Content,
                    ImageUrl = p.ImageUrl,
                    ReadTime = p.ReadTime,
                    IsPublished = p.IsPublished,
                    CreatedDate = p.CreatedDate,
                    PublishDate = p.PublishDate,
                    Category = p.Category.Name,
                    CategoryId = p.CategoryId,
                    Author = p.Author != null
                        ? (string.IsNullOrWhiteSpace(p.Author.FullName) ? p.Author.UserName! : p.Author.FullName)
                        : null,
                    AuthorId = p.AuthorId
                };

                return Ok(dto);
            }

            [HttpGet("slug/{slug}")]
            [AllowAnonymous]
            public async Task<ActionResult<PostDetailDTO>> GetBySlug(string slug)
            {
                var p = await _db.Posts
                    .Include(x => x.Category)
                    .Include(x => x.Author)
                    .FirstOrDefaultAsync(x => x.Slug == slug);

                if (p is null) return NotFound();

                var dto = new PostDetailDTO
                {
                    Id = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
                    Summary = p.Summary,
                    Content = p.Content,
                    ImageUrl = p.ImageUrl,
                    ReadTime = p.ReadTime,
                    IsPublished = p.IsPublished,
                    CreatedDate = p.CreatedDate,
                    PublishDate = p.PublishDate,
                    Category = p.Category.Name,
                    CategoryId = p.CategoryId,
                    Author = p.Author != null
                        ? (string.IsNullOrWhiteSpace(p.Author.FullName) ? p.Author.UserName! : p.Author.FullName)
                        : null,
                    AuthorId = p.AuthorId
                };

                return Ok(dto);
            }

          
            [HttpPost]
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> Create([FromBody] PostCreateDto dto)
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var user = await _users.GetUserAsync(User);
                if (user is null) return Unauthorized();

          
                var baseSlug = SlugHelper.ToSlug(dto.Title);
                var slug = baseSlug;
                int i = 2;
                while (await _db.Posts.AnyAsync(x => x.Slug == slug))
                    slug = $"{baseSlug}-{i++}";

                var post = new Post
                {
                    Title = dto.Title,
                    Summary = dto.Summary,
                    Content = dto.Content,
                    CategoryId = dto.CategoryId,
                    IsPublished = dto.IsPublished,
                    PublishDate = dto.PublishDate,
                    ReadTime = dto.ReadTime,
                    ImageUrl = dto.ImageUrl,
                    AuthorId = user.Id,
                    Slug = slug
                };

                _db.Posts.Add(post);
                await _db.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = post.Id }, new { post.Id, post.Slug });
            }

            [HttpPut("{id:guid}")]
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> Update(Guid id, [FromBody] PostUpdateDto dto)
            {
                if (id != dto.Id) return BadRequest("Mismatched id.");
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var post = await _db.Posts.FirstOrDefaultAsync(x => x.Id == id);
                if (post is null) return NotFound();

                post.Title = dto.Title;
                post.Summary = dto.Summary;
                post.Content = dto.Content;
                post.CategoryId = dto.CategoryId;
                post.IsPublished = dto.IsPublished;
                post.PublishDate = dto.PublishDate;
                post.ReadTime = dto.ReadTime;
                post.ImageUrl = dto.ImageUrl;

               
                var newBase = SlugHelper.ToSlug(dto.Title);
                if (!string.Equals(post.Slug, newBase, StringComparison.OrdinalIgnoreCase))
                {
                    var newSlug = newBase;
                    int i = 2;
                    while (await _db.Posts.AnyAsync(x => x.Id != post.Id && x.Slug == newSlug))
                        newSlug = $"{newBase}-{i++}";
                    post.Slug = newSlug;
                }

                await _db.SaveChangesAsync();
                return NoContent();
            }

           
            [HttpDelete("{id:guid}")]
            [Authorize(Roles ="Admin")]
            public async Task<IActionResult> Delete(Guid id)
            {
                var post = await _db.Posts.FindAsync(id);
                if (post is null) return NotFound();

                _db.Posts.Remove(post);
                await _db.SaveChangesAsync();
                return NoContent();
            }
        }
    }
}
