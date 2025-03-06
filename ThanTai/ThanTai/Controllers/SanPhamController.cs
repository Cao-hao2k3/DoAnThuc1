using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThanTai.Models;

namespace ThanTai.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly ThanTaiShopDbContext _context;

        public SanPhamController(ThanTaiShopDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var sanPhams = await _context.SanPham
                .Include(sp => sp.HinhAnhSanPham) // Load hình ảnh sản phẩm
                .ToListAsync();

            return View(sanPhams);
        }

    }
}
