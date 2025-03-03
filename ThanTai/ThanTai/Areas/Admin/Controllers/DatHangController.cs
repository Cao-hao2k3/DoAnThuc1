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

        public DatHangController(ThanTaiShopDbContext context)
        {
            _context = context;
        }

        // GET: DatHang
        public async Task<IActionResult> Index()
        {
            var thanTaiShopDbContext = _context.DatHang.Include(d => d.NguoiDung).Include(d => d.TinhTrang);
            return View(await thanTaiShopDbContext.ToListAsync());
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
        public async Task<IActionResult> Create([Bind("ID,NguoiDungID,TinhTrangID,TenNguoiDat,DienThoaiGiaoHang,DiaChiGiaoHang,NgayDatHang,TinhTrangThanhToan")] DatHang datHang)
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
            ViewData["NguoiDungID"] = new SelectList(_context.NguoiDung, "ID", "DiaChi", datHang.NguoiDungID);
            ViewData["TinhTrangID"] = new SelectList(_context.TinhTrang, "ID", "MoTa", datHang.TinhTrangID);
            return View(datHang);
        }

        // POST: DatHang/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,NguoiDungID,TinhTrangID,TenNguoiDat,DienThoaiGiaoHang,DiaChiGiaoHang,NgayDatHang,TinhTrangThanhToan")] DatHang datHang)
        {
            if (id != datHang.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(datHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DatHangExists(datHang.ID))
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
            ViewData["NguoiDungID"] = new SelectList(_context.NguoiDung, "ID", "DiaChi", datHang.NguoiDungID);
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
