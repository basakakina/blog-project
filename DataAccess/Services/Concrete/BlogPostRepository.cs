using ApplicationCore.Entities.Concrete;
using DataAccess.DbContext;
using DataAccess.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DataAccess.Services.Concrete
{
    public class BlogPostRepository : BaseRepository<Post>, IBlogPostRepository
    {
        private readonly BlogDbContext _context;
        private readonly IUserService _userService;

   
        public BlogPostRepository(BlogDbContext context, IUserService userService) : base(context)
        {
            _context = context;
            _userService = userService;
        }

        public Task<List<Post>> GetLatestAsync(int count) =>
            _context.Set<Post>()                     
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.PublishDate ?? p.CreatedDate)
                    .Take(count)
                    .ToListAsync();
    }
}
