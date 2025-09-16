using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.Concrete
{
    public class PostTag
    {
        public Guid PostId { get; set; }
        public Post Post { get; set; } = default!;
        public Guid TagId { get; set; }
        public Tag Tag { get; set; } = default!;

    }
}
