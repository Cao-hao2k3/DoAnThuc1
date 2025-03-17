using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using ThanTai.Models;

namespace ThanTai.Controllers
{
    public class BanTinController : Controller
    {
        private readonly ThanTaiShopDbContext _context;

        public BanTinController(ThanTaiShopDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var banTins = _context.BanTin.ToList(); // Lấy toàn bộ bài viết từ DB
            return View(banTins);
        }

        public IActionResult ChiTietBanTin(int id)
        {
            var banTin = _context.BanTin.FirstOrDefault(b => b.ID == id);
            if (banTin == null)
            {
                return NotFound(); // Trả về lỗi 404 nếu không tìm thấy bài viết
            }
            return View(banTin);
        }

    }
}
