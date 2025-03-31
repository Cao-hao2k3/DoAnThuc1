 using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using ThanTai.Models;
using WebBanHang.Services.VNPAY;
using WebBanHang.Models.VNPAY;
using System.Dynamic;
using System.Web;
using System.Globalization;
using System.Text;
using Newtonsoft.Json.Linq;
using WebBanHang.Logic;

namespace ThanTai.Controllers
{
    public class GioHangController : Controller
    {
        private readonly ThanTaiShopDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IVnPayService _vnPayService;
        private readonly IMailLogic _mailLogic;
        public GioHangController(ThanTaiShopDbContext context, IHttpContextAccessor httpContextAccessor, IVnPayService vnPayService, IMailLogic mailLogic)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _vnPayService = vnPayService;
            _mailLogic = mailLogic;
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
        public async Task<IActionResult> DatHang(string tenNguoiDat, string dienThoaiNguoiDat, string diaChiGiaoHang, int hinhThucGiaoHang, int paymentMethod, string[] sanPhamIDs, int[] soLuongs, decimal[] donGias, string? otherName, string? otherPhone)
        {
            int? userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Home");
            }

            //  Truy vấn email từ database
            var nguoiDung = await _context.NguoiDung
                .Where(nd => nd.ID == userId.Value)
                .Select(nd => new { nd.Email })
                .FirstOrDefaultAsync();

            if (nguoiDung == null || string.IsNullOrEmpty(nguoiDung.Email))
            {
                TempData["ThongBaoLoi"] = "Không tìm thấy email của người dùng!";
                return RedirectToAction("DatHangThatBai", "GioHang");
            }

            if (paymentMethod == 3) // Thanh toán VNPAY
            {
                var paymentInfo = new PaymentInformationModel
                {
                    Name = RemoveDiacritics(tenNguoiDat),
                    Amount = (double)donGias.Zip(soLuongs, (gia, sl) => gia * sl).Sum(),
                    OrderDescription = "Thanh toán qua VNPAY",
                    OrderType = "billpayment"
                };

                // Lưu thông tin đặt hàng vào Session
                var orderData = new
                {
                    TenNguoiDat = tenNguoiDat,
                    DienThoaiNguoiDat = dienThoaiNguoiDat,
                    DiaChiGiaoHang = diaChiGiaoHang,
                    HinhThucGiaoHang = hinhThucGiaoHang,
                    PaymentMethod = paymentMethod,
                    EmailNguoiDat = nguoiDung.Email, 
                    SanPhamIDs = sanPhamIDs,
                    SoLuongs = soLuongs,
                    DonGias = donGias,
                    OtherName = otherName,
                    OtherPhone = otherPhone
                };
                _httpContextAccessor.HttpContext.Session.SetString("PendingOrder", JsonConvert.SerializeObject(orderData));

                return await CreatePaymentUrlVnpay(paymentInfo);
            }

            // Xử lý các phương thức thanh toán khác (không qua VNPAY)
            var datHang = new DatHang
            {
                NguoiDungID = userId.Value,
                TinhTrangID = 3,
                TenNguoiDat = tenNguoiDat,
                DienThoaiNguoiDat = dienThoaiNguoiDat,
                DiaChiGiaoHang = diaChiGiaoHang,
                NgayDatHang = DateTime.Now,
                TinhTrangThanhToan = 2, // Chưa thanh toán
                HinhThucGiaoHang = hinhThucGiaoHang,
                TenNguoiNhanHangKhac = otherName,
                SoDienThoaiNguoiNhanKhac = otherPhone,
                HinhThucThanhToan = paymentMethod
            };

            _context.DatHang.Add(datHang);
            await _context.SaveChangesAsync();

            var datHangChiTiet = sanPhamIDs.Select((spId, index) => new DatHangChiTiet
            {
                DatHangID = datHang.ID,
                SanPhamID = int.Parse(spId),
                SoLuong = (short)soLuongs[index],
                DonGia = donGias[index],
                TongTien = soLuongs[index] * donGias[index]
            }).ToList();

            await _context.DatHangChiTiet.AddRangeAsync(datHangChiTiet);
            await _context.SaveChangesAsync();
            // Gửi gmail
            try
            {
                var datHangInfo = await _context.DatHang
                    .Where(r => r.ID == datHang.ID)
                    .Include(s => s.NguoiDung)
                    .Include(s => s.DatHangChiTiet)
                    .ThenInclude(ct => ct.SanPham)
                    .SingleOrDefaultAsync();

                if (datHangInfo?.NguoiDung == null)
                {
                    TempData["ThongBaoLoi"] = "Lỗi: Không tìm thấy thông tin người đặt hàng!";
                    return RedirectToAction("DatHangThatBai", "GioHang");
                }
                
                await _mailLogic.GoiEmailDatHangThanhCong(datHangInfo, new MailInfo
                {
                    Subject = "Đặt hàng thành công tại Thần Tài Shop"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi gửi email: " + ex.Message);
            }

            // Cập nhật số lượng tồn kho và số lượng khuyến mãi
            for (int i = 0; i < sanPhamIDs.Length; i++)
            {
                int sanPhamID = int.Parse(sanPhamIDs[i]);
                int soLuongMua = soLuongs[i];

                var sanPham = await _context.SanPham.FirstOrDefaultAsync(s => s.ID == sanPhamID);
                var khuyenMai = await _context.KhuyenMai
                    .Where(km => km.SanPhamID == sanPhamID && km.NgayBatDau <= DateTime.Now && km.NgayKetThuc >= DateTime.Now && km.SoLuong > 0)
                    .OrderBy(km => km.NgayBatDau) // Ưu tiên khuyến mãi bắt đầu trước
                    .FirstOrDefaultAsync();

                if (sanPham != null)
                {
                    if (khuyenMai != null)
                    {
                        if (khuyenMai.SoLuong >= soLuongMua)
                        {
                            // Trừ trực tiếp số lượng khuyến mãi
                            khuyenMai.SoLuong -= soLuongMua;
                        }
                        else
                        {
                            int soLuongKhuyenMai = khuyenMai.SoLuong;
                            int soLuongConLai = soLuongMua - soLuongKhuyenMai;

                            // Hết số lượng khuyến mãi, trừ phần còn lại vào kho chính
                            khuyenMai.SoLuong = 0;
                            sanPham.SoLuong -= soLuongConLai;
                        }
                    }
                    else
                    {
                        // Không có khuyến mãi, trừ trực tiếp vào kho chính
                        sanPham.SoLuong -= soLuongMua;
                    }

                    sanPham.LuotBan += soLuongMua;
                }
                else
                {
                    TempData["ThongBaoLoi"] = $"Sản phẩm không tồn tại hoặc không đủ số lượng trong kho.";
                    return RedirectToAction("DatHangThatBai", "GioHang");
                }
            }

            // Lưu thay đổi vào database
            await _context.SaveChangesAsync();


            var gioHang = await _context.GioHang.Where(g => g.NguoiDungID == userId).ToListAsync();
            _context.GioHang.RemoveRange(gioHang);
            await _context.SaveChangesAsync();

            return RedirectToAction("DatHangThanhCong");
        }


        [HttpPost]
        public async Task<IActionResult> CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            // Kiểm tra mô hình đầu vào
            if (model == null || model.Amount <= 0)
            {
                TempData["ThongBaoLoi"] = "Thông tin thanh toán không hợp lệ.";
                return RedirectToAction("Index", "DatHang");
            }

            // Gọi dịch vụ tạo URL thanh toán
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            return Redirect(url);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (!response.Success)
            {
                TempData["ThongBaoLoi"] = $"Thanh toán thất bại: {response.VnPayResponseCode}";
                return RedirectToAction("Index", "GioHang");
            }

            int? userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                TempData["ThongBaoLoi"] = "Bạn cần đăng nhập để tiếp tục.";
                return RedirectToAction("Login", "Home");
            }

            var orderDataJson = _httpContextAccessor.HttpContext.Session.GetString("PendingOrder");
            if (string.IsNullOrEmpty(orderDataJson))
            {
                TempData["ThongBaoLoi"] = "Không tìm thấy thông tin đơn hàng!";
                return RedirectToAction("Index", "GioHang");
            }

            var orderData = JsonConvert.DeserializeObject<dynamic>(orderDataJson);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var nguoiDung = await _context.NguoiDung.FindAsync(userId.Value);
                if (nguoiDung == null)
                {
                    TempData["ThongBaoLoi"] = "Lỗi: Không tìm thấy thông tin người dùng!";
                    return RedirectToAction("DatHangThatBai", "GioHang");
                }

                var datHang = new DatHang
                {
                    NguoiDungID = userId.Value,
                    TinhTrangID = 3,
                    NgayDatHang = DateTime.Now,
                    TinhTrangThanhToan = 1,
                    HinhThucThanhToan = 3,
                    TenNguoiDat = orderData.TenNguoiDat,
                    DienThoaiNguoiDat = orderData.DienThoaiNguoiDat,
                    DiaChiGiaoHang = orderData.DiaChiGiaoHang,
                    HinhThucGiaoHang = (int)orderData.HinhThucGiaoHang,
                    TenNguoiNhanHangKhac = orderData.OtherName,
                    SoDienThoaiNguoiNhanKhac = orderData.OtherPhone
                };

                _context.DatHang.Add(datHang);
                await _context.SaveChangesAsync();

                var datHangChiTietList = ((JArray)orderData.SanPhamIDs).Select((spId, index) => new DatHangChiTiet
                {
                    DatHangID = datHang.ID,
                    SanPhamID = int.Parse(spId.ToString()),
                    SoLuong = (short)((JArray)orderData.SoLuongs)[index],
                    DonGia = (decimal)((JArray)orderData.DonGias)[index],
                    TongTien = ((short)((JArray)orderData.SoLuongs)[index]) * ((decimal)((JArray)orderData.DonGias)[index])
                }).ToList();

                await _context.DatHangChiTiet.AddRangeAsync(datHangChiTietList);
                await _context.SaveChangesAsync();

                var sanPhamIDs = ((JArray)orderData.SanPhamIDs).Select(id => int.Parse(id.ToString())).ToList();
                var soLuongs = ((JArray)orderData.SoLuongs).Select(sl => (short)sl).ToList();

                for (int i = 0; i < sanPhamIDs.Count; i++)
                {
                    var sanPham = await _context.SanPham.FirstOrDefaultAsync(s => s.ID == sanPhamIDs[i]);

                    if (sanPham != null)
                    {
                        if (sanPham.SoLuong >= soLuongs[i])
                        {
                            //sanPham.SoLuong -= soLuongs[i]; // Trừ số lượng tồn kho
                            sanPham.LuotBan += soLuongs[i]; //Tăng lượt bán lên
                        }
                        else
                        {
                            TempData["ThongBaoLoi"] = $"Sản phẩm {sanPham.TenSanPham} không đủ số lượng trong kho.";
                            return RedirectToAction("DatHangThatBai", "GioHang");
                        }
                    }
                }

                // Lưu thay đổi vào database
                await _context.SaveChangesAsync();


                // Xóa giỏ hàng
                var gioHang = await _context.GioHang.Where(g => g.NguoiDungID == userId).ToListAsync();
                _context.GioHang.RemoveRange(gioHang);
                await _context.SaveChangesAsync();
                // Gửi gmail
                try
                {
                    var datHangInfo = await _context.DatHang
                        .Where(r => r.ID == datHang.ID)
                        .Include(s => s.NguoiDung)
                        .Include(s => s.DatHangChiTiet)
                        .ThenInclude(ct => ct.SanPham)
                        .SingleOrDefaultAsync();

                    if (datHangInfo?.NguoiDung == null)
                    {
                        TempData["ThongBaoLoi"] = "Lỗi: Không tìm thấy thông tin người đặt hàng!";
                        return RedirectToAction("DatHangThatBai", "GioHang");
                    }

                    Console.WriteLine($"Đang gửi email đến: {datHangInfo.NguoiDung.Email}");
                    await _mailLogic.GoiEmailDatHangThanhCong(datHangInfo, new MailInfo
                    {
                        Subject = "Đặt hàng thành công tại Thần Tài Shop"
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi khi gửi email: " + ex.Message);
                }
                //Xóa seesion
                _httpContextAccessor.HttpContext.Session.Remove("PendingOrder");

                await transaction.CommitAsync();

                TempData["ThongBaoThanhCong"] = "Thanh toán và đặt hàng thành công!";
                return RedirectToAction("DatHangThanhCong", "GioHang");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ThongBaoLoi"] = "Có lỗi khi xử lý đơn hàng: " + ex.Message;
                return RedirectToAction("DatHangThatBai", "GioHang");
            }
        }

        public IActionResult DatHangThanhCong()
        {
            return View();
        }

        public IActionResult DatHangThatBai()
        {
            return View();
        }

        // Hàm loại bỏ kí tự tiếng việt
        public static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}