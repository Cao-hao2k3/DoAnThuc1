using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using ThanTai.Models;

namespace ThanTai.Controllers
{
    public class DanhGiaController : Controller
    {
        private readonly ThanTaiShopDbContext _context;

        public DanhGiaController(ThanTaiShopDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ChiTiet(int id)
        {
            var sanPham = await _context.SanPham
                .Include(s => s.ThuongHieu)
                .Include(s => s.LoaiSanPham)
                .Include(s => s.HinhAnhSanPham)
                .Include(s => s.DanhGiaSanPham)
                    .ThenInclude(d => d.NguoiDung) // Lấy thông tin người dùng đánh giá
                .FirstOrDefaultAsync(m => m.ID == id);

            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        [HttpPost]
        public IActionResult GuiDanhGia(int sanPhamID, int soSao, string noiDung)
        {
            int? nguoiDungID = HttpContext.Session.GetInt32("UserID");
            if (nguoiDungID == null)
            {
                TempData["ThongBaoLoi"] = "Bạn cần đăng nhập để đánh giá sản phẩm!";
                return RedirectToAction("Login", "Home");
            }

            var danhGia = new DanhGiaSanPham
            {
                NguoiDungID = nguoiDungID.Value,
                SanPhamID = sanPhamID,
                SoSao = soSao,
                BinhLuan = noiDung,
                NgayDanhGia = DateTime.Now
            };

            try
            {
                _context.DanhGiaSanPham.Add(danhGia);
                _context.SaveChanges();

                TempData["ThongBao"] = "Đánh giá của bạn đã được gửi thành công!";
            }
            catch
            {
                TempData["ThongBaoLoi"] = "Gửi đánh giá thất bại, vui lòng thử lại!";
            }

            return RedirectToAction("Index", "SanPhamChiTiet", new { id = sanPhamID });
        }
    }
}
