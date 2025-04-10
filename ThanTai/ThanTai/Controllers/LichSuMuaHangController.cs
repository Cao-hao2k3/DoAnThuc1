using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThanTai.Models;

namespace ThanTai.Controllers
{
    public class LichSuMuaHangController : Controller
    {
        private readonly ThanTaiShopDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LichSuMuaHangController(ThanTaiShopDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index()
        {
            var userIdSession = HttpContext.Session.GetInt32("UserID");
            if (!userIdSession.HasValue)
            {
                return Unauthorized("Người dùng chưa đăng nhập hoặc thông tin không hợp lệ.");
            }

            // Truy vấn lấy danh sách đặt hàng của người dùng
            var donDatHang = await _context.DatHang
                .Where(dh => dh.NguoiDungID == userIdSession)
                .Include(dh => dh.NguoiDung)
                .Include(dh => dh.TinhTrang)
                .Include(dh => dh.DatHangChiTiet)
                    .ThenInclude(dhct => dhct.SanPham)
                .ToListAsync();

            if (!donDatHang.Any())
            {
                return View("KhongCoDonHang"); // Hiển thị View thông báo không có đơn hàng
            }

            // Không sử dụng ViewModel mà trả trực tiếp Model `DatHang`
            return View(donDatHang);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDonHang(int orderId)
        {
            Console.WriteLine($"🔹 Yêu cầu hủy đơn hàng: {orderId}");

            var donHang = await _context.DatHang.FindAsync(orderId);
            if (donHang == null)
            {
                Console.WriteLine($"❌ Không tìm thấy đơn hàng với ID {orderId}");
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng!";
                return RedirectToAction("Index");
            }

            // ✅ Kiểm tra trạng thái đơn hàng (chỉ hủy nếu đang xử lý - ID = 3)
            if (donHang.TinhTrangID != 3)
            {
                Console.WriteLine($"⚠️ Không thể hủy đơn hàng {orderId}, trạng thái hiện tại: {donHang.TinhTrangID}");
                TempData["ErrorMessage"] = "Chỉ có thể hủy đơn hàng đang xử lý!";
                return RedirectToAction("Index");
            }

            // 🛠 Cập nhật trạng thái đơn hàng thành "Đã hủy" (ID = 7)
            donHang.TinhTrangID = 7;
            Console.WriteLine($"✅ Đơn hàng {orderId} cập nhật trạng thái: {donHang.TinhTrangID}");

            try
            {
                _context.Entry(donHang).State = EntityState.Modified;
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    Console.WriteLine($"✅ Đơn hàng {orderId} đã được cập nhật thành công trong DB!");
                    TempData["SuccessMessage"] = "Đơn hàng đã được hủy thành công!";
                }
                else
                {
                    Console.WriteLine($"❌ Lỗi khi cập nhật đơn hàng {orderId} trong DB!");
                    TempData["ErrorMessage"] = "Có lỗi xảy ra, không thể hủy đơn hàng!";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi Exception khi cập nhật đơn hàng {orderId}: {ex.Message}");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi hủy đơn hàng!";
            }

            return RedirectToAction("Index");
        }



    }
}
