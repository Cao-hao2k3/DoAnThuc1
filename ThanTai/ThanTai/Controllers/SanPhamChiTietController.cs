using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
                .Include(sp => sp.HinhAnhSanPham) // Đảm bảo load HinhAnhSanPham
                .Include(sp => sp.LoaiSanPham)
                .Include(sp => sp.ThuongHieu)
                .Include(sp => sp.GiaTriThuocTinh)
                .ThenInclude(gt => gt.ThuocTinh)
                .FirstOrDefault(sp => sp.ID == id);

            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }


        [HttpPost]
        public IActionResult ThemVaoGioHang(int sanPhamID, int soLuong = 1)
        {
            int? nguoiDungID = HttpContext.Session.GetInt32("UserID");
            if (nguoiDungID == null)
            {
                TempData["ThongBaoLoi"] = "Bạn cần đăng nhập để thêm vào giỏ hàng!";
                return RedirectToAction("Login", "Home");
            }

            var sanPham = _context.SanPham.Find(sanPhamID);
            if (sanPham == null)
            {
                TempData["ThongBaoLoi"] = "Sản phẩm không tồn tại!";
                return RedirectToAction("Index", "SanPhamChiTiet", new { id = sanPhamID });
            }

            try
            {
                var gioHangItem = _context.GioHang
                    .FirstOrDefault(g => g.NguoiDungID == nguoiDungID && g.SanPhamID == sanPhamID);

                if (gioHangItem != null)
                {
                    gioHangItem.SoLuong += soLuong;
                    _context.GioHang.Update(gioHangItem);
                }
                else
                {
                    gioHangItem = new GioHang
                    {
                        NguoiDungID = nguoiDungID.Value,
                        SanPhamID = sanPhamID,
                        SoLuong = soLuong
                    };
                    _context.GioHang.Add(gioHangItem);
                }

                _context.SaveChanges();

                // ✅ Thành công -> hiển thị thông báo màu xanh
                TempData["ThongBao"] = "Đã thêm sản phẩm vào giỏ hàng thành công!";
            }
            catch
            {
                // ❌ Thất bại -> hiển thị thông báo màu đỏ
                TempData["ThongBaoLoi"] = "Thêm vào giỏ hàng thất bại, vui lòng thử lại!";
            }

            return RedirectToAction("Index", "SanPhamChiTiet", new { id = sanPhamID });
        }

       

    }
}
