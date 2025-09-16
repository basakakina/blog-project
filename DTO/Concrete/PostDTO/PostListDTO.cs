using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Concrete.PostDTO
{
    public class PostListDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string? ImageUrl { get; set; }
        public string? ReadTime { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? PublishDate { get; set; }
        public string Category { get; set; } = string.Empty;  
        public Guid CategoryId { get; set; }
        public string? Author { get; set; }
    }
}
