using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Concrete.PostDTO
{
    public class PostCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? PublishDate { get; set; }
        public string? ReadTime { get; set; }
        public string? ImageUrl { get; set; }
    }
}
