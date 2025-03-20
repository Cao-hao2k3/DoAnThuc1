using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThanTai.Models;

namespace ThanTai.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class QuanLyKhoHangController : Controller
    {
        private readonly ThanTaiShopDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public QuanLyKhoHangController(ThanTaiShopDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            var sanPhams = _context.SanPham.Include(sp => sp.ThuongHieu).ToList();
            return View(sanPhams);
        }

        // GET: Nhập hàng
        public IActionResult NhapHang()
        {
            ViewData["SanPhamID"] = new SelectList(_context.SanPham, "ID", "TenSanPham");
            return View();
        }

        // POST: Nhập hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NhapHang([Bind("SanPhamID,SoLuong,GhiChu")] QuanLyKhoHang quanLyKhoHang)
        {
            try
            {
                // Kiểm tra session UserID
                int? userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserID");

                // Nếu chưa đăng nhập, chuyển về trang Login
                if (userId == null)
                {
                    return RedirectToAction("Login", "Home");
                }

                quanLyKhoHang.NguoiDungID = userId.Value;
                quanLyKhoHang.LoaiGiaoDich = 1;
                quanLyKhoHang.ThoiGian = DateTime.Now;

       
                _context.QuanLyKhoHang.Add(quanLyKhoHang);

                var sanPham = await _context.SanPham.FindAsync(quanLyKhoHang.SanPhamID);
                if (sanPham == null)
                {
                    ModelState.AddModelError("", "Lỗi: Sản phẩm không tồn tại!");
                    return View(quanLyKhoHang);
                }

                sanPham.SoLuong += quanLyKhoHang.SoLuong;
                _context.SanPham.Update(sanPham);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi nhập hàng: " + ex.Message);
                ModelState.AddModelError("", "Lỗi khi lưu dữ liệu!");
            }

            // Load lại danh sách sản phẩm nếu có lỗi
            ViewData["SanPhamID"] = new SelectList(_context.SanPham, "ID", "TenSanPham", quanLyKhoHang.SanPhamID);
            return View(quanLyKhoHang);
        }

        public async Task<IActionResult> HienThiLichSu()
        {
            var danhSachKho = await _context.QuanLyKhoHang
                .Include(q => q.SanPham)
                .Include(q => q.NguoiDung)
                .ToListAsync(); // Thêm await vào đây

            return View(danhSachKho);
        }
    }
}
