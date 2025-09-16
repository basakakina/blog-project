using DTO.Concrete.AccountDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Business.Manager.Interface
{
    public interface IUserManager
    {
    
        Task<(bool ok, IEnumerable<string> errors)> RegisterAsync(RegisterDTO dto, bool signIn = true);
        Task<(bool ok, string? error)> LoginAsync(LoginDTO dto);
        Task LogoutAsync();

        Task<T?> FindUserAsync<T>(ClaimsPrincipal principal) where T : class;
        Task<bool> IsUserInRoleAsync(string userNameOrEmail, string role);
        Task<string?> GetUserIdAsync(ClaimsPrincipal principal);

     

    }
}
