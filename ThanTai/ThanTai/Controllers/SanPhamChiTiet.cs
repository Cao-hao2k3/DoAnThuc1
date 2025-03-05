using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using ThanTai.Models;

namespace ThanTai.Controllers
{
    public class SanPhamChiTietController : Controller
    {
        private readonly ThanTaiShopDbContext _context;

        public SanPhamChiTietController(ThanTaiShopDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int id)
        {
            var sanPham = _context.SanPham
                .Include(sp => sp.HinhAnhSanPham)  // Lấy ảnh sản phẩm
                .Include(sp => sp.LoaiSanPham)    // Lấy thông tin loại sản phẩm
                .Include(sp => sp.ThuongHieu)     // Lấy thông tin thương hiệu
                .Include(sp => sp.GiaTriThuocTinhs)  // Lấy giá trị thuộc tính
                .ThenInclude(gt => gt.ThuocTinh)  // Lấy thông tin thuộc tính
                .FirstOrDefault(sp => sp.ID == id);

            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }
    }
}
