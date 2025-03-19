using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThanTai.Models; // Import model
using System.Linq;

namespace ThanTai.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DoanhThuController : Controller
    {
        private readonly ThanTaiShopDbContext _context;

        public DoanhThuController(ThanTaiShopDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Ngay = Enumerable.Range(1, 31); // Danh sách ngày 1-31
            ViewBag.Thang = Enumerable.Range(1, 12); // Danh sách tháng 1-12
            ViewBag.Nam = Enumerable.Range(2018, 13).ToList(); // Danh sách năm từ 2018 đến 2030

            ViewBag.DoanhThu = null;
            ViewBag.TongTien = 0;
            return View();
        }

        public IActionResult SoSanhDoanhThu()
        {

            ViewBag.Ngay = Enumerable.Range(1, 31); // Danh sách ngày 1-31
            ViewBag.Thang = Enumerable.Range(1, 12); // Danh sách tháng 1-12
            ViewBag.Nam = Enumerable.Range(2018, 13).ToList(); // Danh sách năm từ 2018 đến 2030

            ViewBag.DoanhThu = null;
            ViewBag.TongTien = 0;

            return View();
        }

        [HttpGet]
        public IActionResult XuLyVe(int? selectedNam)
        {
            if (!selectedNam.HasValue)
            {
                return Json(new { categories = new string[0], revenue = new double[0] });
            }

            // Lọc đơn hàng theo năm được chọn
            var doanhThuThang = _context.DatHang
               .Where(dh => dh.NgayDatHang.Year == selectedNam.Value)
               .Include(dh => dh.DatHangChiTiet)
               .SelectMany(dh => dh.DatHangChiTiet, (dh, ct) => new
               {
                   Month = dh.NgayDatHang.Month,
                   Revenue = (double)(ct.DonGia * ct.SoLuong) // Chuyển đổi kiểu decimal -> double
               })
               .GroupBy(d => d.Month)
               .Select(group => new
               {
                   Month = group.Key,
                   TotalRevenue = (double)group.Sum(g => g.Revenue) // Chuyển đổi kiểu decimal -> double
               })
               .OrderBy(g => g.Month)
               .ToList();


            // Tạo danh sách 12 tháng
            var categories = Enumerable.Range(1, 12)
                .Select(m => $"Tháng {m}")
                .ToArray();

            // Khởi tạo doanh thu các tháng là 0
            var revenue = new double[12];

            // Cập nhật doanh thu từng tháng có dữ liệu
            foreach (var dt in doanhThuThang)
            {
                revenue[dt.Month - 1] = (double)dt.TotalRevenue;
            }

            return Json(new { categories, revenue });
        }


        [HttpPost]
        public IActionResult Index(int? selectedNgay, int? selectedThang, int? selectedNam)
        {
            ViewBag.Ngay = Enumerable.Range(1, 31);
            ViewBag.Thang = Enumerable.Range(1, 12);
            ViewBag.Nam = Enumerable.Range(2018, 13).ToList();

            var query = _context.DatHang
                                .Include(dh => dh.DatHangChiTiet)
                                .AsQueryable();

            // Lọc theo ngày, tháng và năm
            if (selectedNgay.HasValue)
                query = query.Where(dh => dh.NgayDatHang.Day == selectedNgay.Value);

            if (selectedThang.HasValue)
                query = query.Where(dh => dh.NgayDatHang.Month == selectedThang.Value);

            if (selectedNam.HasValue)
                query = query.Where(dh => dh.NgayDatHang.Year == selectedNam.Value);

            var doanhThu = query.ToList();

            // Tính tổng tiền
            decimal tongTien = doanhThu.Sum(dh => dh.DatHangChiTiet
                                                   .Sum(ct => ct.DonGia * ct.SoLuong));

            ViewBag.DoanhThu = doanhThu;
            ViewBag.TongTien = tongTien;
            ViewBag.SelectedNgay = selectedNgay;
            ViewBag.SelectedThang = selectedThang;
            ViewBag.SelectedNam = selectedNam;

            return View();
        }

        public IActionResult ChiTiet(int id)
        {
            var chiTietDonHang = _context.DatHangChiTiet
                .Include(d => d.SanPham)
                .Where(d => d.DatHangID == id)
                .ToList();

            if (chiTietDonHang == null || !chiTietDonHang.Any())
            {
                return NotFound();
            }

            return View(chiTietDonHang);
        }
    }
}
