using DTO.Concrete.CategoryDTO;
using DTO.Concrete.PostDTO;

namespace WEB.Models.ViewModels.Account
{
    public class HomeVM
    {
        public IEnumerable<CategoryListDTO> Categories { get; set; } = Enumerable.Empty<CategoryListDTO>();
        public IEnumerable<PostListDTO> PublishedPosts { get; set; } = Enumerable.Empty<PostListDTO>();
    }
}
