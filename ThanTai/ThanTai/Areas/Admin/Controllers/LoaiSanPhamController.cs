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

    public class LoaiSanPhamController : Controller
    {
        private readonly ThanTaiShopDbContext _context;

        public LoaiSanPhamController(ThanTaiShopDbContext context)
        {
            _context = context;
        }

        // GET: LoaiSanPham
        public async Task<IActionResult> Index()
        {
            var thanTaiShopDbContext = _context.LoaiSanPham.Include(l => l.ParentCategory);
            return View(await thanTaiShopDbContext.ToListAsync());
        }

        // GET: LoaiSanPham/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiSanPham = await _context.LoaiSanPham
                .Include(l => l.ParentCategory)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (loaiSanPham == null)
            {
                return NotFound();
            }

            return View(loaiSanPham);
        }

        // GET: LoaiSanPham/Create
        public IActionResult Create()
        {
            ViewData["ParentID"] = new SelectList(_context.LoaiSanPham, "ID", "Tenloai");
            return View();
        }

        // POST: LoaiSanPham/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Tenloai,ParentID")] LoaiSanPham loaiSanPham)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra nếu ParentID không hợp lệ (ví dụ: 0 hoặc chuỗi rỗng), thì gán null
                loaiSanPham.ParentID = loaiSanPham.ParentID == 0 ? null : loaiSanPham.ParentID;

                _context.Add(loaiSanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Load lại danh sách ParentID nếu có lỗi
            ViewData["ParentID"] = new SelectList(_context.LoaiSanPham, "ID", "Tenloai", loaiSanPham.ParentID);
            return View(loaiSanPham);
        }

        // GET: LoaiSanPham/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiSanPham = await _context.LoaiSanPham.FindAsync(id);
            if (loaiSanPham == null)
            {
                return NotFound();
            }
            ViewData["ParentID"] = new SelectList(_context.LoaiSanPham, "ID", "Tenloai", loaiSanPham.ParentID);
            return View(loaiSanPham);
        }

        // POST: LoaiSanPham/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Tenloai,ParentID")] LoaiSanPham loaiSanPham)
        {
            if (id != loaiSanPham.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loaiSanPham);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaiSanPhamExists(loaiSanPham.ID))
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
            ViewData["ParentID"] = new SelectList(_context.LoaiSanPham, "ID", "Tenloai", loaiSanPham.ParentID);
            return View(loaiSanPham);
        }

        // GET: LoaiSanPham/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiSanPham = await _context.LoaiSanPham
                .Include(l => l.ParentCategory)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (loaiSanPham == null)
            {
                return NotFound();
            }

            return View(loaiSanPham);
        }

        // POST: LoaiSanPham/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var loaiSanPham = await _context.LoaiSanPham
                    .Include(l => l.SubCategories) // Load danh sách sản phẩm con
                    .FirstOrDefaultAsync(m => m.ID == id);

                if (loaiSanPham == null)
                {
                    return NotFound();
                }

                // Kiểm tra nếu loại sản phẩm có sản phẩm con thì không cho xóa
                if (loaiSanPham.SubCategories != null && loaiSanPham.SubCategories.Any())
                {
                    TempData["ErrorMessage"] = "Không thể xóa loại sản phẩm này vì nó đang có sản phẩm con.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                _context.LoaiSanPham.Remove(loaiSanPham);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa loại sản phẩm thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa. Vui lòng thử lại.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool LoaiSanPhamExists(int id)
        {
            return _context.LoaiSanPham.Any(e => e.ID == id);
        }
    }
}
