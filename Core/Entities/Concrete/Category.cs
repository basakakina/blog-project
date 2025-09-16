using ApplicationCore.Entities.Abstract;
using ApplicationCore.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.Concrete
{
    public class Category 
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Slug { get; set; }
        public ICollection<Post> Posts { get; set; } = new List<Post>();


    }
}
