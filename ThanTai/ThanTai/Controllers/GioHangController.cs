using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using ThanTai.Models;

namespace ThanTai.Controllers
{
    public class GioHangController : Controller
    {
        private readonly ThanTaiShopDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GioHangController(ThanTaiShopDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult GioHangRong()
        {
            return View();
        }

        public IActionResult Index()
        {
            int? userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var gioHang = _context.GioHang
                .Where(g => g.NguoiDungID == userId)
                .Include(g => g.SanPham)
                .ThenInclude(s => s.HinhAnhSanPham)
                .ToList();

            // Nếu giỏ hàng không có sản phẩm thì trả về view "GioHangRong"
            if (gioHang == null || gioHang.Count == 0)
            {
                return View("GioHangRong");
            }

            // Xử lý hình ảnh sản phẩm
            foreach (var item in gioHang)
            {
                if (item.SanPham?.HinhAnhSanPham != null)
                {
                    var firstImage = GetFirstImageFromJson(item.SanPham.HinhAnhSanPham.FirstOrDefault()?.AnhSanPham);
                    item.SanPham.HinhAnhSanPham.FirstOrDefault().AnhSanPham = firstImage;
                }
            }

            return View(gioHang);
        }


        private string GetFirstImageFromJson(string jsonImages)
        {
            if (!string.IsNullOrEmpty(jsonImages))
            {
                try
                {
                    var images = JsonConvert.DeserializeObject<List<string>>(jsonImages);
                    return images?.FirstOrDefault() ?? "/uploads/no-image.jpg";
                }
                catch
                {
                    return "/uploads/no-image.jpg";
                }
            }
            return "/uploads/no-image.jpg";
        }

        [HttpPost]
        public IActionResult CapNhatSoLuong(int gioHangId, int thayDoi)
        {
            var gioHang = _context.GioHang.FirstOrDefault(g => g.ID == gioHangId);
            if (gioHang != null)
            {
                gioHang.SoLuong += thayDoi;
                if (gioHang.SoLuong <= 0)
                {
                    _context.GioHang.Remove(gioHang);
                }
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult XoaSanPham(int gioHangId)
        {
            var gioHang = _context.GioHang.FirstOrDefault(g => g.ID == gioHangId);
            if (gioHang != null)
            {
                _context.GioHang.Remove(gioHang);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DatHang(string tenNguoiDat, string dienThoaiNguoiDat, string diaChiGiaoHang, int hinhThucGiaoHang, int paymentMethod, string[] sanPhamIDs, int[] soLuongs, decimal[] donGias, string? otherName, string? otherPhone)
        {
            int? userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Tạo đối tượng DatHang
            var datHang = new DatHang
            {
                NguoiDungID = userId.Value,
                TinhTrangID = 3, // Đang xử lý
                TenNguoiDat = tenNguoiDat,
                DienThoaiNguoiDat = dienThoaiNguoiDat,
                DiaChiGiaoHang = diaChiGiaoHang,
                NgayDatHang = DateTime.Now,
                TinhTrangThanhToan = 1, // 1 là chưa thanh toán 2 là đã thanh toán
                HinhThucGiaoHang = hinhThucGiaoHang,
                TenNguoiNhanHangKhac = otherName,
                SoDienThoaiNguoiNhanKhac = otherPhone,
                HinhThucThanhToan = paymentMethod
            };

            _context.DatHang.Add(datHang);
            _context.SaveChanges();

            // Tạo danh sách DatHangChiTiet
            for (int i = 0; i < sanPhamIDs.Length; i++)
            {
                var chiTiet = new DatHangChiTiet
                {
                    DatHangID = datHang.ID,
                    SanPhamID = int.Parse(sanPhamIDs[i]),
                    SoLuong = (short)soLuongs[i],
                    DonGia = donGias[i],
                    TongTien = soLuongs[i] * donGias[i]
                };
                _context.DatHangChiTiet.Add(chiTiet);
            }
            _context.SaveChanges();

            // Xóa giỏ hàng sau khi đặt hàng thành công
            var gioHang = _context.GioHang.Where(g => g.NguoiDungID == userId).ToList();
            _context.GioHang.RemoveRange(gioHang);
            _context.SaveChanges();

            return RedirectToAction("DatHangThanhCong");
        }

        public IActionResult DatHangThanhCong()
        {
            return View();
        }
    }
}
