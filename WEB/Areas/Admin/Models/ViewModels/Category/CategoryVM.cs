using System.ComponentModel.DataAnnotations;

namespace WEB.Areas.Admin.Models.ViewModels.Category
{
    public class CategoryVM
    {
        public Guid Id { get; set; }

        [Required, StringLength(80)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(120)]
        public string Slug { get; set; } = string.Empty;
    }
}
