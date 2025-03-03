using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThanTai.Models;
using System.Security.Claims;

namespace ThanTai.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ThanTaiShopDbContext _context;

        public HomeController(ILogger<HomeController> logger, ThanTaiShopDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirst("ID")?.Value;

            if (userId != null)
            {
                var nguoiDung = _context.NguoiDung.FirstOrDefault(x => x.ID.ToString() == userId);
                if (nguoiDung != null)
                {
                    ViewBag.UserName = nguoiDung.HoVaTen;
                    ViewBag.UserImage = nguoiDung.Anh;
                }
            }

            return View();
        }
    }
}
