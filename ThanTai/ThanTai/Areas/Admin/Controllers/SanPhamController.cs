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
    public class SanPhamController : Controller
    {
        private readonly ThanTaiShopDbContext _context;

        public SanPhamController(ThanTaiShopDbContext context)
        {
            _context = context;
        }

        // GET: SanPham
        public async Task<IActionResult> Index()
        {
            var thanTaiShopDbContext = _context.SanPham.Include(s => s.LoaiSanPham).Include(s => s.ThuongHieu);
            return View(await thanTaiShopDbContext.ToListAsync());
        }

        // GET: SanPham/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPham
                .Include(s => s.LoaiSanPham)
                .Include(s => s.ThuongHieu)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // GET: SanPham/Create
        public IActionResult Create()
        {
            ViewData["LoaiSanPhamID"] = new SelectList(_context.LoaiSanPham, "ID", "Tenloai");
            ViewData["ThuongHieuID"] = new SelectList(_context.ThuongHieu, "ID", "TenThuongHieu");
            return View();
        }

        // POST: SanPham/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,LoaiSanPhamID,ThuongHieuID,TenSanPham,DonGia,SoLuong,MoTa,GiamGia,LuotDanhGia,LuotBan,ThongTinThongSo")] SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                sanPham.SoLuong = 0;

                // Tính giá sau khi giảm
                sanPham.GiaSauKhiGiam = sanPham.DonGia - (sanPham.DonGia * (sanPham.GiamGia ?? 0) / 100);

                // Kiểm tra dữ liệu nhập vào trường ThongTinThongSo
                if (string.IsNullOrWhiteSpace(sanPham.ThongTinThongSo))
                {
                    sanPham.ThongTinThongSo = "Không có thông tin."; // Gán giá trị mặc định nếu trống
                }

                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["LoaiSanPhamID"] = new SelectList(_context.LoaiSanPham, "ID", "Tenloai", sanPham.LoaiSanPhamID);
            ViewData["ThuongHieuID"] = new SelectList(_context.ThuongHieu, "ID", "TenThuongHieu", sanPham.ThuongHieuID);
            return View(sanPham);
        }

        // GET: SanPham/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPham.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }
            ViewData["LoaiSanPhamID"] = new SelectList(_context.LoaiSanPham, "ID", "Tenloai", sanPham.LoaiSanPhamID);
            ViewData["ThuongHieuID"] = new SelectList(_context.ThuongHieu, "ID", "TenThuongHieu", sanPham.ThuongHieuID);
            return View(sanPham);
        }

        // POST: SanPham/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,LoaiSanPhamID,ThuongHieuID,TenSanPham,DonGia,SoLuong,MoTa,GiamGia,LuotDanhGia,LuotBan,ThongTinThongSo")] SanPham sanPham)
        {
            if (id != sanPham.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tính toán giá sau khi giảm
                    sanPham.GiaSauKhiGiam = sanPham.DonGia - (sanPham.DonGia * (sanPham.GiamGia ?? 0) / 100);

                    _context.Update(sanPham);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamExists(sanPham.ID))
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

            ViewData["LoaiSanPhamID"] = new SelectList(_context.LoaiSanPham, "ID", "Tenloai", sanPham.LoaiSanPhamID);
            ViewData["ThuongHieuID"] = new SelectList(_context.ThuongHieu, "ID", "TenThuongHieu", sanPham.ThuongHieuID);
            return View(sanPham);
        }


        // GET: SanPham/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPham
                .Include(s => s.LoaiSanPham)
                .Include(s => s.ThuongHieu)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // POST: SanPham/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sanPham = await _context.SanPham.FindAsync(id);
            if (sanPham != null)
            {
                _context.SanPham.Remove(sanPham);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SanPhamExists(int id)
        {
            return _context.SanPham.Any(e => e.ID == id);
        }
    }
}
