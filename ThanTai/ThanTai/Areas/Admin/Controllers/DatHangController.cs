using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThanTai.Models;

namespace ThanTai.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class DatHangController : Controller
    {
        private readonly ThanTaiShopDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DatHangController(ThanTaiShopDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: DatHang
        public async Task<IActionResult> Index()
        {
            // Lấy tất cả đơn hàng và bao gồm các dữ liệu liên quan
            var datHangList = await _context.DatHang.Include(d => d.NguoiDung).Include(d => d.TinhTrang).ToListAsync();

            // Thống kê số lượng đơn hàng theo TinhTrangID
            var tinhTrangStats = datHangList
                .GroupBy(d => d.TinhTrangID)
                .Select(g => new
                {
                    TinhTrangID = g.Key,
                    Count = g.Count()
                }).ToList();

            // Trả về View và truyền cả danh sách đơn hàng và thống kê
            ViewBag.TinhTrangStats = tinhTrangStats;

            return View(datHangList);
        }


        public IActionResult DangChoXuLy()
        {
            var donHangs = _context.DatHang
                .Where(d => d.TinhTrangID == 3) // Lọc đơn hàng có tình trạng là 3
                .ToList();

            return View(donHangs);
        }

        public IActionResult XuLyThanhCong()
        {
            var donHangs = _context.DatHang
                .Where(d => d.TinhTrangID == 1)
                .ToList();

            return View("~/Areas/Admin/Views/DatHang/XuLyThanhCong.cshtml", donHangs);
        }

        public IActionResult DonBiHuy()
        {
            var donHangs = _context.DatHang
                .Where(d => d.TinhTrangID == 7)
                .ToList();

            return View("~/Areas/Admin/Views/DatHang/DonBiHuy.cshtml", donHangs);
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

        // GET: DatHang/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datHang = await _context.DatHang
                .Include(d => d.NguoiDung)
                .Include(d => d.TinhTrang)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (datHang == null)
            {
                return NotFound();
            }

            return View(datHang);
        }

        // GET: DatHang/Create
        public IActionResult Create()
        {
            ViewData["NguoiDungID"] = new SelectList(_context.NguoiDung, "ID", "DiaChi");
            ViewData["TinhTrangID"] = new SelectList(_context.TinhTrang, "ID", "MoTa");
            return View();
        }

        // POST: DatHang/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,NguoiDungID,TinhTrangID,TenNguoiDat,DienThoaiNguoiDat,DiaChiGiaoHang,NgayDatHang,TinhTrangThanhToan")] DatHang datHang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(datHang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NguoiDungID"] = new SelectList(_context.NguoiDung, "ID", "DiaChi", datHang.NguoiDungID);
            ViewData["TinhTrangID"] = new SelectList(_context.TinhTrang, "ID", "MoTa", datHang.TinhTrangID);
            return View(datHang);
        }

        // GET: DatHang/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datHang = await _context.DatHang.FindAsync(id);
            if (datHang == null)
            {
                return NotFound();
            }
            ViewData["TinhTrangID"] = new SelectList(_context.TinhTrang, "ID", "MoTa", datHang.TinhTrangID);
            return View(datHang);
        }

        // POST: DatHang/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,NguoiDungID,TinhTrangID,TenNguoiDat,DienThoaiNguoiDat,DiaChiGiaoHang,NgayDatHang")] DatHang datHang)
        {
            // Kiểm tra session UserID
            int? userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserID");

            // Nếu chưa đăng nhập, chuyển về trang Login
            if (userId == null)
            {
                Console.WriteLine("⚠️ UserID trong session không tồn tại hoặc bị null!");
                return RedirectToAction("Login", "Home");
            }

            if (id != datHang.ID)
            {
                return NotFound();
            }

            datHang.TinhTrangThanhToan = 1;
            if (ModelState.IsValid)
            {
                try
                {
                    if (datHang.TinhTrangThanhToan == 1) // Nếu đơn hàng đã thanh toán thì xử lý xuất kho
                    {
                        // Lấy danh sách chi tiết đơn hàng
                        var chiTietDonHang = _context.DatHangChiTiet.Where(d => d.DatHangID == datHang.ID).ToList();

                        // Kiểm tra tất cả sản phẩm trước khi trừ
                        foreach (var item in chiTietDonHang)
                        {
                            var sanPham = await _context.SanPham.FindAsync(item.SanPhamID);
                            if (sanPham != null)
                            {
                                // Tính toán số lượng tồn kho thực tế
                                var tongSoLuongKho = _context.QuanLyKhoHang
                                    .Where(k => k.SanPhamID == sanPham.ID && k.LoaiGiaoDich == 1) // Tính số lượng nhập kho
                                    .Sum(k => k.SoLuong)
                                    - _context.QuanLyKhoHang
                                    .Where(k => k.SanPhamID == sanPham.ID && k.LoaiGiaoDich == 2) // Trừ số lượng xuất kho
                                    .Sum(k => k.SoLuong);
                                if (tongSoLuongKho < item.SoLuong)
                                {
                                    ModelState.AddModelError("", $"Sản phẩm {sanPham.TenSanPham} không đủ số lượng trong kho.");
                                    return View(datHang); // Báo lỗi và không cập nhật đơn hàng
                                }
                            }
                        }
                   
                        // Nếu tất cả sản phẩm đều đủ số lượng, tiến hành trừ kho
                        foreach (var item in chiTietDonHang)
                        {
                            var sanPham = await _context.SanPham.FindAsync(item.SanPhamID);
                            if (sanPham != null)
                            {
                                // Trừ số lượng trong kho
                                sanPham.SoLuong -= item.SoLuong;
                                
                                // Ghi nhận lịch sử xuất kho
                                _context.QuanLyKhoHang.Add(new QuanLyKhoHang
                                {
                                    SanPhamID = sanPham.ID,
                                    NguoiDungID = userId.Value,
                                    LoaiGiaoDich = 2, // Xuất kho
                                    SoLuong = item.SoLuong,
                                    ThoiGian = DateTime.Now,
                                    GhiChu = $"Xuất kho do đơn hàng #{datHang.ID}"
                                });
                            }
                        }
                    }
                    _context.Update(datHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.DatHang.Any(e => e.ID == datHang.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["TinhTrangID"] = new SelectList(_context.TinhTrang, "ID", "MoTa", datHang.TinhTrangID);
            return View(datHang);
        }


        // GET: DatHang/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datHang = await _context.DatHang
                .Include(d => d.NguoiDung)
                .Include(d => d.TinhTrang)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (datHang == null)
            {
                return NotFound();
            }

            return View(datHang);
        }

        // POST: DatHang/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var datHang = await _context.DatHang.FindAsync(id);
            if (datHang != null)
            {
                _context.DatHang.Remove(datHang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DatHangExists(int id)
        {
            return _context.DatHang.Any(e => e.ID == id);
        }
    }
}
