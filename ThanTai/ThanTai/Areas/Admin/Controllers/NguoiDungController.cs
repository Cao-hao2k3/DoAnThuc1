using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SlugGenerator;
using ThanTai.Models;
using BC = BCrypt.Net.BCrypt;

namespace ThanTai.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class NguoiDungController : Controller
    {
        private readonly ThanTaiShopDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public NguoiDungController(ThanTaiShopDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: NguoiDung
        public async Task<IActionResult> Index()
        {
            return View(await _context.NguoiDung.ToListAsync());
        }

        // GET: NguoiDung/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDung
                .FirstOrDefaultAsync(m => m.ID == id);
            if (nguoiDung == null)
            {
                return NotFound();
            }

            return View(nguoiDung);
        }

        // GET: NguoiDung/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NguoiDung/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,HoVaTen,Email,DienThoai,DiaChi,TenDangNhap,MatKhau,XacNhanMatKhau,Quyen,DuLieuHinhAnh")] NguoiDung nguoiDung)
        {
            if (ModelState.IsValid)
            {
                string path = "";

                // Nếu hình ảnh không bỏ trống thì upload
                if (nguoiDung.DuLieuHinhAnh != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string folder = "/uploads/";
                    string fileExtension = Path.GetExtension(nguoiDung.DuLieuHinhAnh.FileName).ToLower();
                    string fileName = nguoiDung.TenDangNhap;
                    string fileNameSluged = fileName.GenerateSlug();
                    path = fileNameSluged + fileExtension;
                    string physicalPath = Path.Combine(wwwRootPath + folder, fileNameSluged + fileExtension);
                    using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                    {
                        await nguoiDung.DuLieuHinhAnh.CopyToAsync(fileStream);
                    }
                }

                // Cập nhật đường dẫn vào CSDL
                nguoiDung.Anh = path ?? null;

                nguoiDung.MatKhau = BC.HashPassword(nguoiDung.MatKhau);
                nguoiDung.XacNhanMatKhau = nguoiDung.MatKhau;


                _context.Add(nguoiDung);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nguoiDung);
        }

        // GET: NguoiDung/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDung.FindAsync(id);
            if (nguoiDung == null)
            {
                return NotFound();
            }
            return View(new NguoiDung_ChinhSua(nguoiDung));
        }

        // POST: NguoiDung/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,HoVaTen,Email,DienThoai,DiaChi,TenDangNhap,MatKhauMoi,XacNhanMatKhau,Quyen,DuLieuHinhAnh,Anh")] NguoiDung_ChinhSua nguoiDung)
        {
            if (id != nguoiDung.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var nguoiDungCu = await _context.NguoiDung.AsNoTracking().FirstOrDefaultAsync(x => x.ID == id);
                    if (nguoiDungCu == null) return NotFound();

                    string path = nguoiDungCu.Anh; // Giữ ảnh cũ nếu không có ảnh mới

                    if (nguoiDung.DuLieuHinhAnh != null)
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string folder = Path.Combine(wwwRootPath, "uploads");

                        // Tạo thư mục nếu chưa tồn tại
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }

                        string fileExtension = Path.GetExtension(nguoiDung.DuLieuHinhAnh.FileName).ToLower();
                        string fileName = nguoiDung.HoVaTen.GenerateSlug() + fileExtension;
                        string physicalPath = Path.Combine(folder, fileName);

                        // Xóa ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(nguoiDungCu.Anh))
                        {
                            string oldImagePath = Path.Combine(wwwRootPath, "uploads", nguoiDungCu.Anh);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Lưu ảnh mới
                        using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                        {
                            await nguoiDung.DuLieuHinhAnh.CopyToAsync(fileStream);
                        }

                        path = fileName; // Gán ảnh mới
                    }

                    // Cập nhật dữ liệu vào database
                    var n = await _context.NguoiDung.FindAsync(id);
                    if (n == null) return NotFound();

                    n.HoVaTen = nguoiDung.HoVaTen;
                    n.Email = nguoiDung.Email;
                    n.DienThoai = nguoiDung.DienThoai;
                    n.DiaChi = nguoiDung.DiaChi;
                    n.TenDangNhap = nguoiDung.TenDangNhap;
                    n.Quyen = nguoiDung.Quyen;
                    n.Anh = path;

                    // Giữ nguyên mật khẩu nếu không nhập mới
                    if (!string.IsNullOrEmpty(nguoiDung.MatKhauMoi))
                    {
                        n.MatKhau = BC.HashPassword(nguoiDung.MatKhauMoi);
                        n.XacNhanMatKhau = n.MatKhau;
                    }

                    _context.Update(n);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NguoiDungExists(nguoiDung.ID))
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
            return View(nguoiDung);
        }



        // GET: NguoiDung/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDung
                .FirstOrDefaultAsync(m => m.ID == id);
            if (nguoiDung == null)
            {
                return NotFound();
            }

            return View(nguoiDung);
        }

        // POST: NguoiDung/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nguoiDung = await _context.NguoiDung.FindAsync(id);
            if (nguoiDung != null)
            {
                // Xóa hình ảnh (nếu có)
                if (!string.IsNullOrEmpty(nguoiDung.Anh))
                {
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "uploads", nguoiDung.Anh);
                    if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);
                }

                _context.NguoiDung.Remove(nguoiDung);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NguoiDungExists(int id)
        {
            return _context.NguoiDung.Any(e => e.ID == id);
        }
    }
}
