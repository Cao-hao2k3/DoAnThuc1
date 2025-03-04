using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ThanTai.Models;

namespace ThanTai.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HinhAnhSanPhamController : Controller
    {
        private readonly ThanTaiShopDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HinhAnhSanPhamController(ThanTaiShopDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var images = await _context.HinhAnhSanPham.Include(h => h.SanPham).ToListAsync();
            return View(images);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var hinhAnhSanPham = await _context.HinhAnhSanPham.FindAsync(id);
            if (hinhAnhSanPham == null) return NotFound();

            ViewData["SanPhamID"] = new SelectList(_context.SanPham, "ID", "TenSanPham", hinhAnhSanPham.SanPhamID);
            return View(hinhAnhSanPham);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string oldImage, IFormFile? newImage)
        {
            if (newImage == null) return BadRequest("Bạn cần chọn ảnh mới để thay thế.");
            var hinhAnhSanPham = await _context.HinhAnhSanPham.FindAsync(id);
            if (hinhAnhSanPham == null) return NotFound();

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            var imageList = JsonConvert.DeserializeObject<List<string>>(hinhAnhSanPham.AnhSanPham) ?? new List<string>();

            if (imageList.Contains(oldImage))
            {
                // Xóa ảnh cũ khỏi hệ thống
                string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, oldImage.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                // Lưu ảnh mới
                string newImagePath = await SaveFile(newImage, uploadsFolder);
                int index = imageList.IndexOf(oldImage);
                imageList[index] = newImagePath;
                hinhAnhSanPham.AnhSanPham = JsonConvert.SerializeObject(imageList);

                _context.Update(hinhAnhSanPham);
                await _context.SaveChangesAsync();
            }
            else
            {
                return BadRequest("Ảnh cũ không tồn tại trong danh sách.");
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveFile(IFormFile file, string uploadsFolder)
        {
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return "/uploads/" + uniqueFileName;
        }
    }
}
