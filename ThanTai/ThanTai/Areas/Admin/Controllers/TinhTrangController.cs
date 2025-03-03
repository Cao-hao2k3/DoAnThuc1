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
    public class TinhTrangController : Controller
    {
        private readonly ThanTaiShopDbContext _context;

        public TinhTrangController(ThanTaiShopDbContext context)
        {
            _context = context;
        }

        // GET: TinhTrang
        public async Task<IActionResult> Index()
        {
            return View(await _context.TinhTrang.ToListAsync());
        }

        // GET: TinhTrang/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tinhTrang = await _context.TinhTrang
                .FirstOrDefaultAsync(m => m.ID == id);
            if (tinhTrang == null)
            {
                return NotFound();
            }

            return View(tinhTrang);
        }

        // GET: TinhTrang/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TinhTrang/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,MoTa")] TinhTrang tinhTrang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tinhTrang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tinhTrang);
        }

        // GET: TinhTrang/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tinhTrang = await _context.TinhTrang.FindAsync(id);
            if (tinhTrang == null)
            {
                return NotFound();
            }
            return View(tinhTrang);
        }

        // POST: TinhTrang/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,MoTa")] TinhTrang tinhTrang)
        {
            if (id != tinhTrang.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tinhTrang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TinhTrangExists(tinhTrang.ID))
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
            return View(tinhTrang);
        }

        // GET: TinhTrang/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tinhTrang = await _context.TinhTrang
                .FirstOrDefaultAsync(m => m.ID == id);
            if (tinhTrang == null)
            {
                return NotFound();
            }

            return View(tinhTrang);
        }

        // POST: TinhTrang/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tinhTrang = await _context.TinhTrang.FindAsync(id);
            if (tinhTrang != null)
            {
                _context.TinhTrang.Remove(tinhTrang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TinhTrangExists(int id)
        {
            return _context.TinhTrang.Any(e => e.ID == id);
        }
    }
}
