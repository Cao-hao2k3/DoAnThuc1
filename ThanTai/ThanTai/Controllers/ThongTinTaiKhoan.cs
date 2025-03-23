using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThanTai.Models;
using ThanTai.ViewModels;

namespace ThanTai.Controllers
{
    public class ThongTinTaiKhoan : Controller
    {
        private readonly ThanTaiShopDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ThongTinTaiKhoan(ThanTaiShopDbContext context, IHttpContextAccessor httpContextAccessor) // Đổi lại tên constructor
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // Hiển thị thông tin người dùng
        public IActionResult Index(int? id)
        {
            // Kiểm tra nếu không có ID thì lấy từ Session
            int? userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserID");

            if (userId == 0)
            {
                // Nếu không có ID, chuyển về trang đăng nhập
                return RedirectToAction("Login", "Home");
            }

            // Lấy thông tin người dùng từ cơ sở dữ liệu
            var nguoiDung = _context.NguoiDung.SingleOrDefault(u => u.ID == userId);

            if (nguoiDung == null)
            {
                // Nếu không tìm thấy người dùng
                return RedirectToAction("Login", "Home");
            }

            // Trả về View với lớp chỉnh sửa dành cho khách hàng
            return View(new NguoiDung_ChinhSua(nguoiDung));
        }

        // Xử lý cập nhật thông tin
        [HttpPost]
        public async Task<IActionResult> CapNhatThongTin(NguoiDung_ChinhSua model)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ ModelState không hợp lệ!");
                foreach (var error in ModelState)
                {
                    foreach (var subError in error.Value.Errors)
                    {
                        Console.WriteLine($"🔸 Field: {error.Key} - Error: {subError.ErrorMessage}");
                    }
                }
                return View("Index", model);
            }

            var nguoiDung = _context.NguoiDung.SingleOrDefault(u => u.ID == model.ID);
            if (nguoiDung == null)
            {
                ModelState.AddModelError("", "Người dùng không tồn tại.");
                return View("Index", model);
            }

            // Cập nhật thông tin
            nguoiDung.HoVaTen = model.HoVaTen;
            nguoiDung.Email = model.Email;
            nguoiDung.DienThoai = model.DienThoai;
            nguoiDung.DiaChi = model.DiaChi;
            nguoiDung.TenDangNhap = model.TenDangNhap;
            nguoiDung.Quyen = model.Quyen;

            // Cập nhật mật khẩu nếu có nhập
            if (!string.IsNullOrEmpty(model.MatKhauMoi))
            {
                nguoiDung.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhauMoi);
            }

            // Xử lý ảnh đại diện nếu có tải lên
            if (model.DuLieuHinhAnh != null && model.DuLieuHinhAnh.Length > 0)
            {
                var fileName = Path.GetFileName(model.DuLieuHinhAnh.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.DuLieuHinhAnh.CopyToAsync(stream);
                }

                nguoiDung.Anh = "/uploads/" + fileName;
            }

            // Lưu thay đổi
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Cập nhật thông tin thành công.";

            return RedirectToAction("Index");
        }
    }
}
