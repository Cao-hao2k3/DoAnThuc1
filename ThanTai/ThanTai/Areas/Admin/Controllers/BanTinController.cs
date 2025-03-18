using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SlugGenerator;
using ThanTai.Models;

namespace ThanTai.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BanTinController : Controller
    {
        private readonly ThanTaiShopDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public BanTinController(ThanTaiShopDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Admin/BanTin
        public async Task<IActionResult> Index()
        {
            return View(await _context.BanTin.ToListAsync());
        }

        // GET: Admin/BanTin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banTin = await _context.BanTin
                .FirstOrDefaultAsync(m => m.ID == id);
            if (banTin == null)
            {
                return NotFound();
            }

            return View(banTin);
        }

        // GET: Admin/BanTin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/BanTin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,Slug,DuLieuHinhAnhImage,DuLieuHinhAnhBanner,Category,Content,CreatedAt")] BanTin banTin)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string folder = "/uploads/";

                // Lưu hình ảnh chính (Image)
                if (banTin.DuLieuHinhAnhImage != null)
                {
                    string fileExtension = Path.GetExtension(banTin.DuLieuHinhAnhImage.FileName).ToLower();
                    string fileNameSluged = banTin.Title.GenerateSlug();
                    string filePath = Path.Combine(wwwRootPath + folder, fileNameSluged + fileExtension);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await banTin.DuLieuHinhAnhImage.CopyToAsync(fileStream);
                    }
                    banTin.Image = folder + fileNameSluged + fileExtension;
                }

                // Lưu ảnh banner (Banner)
                if (banTin.DuLieuHinhAnhBanner != null)
                {
                    string fileExtension = Path.GetExtension(banTin.DuLieuHinhAnhBanner.FileName).ToLower();
                    string fileNameSluged = banTin.Title.GenerateSlug() + "_banner";
                    string filePath = Path.Combine(wwwRootPath + folder, fileNameSluged + fileExtension);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await banTin.DuLieuHinhAnhBanner.CopyToAsync(fileStream);
                    }
                    banTin.Banner = folder + fileNameSluged + fileExtension;
                }

                // Lưu dữ liệu vào database
                _context.Add(banTin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(banTin);
        }


        // GET: Admin/BanTin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banTin = await _context.BanTin.FindAsync(id);
            if (banTin == null)
            {
                return NotFound();
            }
            return View(banTin);
        }

        // POST: Admin/BanTin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,Slug,DuLieuHinhAnhImage,Image,DuLieuHinhAnhBanner,Banner,Category,Content,CreatedAt")] BanTin_ChinhSua banTin)
        {
            if (id != banTin.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var banTinCu = await _context.BanTin.AsNoTracking().FirstOrDefaultAsync(x => x.ID == id);
                    if (banTinCu == null) return NotFound();

                    string imagePath = banTinCu.Image; // Giữ ảnh cũ nếu không có ảnh mới
                    string bannerPath = banTinCu.Banner;
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string uploadFolder = Path.Combine(wwwRootPath, "uploads");

                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    // Xử lý ảnh đại diện
                    if (banTin.DuLieuHinhAnhImage != null)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(banTin.DuLieuHinhAnhImage.FileName) + Path.GetExtension(banTin.DuLieuHinhAnhImage.FileName);
                        string imageFilePath = Path.Combine(uploadFolder, fileName);

                        if (!string.IsNullOrEmpty(banTinCu.Image))
                        {
                            string oldImagePath = Path.Combine(uploadFolder, banTinCu.Image);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        using (var fileStream = new FileStream(imageFilePath, FileMode.Create))
                        {
                            await banTin.DuLieuHinhAnhImage.CopyToAsync(fileStream);
                        }
                        imagePath = fileName;
                    }

                    // Xử lý ảnh banner
                    if (banTin.DuLieuHinhAnhBanner != null)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(banTin.DuLieuHinhAnhBanner.FileName) + Path.GetExtension(banTin.DuLieuHinhAnhBanner.FileName);
                        string bannerFilePath = Path.Combine(uploadFolder, fileName);

                        if (!string.IsNullOrEmpty(banTinCu.Banner))
                        {
                            string oldBannerPath = Path.Combine(uploadFolder, banTinCu.Banner);
                            if (System.IO.File.Exists(oldBannerPath))
                            {
                                System.IO.File.Delete(oldBannerPath);
                            }
                        }

                        using (var fileStream = new FileStream(bannerFilePath, FileMode.Create))
                        {
                            await banTin.DuLieuHinhAnhBanner.CopyToAsync(fileStream);
                        }
                        bannerPath = fileName;
                    }

                    // Cập nhật dữ liệu
                    var b = await _context.BanTin.FindAsync(id);
                    if (b == null) return NotFound();

                    b.Title = banTin.Title;
                    b.Slug = banTin.Slug;
                    b.Category = banTin.Category;
                    b.Content = banTin.Content;
                    b.Image = imagePath;
                    b.Banner = bannerPath;

                    _context.Update(b);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BanTinExists(banTin.ID))
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
            return View(banTin);
        }

        // GET: Admin/BanTin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banTin = await _context.BanTin
                .FirstOrDefaultAsync(m => m.ID == id);
            if (banTin == null)
            {
                return NotFound();
            }

            return View(banTin);
        }

        // POST: Admin/BanTin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var banTin = await _context.BanTin.FindAsync(id);
            if (banTin != null)
            {
                _context.BanTin.Remove(banTin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BanTinExists(int id)
        {
            return _context.BanTin.Any(e => e.ID == id);
        }
    }
}
