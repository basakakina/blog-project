using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Concrete.AccountDTO
{
    public class LoginDTO
    {
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
