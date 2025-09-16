using ApplicationCore.UserEntites.Concrete;
using Business.Manager.Interface;
using DTO.Concrete.AccountDTO;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Business.Manager.Concrete
{
    public class UserManagerFacade : IUserManager
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UserManagerFacade(UserManager<AppUser> userManager,
                                 SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<(bool ok, IEnumerable<string> errors)> RegisterAsync(RegisterDTO dto, bool signIn = true)
        {
            var user = new AppUser { UserName = dto.UserName, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description));

            if (signIn)
                await _signInManager.SignInAsync(user, isPersistent: true);

            return (true, Array.Empty<string>());
        }

        public async Task<(bool ok, string? error)> LoginAsync(LoginDTO dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);
            if (user is null) return (false, "Invalid credentials.");

            var signIn = await _signInManager.PasswordSignInAsync(
                user, dto.Password, dto.RememberMe, lockoutOnFailure: false);

            return signIn.Succeeded ? (true, null) : (false, "Invalid credentials.");
        }

        public async Task LogoutAsync() => await _signInManager.SignOutAsync();

        public Task<T?> FindUserAsync<T>(ClaimsPrincipal principal) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsUserInRoleAsync(string userNameOrEmail, string role)
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetUserIdAsync(ClaimsPrincipal principal)
        {
            throw new NotImplementedException();
        }
    }
}
