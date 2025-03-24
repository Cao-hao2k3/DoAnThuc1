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
using ThanTai.Libraries;
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

        public IActionResult Create()
        {
            ViewData["SanPhamID"] = new SelectList(_context.SanPham, "ID", "TenSanPham");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HinhAnhSanPham model, List<IFormFile> AnhSanPhamFiles, IFormFile? AnhThongSoFile, string? Video)
        {
            if (!AnhSanPhamFiles.Any() && AnhThongSoFile == null && string.IsNullOrEmpty(Video))
            {
                ModelState.AddModelError("", "Vui lòng chọn ít nhất một ảnh hoặc nhập video.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["SanPhamID"] = new SelectList(_context.SanPham, "ID", "TenSanPham", model.SanPhamID);
                return View(model);
            }

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            List<string> sanPhamImages = await SaveFiles(AnhSanPhamFiles, uploadsFolder);

            model.AnhSanPham = JsonConvert.SerializeObject(sanPhamImages);

            if (AnhThongSoFile != null)
            {
                model.AnhThongSo = await SaveFile(AnhThongSoFile, uploadsFolder);
            }

            // Chuyển đổi Video thành URL nhúng
            model.VideoReview = YouTubeHelper.GetYouTubeEmbedUrl(Video);

            _context.Add(model);
            await _context.SaveChangesAsync();

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

        private async Task<List<string>> SaveFiles(List<IFormFile> files, string uploadsFolder)
        {
            List<string> savedFiles = new List<string>();

            foreach (var file in files)
            {
                string filePath = await SaveFile(file, uploadsFolder);
                savedFiles.Add(filePath);
            }

            return savedFiles;
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hinhAnhSanPham = await _context.HinhAnhSanPham
                .Include(h => h.SanPham)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (hinhAnhSanPham == null)
            {
                return NotFound();
            }

            return View(hinhAnhSanPham);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hinhAnhSanPham = await _context.HinhAnhSanPham.FindAsync(id);
            if (hinhAnhSanPham == null)
            {
                return NotFound();
            }

            List<string> imagePaths = string.IsNullOrEmpty(hinhAnhSanPham.AnhSanPham)
                ? new List<string>()
                : JsonConvert.DeserializeObject<List<string>>(hinhAnhSanPham.AnhSanPham) ?? new List<string>();

            foreach (var path in imagePaths)
            {
                string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, path.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            if (!string.IsNullOrEmpty(hinhAnhSanPham.AnhThongSo))
            {
                string tsPath = Path.Combine(_webHostEnvironment.WebRootPath, hinhAnhSanPham.AnhThongSo.TrimStart('/'));
                if (System.IO.File.Exists(tsPath))
                {
                    System.IO.File.Delete(tsPath);
                }
            }

            _context.HinhAnhSanPham.Remove(hinhAnhSanPham);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hinhAnhSanPham = await _context.HinhAnhSanPham
                .Include(h => h.SanPham)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (hinhAnhSanPham == null)
            {
                return NotFound();
            }

            ViewData["SanPhamID"] = new SelectList(_context.SanPham, "ID", "TenSanPham", hinhAnhSanPham.SanPhamID);
            return View(hinhAnhSanPham);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HinhAnhSanPham model, IFormFile? AnhThongSoFile, string? Video)
        {
            if (id != model.ID)
            {
                return NotFound();
            }

            var existingImage = await _context.HinhAnhSanPham.FindAsync(id);
            if (existingImage == null)
            {
                return NotFound();
            }

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

            // Lấy danh sách ảnh cũ từ JSON
            List<string> sanPhamImages = JsonConvert.DeserializeObject<List<string>>(existingImage.AnhSanPham) ?? new List<string>();

            // Xử lý ảnh mới được tải lên
            foreach (var file in Request.Form.Files)
            {
                string key = file.Name; // key sẽ có dạng "NewImages[1]", "NewImages[2]"
                if (key.StartsWith("NewImages[") && key.EndsWith("]"))
                {
                    int startIndex = key.IndexOf("[") + 1;
                    int endIndex = key.IndexOf("]");
                    int index = int.Parse(key.Substring(startIndex, endIndex - startIndex));

                    string newImagePath = await SaveFile(file, uploadsFolder);

                    if (index < sanPhamImages.Count)
                    {
                        sanPhamImages[index] = newImagePath;
                    }
                }
            }

            existingImage.AnhSanPham = JsonConvert.SerializeObject(sanPhamImages);

            if (AnhThongSoFile != null)
            {
                existingImage.AnhThongSo = await SaveFile(AnhThongSoFile, uploadsFolder);
            }

            // Cập nhật Video nếu có thay đổi
            if (!string.IsNullOrEmpty(Video))
            {
                existingImage.VideoReview = YouTubeHelper.GetYouTubeEmbedUrl(Video);
            }

            _context.Update(existingImage);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}