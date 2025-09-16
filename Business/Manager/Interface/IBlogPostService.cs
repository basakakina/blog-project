using ApplicationCore.Entities.Concrete;


namespace Business.Manager.Interface
{
    public interface IBlogPostService
    {
        Task<IEnumerable<Post>> GetAllAsync();
        Task<Post> GetByIdAsync(Guid id);
        Task AddAsync(Post post);
        Task UpdateAsync(Post post);
        Task DeleteAsync(Guid id);
        Task<List<Post>> GetLatestAsync(int count);
    }
}
