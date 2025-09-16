using ApplicationCore.Entities.Abstract;
using ApplicationCore.UserEntites.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.Concrete
{
    public class Comment 
    {

        public Guid Id { get; set; }
        public string Content { get; set; } = "";
        public DateTime CreatedDate { get; set; }

        public Guid PostId { get; set; }
        public Post Post { get; set; } = null!;

        public Guid AuthorId { get; set; }
        public AppUser Author { get; set; } = null!;


    }

}
