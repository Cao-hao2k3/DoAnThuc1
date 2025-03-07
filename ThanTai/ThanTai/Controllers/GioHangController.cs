using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using ThanTai.Models;

namespace ThanTai.Controllers
{
    public class GioHangController : Controller
    {
        private readonly ThanTaiShopDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GioHangController(ThanTaiShopDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            // Lấy ID tài khoản từ session
            int? userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account"); // Chuyển hướng về trang đăng nhập nếu chưa đăng nhập
            }

            // Lấy giỏ hàng của người dùng
            var gioHang = _context.GioHang
                .Where(g => g.NguoiDungID == userId)
                .Include(g => g.SanPham)
                .ThenInclude(s => s.HinhAnhSanPham)
                .ToList();

            // Lấy ảnh đầu tiên từ JSON ngay trong controller
            foreach (var item in gioHang)
            {
                if (item.SanPham?.HinhAnhSanPham != null)
                {
                    var firstImage = GetFirstImageFromJson(item.SanPham.HinhAnhSanPham.FirstOrDefault()?.AnhSanPham);
                    item.SanPham.HinhAnhSanPham.FirstOrDefault().AnhSanPham = firstImage;
                }
            }

            return View(gioHang);
        }

        // Phương thức lấy ảnh đầu tiên từ chuỗi JSON
        private string GetFirstImageFromJson(string jsonImages)
        {
            if (!string.IsNullOrEmpty(jsonImages))
            {
                try
                {
                    var images = JsonConvert.DeserializeObject<List<string>>(jsonImages);
                    return images?.FirstOrDefault() ?? "/uploads/no-image.jpg";
                }
                catch
                {
                    return "/uploads/no-image.jpg"; // Tránh lỗi JSON
                }
            }
            return "/uploads/no-image.jpg"; // Trả về ảnh mặc định nếu dữ liệu trống
        }

        [HttpPost]
        public IActionResult CapNhatSoLuong(int gioHangId, int thayDoi)
        {
            var gioHang = _context.GioHang.FirstOrDefault(g => g.ID == gioHangId);
            if (gioHang != null)
            {
                gioHang.SoLuong += thayDoi;
                if (gioHang.SoLuong <= 0)
                {
                    _context.GioHang.Remove(gioHang); // Xóa nếu số lượng <= 0
                }
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult XoaSanPham(int gioHangId)
        {
            var gioHang = _context.GioHang.FirstOrDefault(g => g.ID == gioHangId);
            if (gioHang != null)
            {
                _context.GioHang.Remove(gioHang);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

    }
}
