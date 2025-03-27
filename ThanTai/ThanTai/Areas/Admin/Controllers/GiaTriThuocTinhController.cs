using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThanTai.Models;
using ThanTai.Models.ViewModels;

namespace ThanTai.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class GiaTriThuocTinhController : Controller
    {
        private readonly ThanTaiShopDbContext _context;

        public GiaTriThuocTinhController(ThanTaiShopDbContext context)
        {
            _context = context;
        }

        // GET: GiaTriThuocTinh
        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.GiaTriThuocTinh
                .Include(g => g.SanPham)
                .Include(g => g.ThuocTinh)
                .Select(g => new GiaTriThuocTinhViewModel
                {
                    ID = g.ID,
                    TenSanPham = g.SanPham.TenSanPham,
                    TenThuocTinh = g.ThuocTinh.TenThuocTinh,
                    GiaTri = g.GiaTri
                })
                .ToListAsync();

            return View(danhSach);
        }

        // GET: GiaTriThuocTinh/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giaTriThuocTinh = await _context.GiaTriThuocTinh
                .Include(g => g.SanPham)
                .Include(g => g.ThuocTinh)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (giaTriThuocTinh == null)
            {
                return NotFound();
            }

            return View(giaTriThuocTinh);
        }

        // GET: GiaTriThuocTinh/Create
        public IActionResult Create()
        {
            ViewData["SanPhamID"] = new SelectList(_context.SanPham, "ID", "TenSanPham");
            ViewData["ThuocTinhID"] = new SelectList(_context.ThuocTinh, "ID", "TenThuocTinh");
            ViewBag.LoaiSanPham = _context.LoaiSanPham.Select(l => new { l.ID, l.Tenloai }).ToList();
            return View();

        }

        [HttpGet]
        public JsonResult GetThuocTinhByLoai(int loaiSanPhamId)
        {
            try
            {
                var thuocTinhs = _context.ThuocTinh
                    .Where(t => t.LoaiSanPhamID == loaiSanPhamId)
                    .Select(t => new { id = t.ID, tenThuocTinh = t.TenThuocTinh })
                    .ToList();

                if (!thuocTinhs.Any())
                {
                    return Json(new { message = "Không có thuộc tính nào cho loại sản phẩm này." });
                }

                return Json(thuocTinhs);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Lỗi server: " + ex.Message });
            }
        }

        // POST: GiaTriThuocTinh/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,SanPhamID,ThuocTinhID,GiaTri")] GiaTriThuocTinh giaTriThuocTinh)
        {
            if (ModelState.IsValid)
            {
                _context.Add(giaTriThuocTinh);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SanPhamID"] = new SelectList(_context.SanPham, "ID", "TenSanPham", giaTriThuocTinh.SanPhamID);
            ViewData["ThuocTinhID"] = new SelectList(_context.ThuocTinh, "ID", "TenThuocTinh", giaTriThuocTinh.ThuocTinhID);
            return View(giaTriThuocTinh);
        }

        // GET: GiaTriThuocTinh/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giaTriThuocTinh = await _context.GiaTriThuocTinh.FindAsync(id);
            if (giaTriThuocTinh == null)
            {
                return NotFound();
            }
            ViewData["SanPhamID"] = new SelectList(_context.SanPham, "ID", "TenSanPham", giaTriThuocTinh.SanPhamID);
            ViewData["ThuocTinhID"] = new SelectList(_context.ThuocTinh, "ID", "TenThuocTinh", giaTriThuocTinh.ThuocTinhID);
            return View(giaTriThuocTinh);
        }

        // POST: GiaTriThuocTinh/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,SanPhamID,ThuocTinhID,GiaTri")] GiaTriThuocTinh giaTriThuocTinh)
        {
            if (id != giaTriThuocTinh.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(giaTriThuocTinh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GiaTriThuocTinhExists(giaTriThuocTinh.ID))
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
            ViewData["SanPhamID"] = new SelectList(_context.SanPham, "ID", "TenSanPham", giaTriThuocTinh.SanPhamID);
            ViewData["ThuocTinhID"] = new SelectList(_context.ThuocTinh, "ID", "TenThuocTinh", giaTriThuocTinh.ThuocTinhID);
            return View(giaTriThuocTinh);
        }

        // GET: GiaTriThuocTinh/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giaTriThuocTinh = await _context.GiaTriThuocTinh
                .Include(g => g.SanPham)
                .Include(g => g.ThuocTinh)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (giaTriThuocTinh == null)
            {
                return NotFound();
            }

            return View(giaTriThuocTinh);
        }

        // POST: GiaTriThuocTinh/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var giaTriThuocTinh = await _context.GiaTriThuocTinh.FindAsync(id);
            if (giaTriThuocTinh != null)
            {
                _context.GiaTriThuocTinh.Remove(giaTriThuocTinh);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GiaTriThuocTinhExists(int id)
        {
            return _context.GiaTriThuocTinh.Any(e => e.ID == id);
        }
    }
}
