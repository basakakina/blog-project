using ApplicationCore.Entities.Concrete;
using Business.Manager.Interface;
using DataAccess.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Manager.Concrete
{
    public class BlogPostManager : IBlogPostService
    {
        private readonly IBlogPostRepository _repo;
        public BlogPostManager(IBlogPostRepository repo) => _repo = repo;

        public async Task<IEnumerable<Post>> GetAllAsync() => await _repo.GetByDefaultsAsync(_ => true);
        public async Task<Post> GetByIdAsync(Guid id) => await _repo.GetByIdAsync(id);
        public async Task AddAsync(Post post) => await _repo.AddAsync(post);
        public async Task UpdateAsync(Post post) => await _repo.UpdateAsync(post);
        public async Task DeleteAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity != null) await _repo.DeleteAsync(entity);
        }
        public Task<List<Post>> GetLatestAsync(int count) => _repo.GetLatestAsync(count);
    }
}
