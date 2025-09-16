using ApplicationCore.Entities.Abstract;
using ApplicationCore.Entities.Enums;
using ApplicationCore.UserEntites.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.Concrete
{
    public class Post : BaseEntity
    {


        public string Title { get; set; } = default!;
      
        public string Content { get; set; } = default!;

        public string? Summary { get; set; }

        public string? ReadTime { get; set; }
        public string? ImageUrl { get; set; }


        public DateTime? PublishDate { get; set; }
        public bool IsPublished { get; set; } = false;

  
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = default!;

     
        public Guid? AuthorId { get; set; }
        public AppUser? Author { get; set; }

    

        public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    }

}


