using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ThanTai.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DoanhThuController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SoSanhDoanhThu()
        {
            return View();

        }
    }
}
