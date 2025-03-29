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

        public async Task<IActionResult> TuLanhView(int? ThuongHieuID, int? SoCanhCua, string Gia, string CongNgheInverter)
        {
            decimal? GiaMin = null;
            decimal? GiaMax = null;

            if (!string.IsNullOrEmpty(Gia))
            {
                var parts = Gia.Split('-').Select(p => decimal.TryParse(p, out var val) ? val : (decimal?)null).ToArray();
                GiaMin = parts.Length > 0 ? parts[0] : null;
                GiaMax = parts.Length > 1 ? parts[1] : null;
            }

            var query = _context.SanPham
                .Include(sp => sp.ThuongHieu)
                .Include(sp => sp.GiaTriThuocTinh)
                    .ThenInclude(gt => gt.ThuocTinh)
                .Include(sp => sp.HinhAnhSanPham)
                .Where(sp => sp.LoaiSanPhamID == 5) // Chỉ lấy sản phẩm loại tủ lạnh
                .AsQueryable();

            if (ThuongHieuID.HasValue)
            {
                query = query.Where(sp => sp.ThuongHieuID == ThuongHieuID.Value);
            }

            if (SoCanhCua.HasValue)
            {
                query = query.Where(sp => sp.GiaTriThuocTinh.Any(gt =>
                    gt.ThuocTinh.TenThuocTinh == "Số cánh cửa" && gt.GiaTri == SoCanhCua.Value.ToString()
                ));
            }

            if (GiaMin.HasValue)
            {
                query = query.Where(sp => sp.GiaSauKhiGiam >= GiaMin.Value);
            }

            if (GiaMax.HasValue)
            {
                query = query.Where(sp => sp.GiaSauKhiGiam <= GiaMax.Value);
            }

            if (!string.IsNullOrEmpty(CongNgheInverter))
            {
                query = query.Where(sp => sp.GiaTriThuocTinh.Any(gt =>
                    gt.ThuocTinh.TenThuocTinh == "Công nghệ Inverter" && gt.GiaTri == CongNgheInverter
                ));
            }

            var sanPhams = await query.ToListAsync();

            ViewBag.ThuongHieus = await _context.ThuongHieu.ToListAsync();
            ViewBag.SelectedThuongHieu = ThuongHieuID;
            ViewBag.SelectedSoCanh = SoCanhCua;
            ViewBag.SelectedCongNghe = CongNgheInverter;

            return View(sanPhams);
        }

        public IActionResult MayLanhView()
        {
            var mayLanh = _context.SanPham
                .Include(sp => sp.HinhAnhSanPham)
                .Where(sp => sp.LoaiSanPhamID == 9)
                .ToList();

            return View(mayLanh);
        }
       
    }
}
