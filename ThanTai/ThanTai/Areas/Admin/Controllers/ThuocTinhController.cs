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
    public class ThuocTinhController : Controller
    {
        private readonly ThanTaiShopDbContext _context;

        public ThuocTinhController(ThanTaiShopDbContext context)
        {
            _context = context;
        }

        // GET: ThuocTinh
        public async Task<IActionResult> Index()
        {
            var thuocTinhList = await _context.ThuocTinh
                                              .Include(t => t.LoaiSanPham) // Load dữ liệu quan hệ
                                              .ToListAsync();

            return View(thuocTinhList);
        }

        // GET: ThuocTinh/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thuocTinh = await _context.ThuocTinh
                .FirstOrDefaultAsync(m => m.ID == id);
            if (thuocTinh == null)
            {
                return NotFound();
            }

            return View(thuocTinh);
        }

        // GET: ThuocTinh/Create
        public IActionResult Create()
        {
            ViewBag.LoaiSanPhamID = new SelectList(_context.LoaiSanPham, "ID", "Tenloai");
            return View();
        }

        // POST: ThuocTinh/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,LoaiSanPhamID,TenThuocTinh")] ThuocTinh thuocTinh)
        {
            if (thuocTinh.LoaiSanPhamID == 0) // Kiểm tra nếu LoaiSanPhamID chưa được chọn
            {
                ModelState.AddModelError("LoaiSanPhamID", "Bạn chưa chọn loại sản phẩm.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(thuocTinh);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Load lại danh sách LoaiSanPham khi ModelState không hợp lệ
            ViewBag.LoaiSanPhamID = new SelectList(_context.LoaiSanPham, "ID", "Tenloai", thuocTinh.LoaiSanPhamID);
            return View(thuocTinh);
        }

        // GET: ThuocTinh/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thuocTinh = await _context.ThuocTinh.FindAsync(id);
            if (thuocTinh == null)
            {
                return NotFound();
            }

            // Load danh sách Loại Sản Phẩm vào ViewBag
            ViewBag.LoaiSanPhamID = new SelectList(_context.LoaiSanPham, "ID", "Tenloai", thuocTinh.LoaiSanPhamID);

            return View(thuocTinh);
        }

        // POST: ThuocTinh/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,LoaiSanPhamID,TenThuocTinh")] ThuocTinh thuocTinh)
        {
            if (id != thuocTinh.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(thuocTinh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThuocTinhExists(thuocTinh.ID))
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

            // Load lại danh sách LoaiSanPham nếu ModelState không hợp lệ
            ViewBag.LoaiSanPhamID = new SelectList(_context.LoaiSanPham, "ID", "Tenloai", thuocTinh.LoaiSanPhamID);

            return View(thuocTinh);
        }

        // GET: ThuocTinh/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thuocTinh = await _context.ThuocTinh
                .FirstOrDefaultAsync(m => m.ID == id);
            if (thuocTinh == null)
            {
                return NotFound();
            }

            return View(thuocTinh);
        }

        // POST: ThuocTinh/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var thuocTinh = await _context.ThuocTinh.FindAsync(id);
            if (thuocTinh != null)
            {
                _context.ThuocTinh.Remove(thuocTinh);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThuocTinhExists(int id)
        {
            return _context.ThuocTinh.Any(e => e.ID == id);
        }
    }
}
