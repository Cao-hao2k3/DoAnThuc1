using Microsoft.AspNetCore.Mvc;

namespace ThanTai.Controllers
{
    public class GioHangController : Controller
    {
        public int NguoiDung { get; internal set; }
        public int SanPhamID { get; internal set; }
        public int SoLuong { get; internal set; }
        public int NguoiDungID { get; internal set; }

        public IActionResult Index()
        {
            return View();
        }
    }
}
