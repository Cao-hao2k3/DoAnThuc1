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
    public class ThuongHieuController : Controller
    {
        private readonly ThanTaiShopDbContext _context;

        public ThuongHieuController(ThanTaiShopDbContext context)
        {
            _context = context;
        }

        // GET: ThuongHieu
        public async Task<IActionResult> Index()
        {
            return View(await _context.ThuongHieu.ToListAsync());
        }

        // GET: ThuongHieu/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thuongHieu = await _context.ThuongHieu
                .FirstOrDefaultAsync(m => m.ID == id);
            if (thuongHieu == null)
            {
                return NotFound();
            }

            return View(thuongHieu);
        }

        // GET: ThuongHieu/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ThuongHieu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,TenThuongHieu")] ThuongHieu thuongHieu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(thuongHieu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(modelError.ErrorMessage);
            }
            return View(thuongHieu);
        }

        // GET: ThuongHieu/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thuongHieu = await _context.ThuongHieu.FindAsync(id);
            if (thuongHieu == null)
            {
                return NotFound();
            }
            return View(thuongHieu);
        }

        // POST: ThuongHieu/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,TenThuongHieu")] ThuongHieu thuongHieu)
        {
            if (id != thuongHieu.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(thuongHieu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThuongHieuExists(thuongHieu.ID))
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
            foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(modelError.ErrorMessage);
            }
            return View(thuongHieu);
        }

        // GET: ThuongHieu/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thuongHieu = await _context.ThuongHieu
                .FirstOrDefaultAsync(m => m.ID == id);
            if (thuongHieu == null)
            {
                return NotFound();
            }

            return View(thuongHieu);
        }

        // POST: ThuongHieu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var thuongHieu = await _context.ThuongHieu.FindAsync(id);
            if (thuongHieu != null)
            {
                _context.ThuongHieu.Remove(thuongHieu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThuongHieuExists(int id)
        {
            return _context.ThuongHieu.Any(e => e.ID == id);
        }
    }
}
