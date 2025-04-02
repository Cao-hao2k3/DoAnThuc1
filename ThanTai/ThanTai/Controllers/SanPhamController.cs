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
        //View Trang chủ
        public async Task<IActionResult> Index()
        {
            var sanPhams = await _context.SanPham
                .Include(sp => sp.HinhAnhSanPham) // Load hình ảnh sản phẩm
                .ToListAsync();

            return View(sanPhams);
        }
        //View Tủ lạnh
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
        //View Máy lạnh
        public async Task<IActionResult> MayLanhView(int? ThuongHieuID, string? CongSuat, string? Gia, string? LoaiMay, string? KieuDang)
        {
            decimal? GiaMin = null;
            decimal? GiaMax = null;

            if (!string.IsNullOrEmpty(Gia))
            {
                var parts = Gia.Split('-').Select(p => decimal.TryParse(p, out var val) ? val : (decimal?)null).ToArray();
                if (parts.Length == 2)
                {
                    GiaMin = parts[0];
                    GiaMax = parts[1];
                }
            }

            var query = _context.SanPham
                .Include(sp => sp.ThuongHieu)
                .Include(sp => sp.GiaTriThuocTinh)
                    .ThenInclude(gt => gt.ThuocTinh)
                .Include(sp => sp.HinhAnhSanPham)
                .Where(sp => sp.LoaiSanPhamID == 9) // Chỉ lấy sản phẩm máy lạnh
                .AsQueryable();

            // Lọc theo thương hiệu nếu được chọn
            if (ThuongHieuID.HasValue)
            {
                query = query.Where(sp => sp.ThuongHieuID == ThuongHieuID.Value);
            }

            // Lọc theo công suất nếu có
            if (!string.IsNullOrEmpty(CongSuat))
            {
                query = query.Where(sp => sp.GiaTriThuocTinh.Any(gt =>
                    gt.ThuocTinh.TenThuocTinh == "Công suất làm lạnh" && gt.GiaTri == CongSuat
                ));
            }

            // Lọc theo giá nếu có
            if (GiaMin.HasValue)
            {
                query = query.Where(sp => sp.GiaSauKhiGiam >= GiaMin.Value);
            }

            if (GiaMax.HasValue)
            {
                query = query.Where(sp => sp.GiaSauKhiGiam <= GiaMax.Value);
            }

            // Lọc theo loại máy nếu có
            if (!string.IsNullOrEmpty(LoaiMay))
            {
                query = query.Where(sp => sp.GiaTriThuocTinh.Any(gt =>
                    gt.ThuocTinh.TenThuocTinh == "Loại Máy" && gt.GiaTri == LoaiMay
                ));
            }

            // Lọc theo kiểu dáng nếu có
            if (!string.IsNullOrEmpty(KieuDang))
            {
                query = query.Where(sp => sp.GiaTriThuocTinh.Any(gt =>
                    gt.ThuocTinh.TenThuocTinh == "Kiểu dáng" && gt.GiaTri == KieuDang
                ));
            }

            var mayLanhs = await query.ToListAsync();

            // Gửi danh sách thương hiệu xuống view để render bộ lọc
            ViewBag.ThuongHieus = await _context.ThuongHieu.ToListAsync();

            // Lưu lại giá trị đã chọn để giữ nguyên trên giao diện sau khi lọc
            ViewBag.SelectedThuongHieu = ThuongHieuID;
            ViewBag.SelectedCongSuat = CongSuat;
            ViewBag.SelectedLoaiMay = LoaiMay;
            ViewBag.SelectedKieuDang = KieuDang;

            // Nếu không có sản phẩm nào thỏa mãn, trả về view với thông báo
            if (!mayLanhs.Any())
            {
                ViewBag.NoResultsMessage = "Không có sản phẩm nào thỏa mãn điều kiện lọc.";
            }

            return View(mayLanhs);
        }
        //View Tivi
        public async Task<IActionResult> TiviView(int? ThuongHieuID, string? Gia, string? DoPhanGiai, string? LoaiTivi)
        {
            decimal? GiaMin = null;
            decimal? GiaMax = null;

            if (!string.IsNullOrEmpty(Gia))
            {
                var parts = Gia.Split('-').Select(p => decimal.TryParse(p, out var val) ? val : (decimal?)null).ToArray();
                if (parts.Length == 2)
                {
                    GiaMin = parts[0];
                    GiaMax = parts[1];
                }
            }

            var query = _context.SanPham
                .Include(sp => sp.ThuongHieu)
                .Include(sp => sp.GiaTriThuocTinh)
                    .ThenInclude(gt => gt.ThuocTinh)
                .Include(sp => sp.HinhAnhSanPham)
                .Where(sp => sp.LoaiSanPhamID == 8) // Chỉ lấy sản phẩm tivi
                .AsQueryable();

            // Lọc theo thương hiệu nếu được chọn
            if (ThuongHieuID.HasValue)
            {
                query = query.Where(sp => sp.ThuongHieuID == ThuongHieuID.Value);
            }

            // Lọc theo độ phân giải nếu có
            if (!string.IsNullOrEmpty(DoPhanGiai))
            {
                query = query.Where(sp => sp.GiaTriThuocTinh.Any(gt =>
                    gt.ThuocTinh.TenThuocTinh == "Độ phân giải" && gt.GiaTri == DoPhanGiai
                ));
            }

            // Lọc theo giá nếu có
            if (GiaMin.HasValue)
            {
                query = query.Where(sp => sp.GiaSauKhiGiam >= GiaMin.Value);
            }

            if (GiaMax.HasValue)
            {
                query = query.Where(sp => sp.GiaSauKhiGiam <= GiaMax.Value);
            }

            // Lọc theo loại tivi nếu có
            if (!string.IsNullOrEmpty(LoaiTivi))
            {
                query = query.Where(sp => sp.GiaTriThuocTinh.Any(gt =>
                    gt.ThuocTinh.TenThuocTinh == "Loại Tivi" && gt.GiaTri == LoaiTivi
                ));
            }

            var tiviList = await query.ToListAsync();

            // Gửi danh sách thương hiệu xuống view để render bộ lọc
            ViewBag.ThuongHieus = await _context.ThuongHieu.ToListAsync();

            // Lưu lại giá trị đã chọn để giữ nguyên trên giao diện sau khi lọc
            ViewBag.SelectedThuongHieu = ThuongHieuID;
            ViewBag.SelectedDoPhanGiai = DoPhanGiai;
            ViewBag.SelectedLoaiTivi = LoaiTivi;
            ViewBag.SelectedGia = Gia;

            // Nếu không có sản phẩm nào thỏa mãn, trả về view với thông báo
            if (!tiviList.Any())
            {
                ViewBag.NoResultsMessage = "Không có sản phẩm nào thỏa mãn điều kiện lọc.";
            }

            return View(tiviList);
        }
        //View Máy giặt
        public async Task<IActionResult> MayGiatView(int? ThuongHieuID, string? Gia, string? LoaiMay, string? KhoiLuong)
        {
            decimal? GiaMin = null;
            decimal? GiaMax = null;

            if (!string.IsNullOrEmpty(Gia))
            {
                var parts = Gia.Split('-').Select(p => decimal.TryParse(p, out var val) ? val : (decimal?)null).ToArray();
                if (parts.Length == 2)
                {
                    GiaMin = parts[0];
                    GiaMax = parts[1];
                }
            }

            var query = _context.SanPham
                .Include(sp => sp.ThuongHieu)
                .Include(sp => sp.GiaTriThuocTinh)
                    .ThenInclude(gt => gt.ThuocTinh)
                .Include(sp => sp.HinhAnhSanPham)
                .Where(sp => sp.LoaiSanPhamID == 10) // Chỉ lấy sản phẩm máy giặt
                .AsQueryable();

            // Lọc theo thương hiệu nếu được chọn
            if (ThuongHieuID.HasValue)
            {
                query = query.Where(sp => sp.ThuongHieuID == ThuongHieuID.Value);
            }

            // Lọc theo loại máy giặt nếu có
            if (!string.IsNullOrEmpty(LoaiMay))
            {
                query = query.Where(sp => sp.GiaTriThuocTinh.Any(gt =>
                    gt.ThuocTinh.TenThuocTinh == "Loại máy giặt" && gt.GiaTri == LoaiMay
                ));
            }

            // Lọc theo khối lượng giặt nếu có
            if (!string.IsNullOrEmpty(KhoiLuong))
            {
                query = query.Where(sp => sp.GiaTriThuocTinh.Any(gt =>
                    gt.ThuocTinh.TenThuocTinh == "Khối lượng giặt" && gt.GiaTri == KhoiLuong
                ));
            }

            // Lọc theo giá nếu có
            if (GiaMin.HasValue)
            {
                query = query.Where(sp => sp.GiaSauKhiGiam >= GiaMin.Value);
            }

            if (GiaMax.HasValue)
            {
                query = query.Where(sp => sp.GiaSauKhiGiam <= GiaMax.Value);
            }

            var mayGiatList = await query.ToListAsync();

            // Gửi danh sách thương hiệu xuống view để render bộ lọc
            ViewBag.ThuongHieus = await _context.ThuongHieu.ToListAsync();

            // Lưu lại giá trị đã chọn để giữ nguyên trên giao diện sau khi lọc
            ViewBag.SelectedThuongHieu = ThuongHieuID;
            ViewBag.SelectedLoaiMay = LoaiMay;
            ViewBag.SelectedKhoiLuong = KhoiLuong;
            ViewBag.SelectedGia = Gia;

            // Nếu không có sản phẩm nào thỏa mãn, trả về view với thông báo
            if (!mayGiatList.Any())
            {
                ViewBag.NoResultsMessage = "Không có sản phẩm nào thỏa mãn điều kiện lọc.";
            }

            return View(mayGiatList);
        }

        //View May Loc khong khi
        public async Task<IActionResult> MayLocKhongKhiView(int? ThuongHieuID, string? Gia, string? PhamViLoc, string? BangDieuKhien, string? LoaiBuiLocDuoc)
        {
            decimal? GiaMin = null, GiaMax = null;

            if (!string.IsNullOrEmpty(Gia))
            {
                var parts = Gia.Split('-').Select(p => decimal.TryParse(p, out var val) ? val : (decimal?)null).ToArray();
                if (parts.Length == 2)
                {
                    GiaMin = parts[0];
                    GiaMax = parts[1];
                }
            }

            var query = _context.SanPham
                .Include(sp => sp.ThuongHieu)
                .Include(sp => sp.GiaTriThuocTinh)
                    .ThenInclude(gt => gt.ThuocTinh)
                .Include(sp => sp.HinhAnhSanPham)
                .Where(sp => sp.LoaiSanPhamID == 12) // ID 11 đại diện cho Máy lọc không khí
                .AsQueryable();

            // Lọc theo thương hiệu
            if (ThuongHieuID.HasValue)
            {
                query = query.Where(sp => sp.ThuongHieuID == ThuongHieuID.Value);
            }

            // Lọc theo phạm vi lọc
            if (!string.IsNullOrEmpty(PhamViLoc))
            {
                query = query.Where(sp => sp.GiaTriThuocTinh.Any(gt =>
                    gt.ThuocTinh.TenThuocTinh == "Phạm vi lọc" && gt.GiaTri == PhamViLoc));
            }

            // Lọc theo bảng điều khiển
            if (!string.IsNullOrEmpty(BangDieuKhien))
            {
                query = query.Where(sp => sp.GiaTriThuocTinh.Any(gt =>
                    gt.ThuocTinh.TenThuocTinh == "Bảng điều khiển" && gt.GiaTri == BangDieuKhien));
            }

            // Lọc theo loại bụi lọc được
            if (!string.IsNullOrEmpty(LoaiBuiLocDuoc))
            {
                query = query.Where(sp => sp.GiaTriThuocTinh.Any(gt =>
                    gt.ThuocTinh.TenThuocTinh == "Loại bụi lọc được" && gt.GiaTri == LoaiBuiLocDuoc));
            }

            // Lọc theo giá
            if (GiaMin.HasValue)
            {
                query = query.Where(sp => sp.GiaSauKhiGiam >= GiaMin.Value);
            }
            if (GiaMax.HasValue)
            {
                query = query.Where(sp => sp.GiaSauKhiGiam <= GiaMax.Value);
            }

            var mayLocList = await query.ToListAsync();

            // Gửi danh sách thương hiệu xuống view
            ViewBag.ThuongHieus = await _context.ThuongHieu.ToListAsync();

            // Lưu giá trị bộ lọc để giữ nguyên sau khi lọc
            ViewBag.SelectedThuongHieu = ThuongHieuID;
            ViewBag.SelectedPhamViLoc = PhamViLoc;
            ViewBag.SelectedBangDieuKhien = BangDieuKhien;
            ViewBag.SelectedLoaiBuiLocDuoc = LoaiBuiLocDuoc;
            ViewBag.SelectedGia = Gia;

            // Hiển thị thông báo nếu không có sản phẩm nào thỏa mãn
            if (!mayLocList.Any())
            {
                ViewBag.NoResultsMessage = "Không có sản phẩm nào thỏa mãn điều kiện lọc.";
            }

            return View(mayLocList);
        }

        public IActionResult TimKiem(string keyword)
        {
            var sanPhams = _context.SanPham
                .Include(sp => sp.HinhAnhSanPham) // Load danh sách hình ảnh
                .Where(sp => sp.TenSanPham.Contains(keyword))
                .ToList();

            ViewBag.Keyword = keyword;
            return View("TimKiem", sanPhams);
        }

    }
}
