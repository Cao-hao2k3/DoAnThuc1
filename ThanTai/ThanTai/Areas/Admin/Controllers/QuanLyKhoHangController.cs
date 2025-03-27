using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
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

        public IActionResult HienThiLichSu(DateTime? fromDate, int? loaiGiaoDich)
        {
            var query = _context.QuanLyKhoHang
                .Include(k => k.SanPham)
                .Include(k => k.NguoiDung)
                .AsQueryable();

            if (fromDate.HasValue)
            {
                DateTime startDate = fromDate.Value.Date;
                DateTime endDate = startDate.AddDays(1).AddTicks(-1);

                query = query.Where(k => k.ThoiGian >= startDate && k.ThoiGian <= endDate);
            }

            if (loaiGiaoDich.HasValue)
            {
                query = query.Where(k => k.LoaiGiaoDich == loaiGiaoDich.Value);
            }

            var data = query.OrderByDescending(k => k.ThoiGian).ToList();

            return View(data);
        }

        //Xử lý nhập file excel

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            var userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserID");

            if (userId == null || !_context.NguoiDung.Any(u => u.ID == userId))
            {
                TempData["Error"] = "Người dùng không hợp lệ!";
                return RedirectToAction(nameof(NhapHang));
            }

            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn một file Excel!";
                return RedirectToAction(nameof(NhapHang));
            }

            var extension = Path.GetExtension(file.FileName);
            if (extension.ToLower() != ".xlsx")
            {
                TempData["Error"] = "Chỉ chấp nhận file Excel có định dạng .xlsx!";
                return RedirectToAction(nameof(NhapHang));
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            TempData["Error"] = "File Excel không chứa dữ liệu!";
                            return RedirectToAction(nameof(NhapHang));
                        }

                        int rowCount = worksheet.Dimension?.Rows ?? 0;
                        int colCount = worksheet.Dimension?.Columns ?? 0;

                  
                        // Kiểm tra tiêu đề cột
                        string col1 = worksheet.Cells[1, 1].Text.Trim();
                        string col2 = worksheet.Cells[1, 2].Text.Trim();
                        string col3 = worksheet.Cells[1, 3].Text.Trim();

                        List<string> titleErrors = new List<string>();

                        if (col1 != "Sản phẩm ID") titleErrors.Add($"Cột A1: '{col1}' → Phải là 'Sản phẩm ID'");
                        if (col2 != "Số lượng") titleErrors.Add($"Cột B1: '{col2}' → Phải là 'Số lượng'");
                        if (col3 != "Ghi chú") titleErrors.Add($"Cột C1: '{col3}' → Phải là 'Ghi chú'");

                        if (titleErrors.Any())
                        {
                            TempData["Error"] = "Lỗi tiêu đề cột trong file Excel:<br/>" + string.Join("<br/>", titleErrors) +
                                                "<br/>Vui lòng sửa lại tiêu đề các cột cho đúng.";
                            return RedirectToAction(nameof(NhapHang));
                        }


                        List<string> errors = new List<string>();

                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                if (worksheet.Cells[row, 1].Value == null || worksheet.Cells[row, 2].Value == null)
                                {
                                    errors.Add($"Lỗi dòng {row}: Thiếu dữ liệu!");
                                    continue;
                                }

                                int sanPhamID = int.Parse(worksheet.Cells[row, 1]?.Value?.ToString() ?? "0");
                                int soLuong = int.Parse(worksheet.Cells[row, 2]?.Value?.ToString() ?? "0");
                                string ghiChu = worksheet.Cells[row, 3]?.Value?.ToString() ?? "";

                                var sanPham = await _context.SanPham.FindAsync(sanPhamID);
                                if (sanPham == null)
                                {
                                    errors.Add($"Lỗi dòng {row}: Sản phẩm ID {sanPhamID} không tồn tại!");
                                    continue;
                                }

                                var quanLyKhoHang = new QuanLyKhoHang
                                {
                                    SanPhamID = sanPhamID,
                                    SoLuong = soLuong,
                                    GhiChu = ghiChu,
                                    LoaiGiaoDich = 1,
                                    ThoiGian = DateTime.Now,
                                    NguoiDungID = userId.Value
                                };

                                _context.QuanLyKhoHang.Add(quanLyKhoHang);
                                sanPham.SoLuong += soLuong;
                                _context.SanPham.Update(sanPham);
                            }
                            catch (FormatException)
                            {
                                errors.Add($"Lỗi dòng {row}: Dữ liệu không hợp lệ (có thể sai kiểu số)!");
                            }
                            catch (Exception rowEx)
                            {
                                errors.Add($"Lỗi dòng {row}: {rowEx.Message}");
                            }
                        }

                        await _context.SaveChangesAsync();

                        if (errors.Any())
                        {
                            TempData["Error"] = string.Join("<br/>", errors);
                        }
                        else
                        {
                            TempData["Success"] = "Nhập dữ liệu từ Excel thành công!";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi nhập file: " + ex.Message;
            }

            return RedirectToAction(nameof(NhapHang));
        }

    }
}
