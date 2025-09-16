using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Concrete.AccountDTO
{
    public class GetUserDTO
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public bool HasPasswordChanged { get; set; } 
        public string FullName { get; set; } = "";
    }
}
