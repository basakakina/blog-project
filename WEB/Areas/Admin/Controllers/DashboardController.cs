using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WEB.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("admin")]
    public class DashboardController : Controller
    {
        [HttpGet("")]
        [HttpGet("dashboard")]
        public IActionResult Index() => View();
    }
}

