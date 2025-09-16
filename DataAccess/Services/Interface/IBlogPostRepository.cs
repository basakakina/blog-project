using ApplicationCore.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services.Interface
{
    public interface IBlogPostRepository : IBaseRepository<Post>
    {
        Task<List<Post>> GetLatestAsync(int count);
    }
}
