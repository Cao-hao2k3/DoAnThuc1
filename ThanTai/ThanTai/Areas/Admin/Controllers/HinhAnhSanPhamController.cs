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

        public IActionResult Create()
        {
            ViewData["SanPhamID"] = new SelectList(_context.SanPham, "ID", "TenSanPham");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HinhAnhSanPham model, List<IFormFile> AnhSanPhamFiles, IFormFile? AnhThongSoFile)
        {
            if (!AnhSanPhamFiles.Any() && AnhThongSoFile == null)
            {
                ModelState.AddModelError("", "Vui lòng chọn ít nhất một ảnh.");
            }

            // Kiểm tra dữ liệu đầu vào
            Console.WriteLine($"SanPhamID: {model.SanPhamID}");
            Console.WriteLine($"Số ảnh sản phẩm: {AnhSanPhamFiles.Count}");
            Console.WriteLine($"Ảnh thông số: {(AnhThongSoFile != null ? AnhThongSoFile.FileName : "Không có")}");

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Lỗi ModelState: {error.ErrorMessage}");
                }

                ViewData["SanPhamID"] = new SelectList(_context.SanPham, "ID", "TenSanPham", model.SanPhamID);
                return View(model);
            }

            // Lưu ảnh sản phẩm
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            List<string> sanPhamImages = await SaveFiles(AnhSanPhamFiles, uploadsFolder);
            model.AnhSanPham = sanPhamImages.Any() ? JsonConvert.SerializeObject(sanPhamImages) : "[]";

            // Lưu ảnh thông số
            if (AnhThongSoFile != null)
            {
                model.AnhThongSo = await SaveFile(AnhThongSoFile, uploadsFolder);
            }

            try
            {
                _context.Add(model);
                int result = await _context.SaveChangesAsync();
                Console.WriteLine($"Kết quả SaveChangesAsync: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lưu vào database: {ex.Message}");
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

            // Xóa file ảnh khỏi thư mục "uploads"
            if (!string.IsNullOrEmpty(hinhAnhSanPham.AnhSanPham))
            {
                List<string> imagePaths = JsonConvert.DeserializeObject<List<string>>(hinhAnhSanPham.AnhSanPham);
                foreach (var imagePath in imagePaths)
                {
                    string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
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

            // Xóa khỏi database
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
        public async Task<IActionResult> Edit(int id, HinhAnhSanPham model, List<IFormFile> AnhSanPhamFiles, IFormFile? AnhThongSoFile)
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

            // Lưu ảnh mới nếu có
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            List<string> sanPhamImages = AnhSanPhamFiles.Any() ? await SaveFiles(AnhSanPhamFiles, uploadsFolder) : JsonConvert.DeserializeObject<List<string>>(existingImage.AnhSanPham);

            // Nếu có ảnh thông số mới thì thay thế
            string anhThongSoPath = existingImage.AnhThongSo;
            if (AnhThongSoFile != null)
            {
                anhThongSoPath = await SaveFile(AnhThongSoFile, uploadsFolder);
            }

            // Cập nhật dữ liệu
            existingImage.SanPhamID = model.SanPhamID;
            existingImage.AnhSanPham = JsonConvert.SerializeObject(sanPhamImages);
            existingImage.AnhThongSo = anhThongSoPath;

            try
            {
                _context.Update(existingImage);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.HinhAnhSanPham.Any(e => e.ID == id))
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

    }
}
