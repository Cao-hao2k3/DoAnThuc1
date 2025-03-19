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
        public IActionResult HuyDonHang(int orderId)
        {
            // Tìm đơn hàng theo ID
            var order = _context.DatHang
                .Include(o => o.DatHangChiTiet) // Bao gồm chi tiết đơn hàng
                .ThenInclude(dhct => dhct.SanPham) // Bao gồm thông tin sản phẩm
                .FirstOrDefault(o => o.ID == orderId);

            if (order != null)
            {
                // Kiểm tra tình trạng đơn hàng
                if (order.TinhTrangID == 4 || order.TinhTrangID == 3)
                {
                    TempData["ErrorMessage"] = order.TinhTrangID == 4
                        ? "Đơn hàng đang vận chuyển, không thể hủy!"
                        : "Đơn hàng đã thành công, không thể hủy!";
                    return RedirectToAction("Index"); // Quay lại trang danh sách
                }

                // Cộng lại số lượng sản phẩm trong kho
                foreach (var chiTiet in order.DatHangChiTiet)
                {
                    var sanPham = chiTiet.SanPham;
                    if (sanPham != null)
                    {
                        sanPham.SoLuong += chiTiet.SoLuong;
                        _context.SanPham.Update(sanPham);
                    }
                }

                // Xóa đơn hàng
                _context.DatHang.Remove(order);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Đơn hàng đã được hủy thành công và số lượng sản phẩm đã được cập nhật!";
            }
            else
            {
                TempData["ErrorMessage"] = "Đơn hàng không tồn tại!";
            }

            return RedirectToAction("Index"); // Quay lại trang danh sách
        }
    }
}
