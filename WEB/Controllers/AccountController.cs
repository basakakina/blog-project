using ApplicationCore.UserEntites.Concrete;
using AutoMapper;
using Business.Manager.Concrete;
using Business.Manager.Interface;
using DTO.Concrete.AccountDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WEB.Models.Account;

namespace WEB.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserManager _userManagerFacade;
        private readonly IMapper _mapper;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public AccountController(IUserManager userManagerFacade, IMapper mapper, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _userManagerFacade = userManagerFacade; 
            _mapper = mapper;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl; 
            return View();
        }

      
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

          
            var loginName = model.UserName;
            if (!string.IsNullOrWhiteSpace(loginName) && loginName.Contains("@"))
            {
                var byEmail = await _userManager.FindByEmailAsync(loginName);
                if (byEmail != null) loginName = byEmail.UserName;
            }

            var result = await _signInManager.PasswordSignInAsync(
                loginName, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return LocalRedirect(returnUrl);

              
                var user = await _userManager.FindByNameAsync(loginName);
                if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

                
                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = "Giriş başarısız. Bilgilerinizi kontrol edin.";
            return View(model);
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }


        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDTO model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

    
            var userName = string.IsNullOrWhiteSpace(model.UserName) ? model.Email : model.UserName;

            var user = new AppUser
            {
                UserName = userName,
                Email = model.Email,
                EmailConfirmed = true
            };

            var create = await _userManager.CreateAsync(user, model.Password);

            if (!create.Succeeded)
            {
                foreach (var e in create.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);

                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

          
            TempData["Success"] = $"Hoş geldin {user.UserName}! Kayıt tamamlandı.";

          
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

    
            return RedirectToAction("Index", "Home");

        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string? returnUrl = null)
        {
            await _signInManager.SignOutAsync();

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return Redirect("/"); 
        }
    }
}
