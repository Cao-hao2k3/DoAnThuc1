using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ThanTai.Models;
using BC = BCrypt.Net.BCrypt;
using Microsoft.EntityFrameworkCore;
using ThanTai.ViewModels;

namespace ThanTai.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ThanTaiShopDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public HomeController(ILogger<HomeController> logger, ThanTaiShopDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            var danhSachKhuyenMai = _context.KhuyenMai
                .Include(km => km.SanPham)
                .ThenInclude(sp => sp.HinhAnhSanPham)
                .Where(km => km.TrangThai == 1 && km.NgayKetThuc > DateTime.Now && km.SoLuong > 0)
                .ToList();

            var danhSachBanTin = _context.BanTin
                .OrderByDescending(bt => bt.CreatedAt)
                .Take(5) // Chỉ lấy 5 bản tin mới nhất
                .ToList();

            var viewModel = new TrangChuViewModels
            {
                DanhSachKhuyenMai = danhSachKhuyenMai,
                DanhSachBanTin = danhSachBanTin
            };

            return View(viewModel);
        }

        // GET: Login
        [AllowAnonymous]
        public IActionResult Login(string? ReturnUrl)
        {
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                return LocalRedirect(ReturnUrl ?? "/");
            }
            else
            {
                ViewBag.LienKetChuyenTrang = ReturnUrl ?? "/";
                return View();
            }
        }

        // POST: Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([Bind] DangNhap dangNhap)
        {
            if (ModelState.IsValid)
            {
                var nguoiDung = _context.NguoiDung.SingleOrDefault(r => r.TenDangNhap == dangNhap.TenDangNhap);

                if (nguoiDung == null || !BC.Verify(dangNhap.MatKhau, nguoiDung.MatKhau))
                {
                    TempData["ThongBaoLoi"] = "Tài khoản hoặc mật khẩu không chính xác.";
                    return View(dangNhap);
                }

                // Tạo claims để lưu thông tin người dùng
                var claims = new List<Claim>
        {
            new Claim("ID", nguoiDung.ID.ToString()),
            new Claim(ClaimTypes.Name, nguoiDung.TenDangNhap),
            new Claim("HoVaTen", nguoiDung.HoVaTen),
            new Claim(ClaimTypes.Role, nguoiDung.Quyen ? "Admin" : "User")
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = dangNhap.DuyTriDangNhap
                };

                // Đăng nhập hệ thống
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                              new ClaimsPrincipal(claimsIdentity),
                                              authProperties);

                //  Lưu thông tin vào Session
                _httpContextAccessor.HttpContext.Session.SetString("UserName", nguoiDung.HoVaTen);
                _httpContextAccessor.HttpContext.Session.SetString("UserImage", nguoiDung.Anh);
                _httpContextAccessor.HttpContext.Session.SetInt32("UserID", nguoiDung.ID);
                if (nguoiDung.Quyen)
                {
                    return RedirectToRoute(new { area = "Admin", controller = "Home", action = "Index" });
                }
                return LocalRedirect(dangNhap.LienKetChuyenTrang ?? "/");
            }

            return View(dangNhap);
        }

        // GET: Register
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Register(NguoiDung model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Kiểm tra tên đăng nhập đã tồn tại chưa
                    if (_context.NguoiDung.Any(x => x.TenDangNhap == model.TenDangNhap))
                    {
                        TempData["ThongBaoLoi"] = "Tên đăng nhập đã tồn tại!";
                        return View(model);
                    }

                    // Kiểm tra email đã tồn tại chưa
                    if (_context.NguoiDung.Any(x => x.Email == model.Email))
                    {
                        TempData["ThongBaoLoi"] = "Email này đã được sử dụng!";
                        return View(model);
                    }

                    // Mã hóa mật khẩu trước khi lưu
                    model.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau);

                    // Xử lý ảnh đại diện nếu có
                    if (model.DuLieuHinhAnh != null)
                    {
                        var fileName = Path.GetFileName(model.DuLieuHinhAnh.FileName);
                        var filePath = Path.Combine("wwwroot/uploads", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            model.DuLieuHinhAnh.CopyTo(stream);
                        }

                        model.Anh = "/uploads/" + fileName;
                    }
                    else
                    {
                        // Gán ảnh mặc định nếu không có ảnh tải lên
                        model.Anh = "/uploads/anhmacdinh.jpg";
                    }

                    _context.NguoiDung.Add(model);
                    _context.SaveChanges();

                    ViewBag.ThongBaoThanhCong = "Đăng ký thành công! Đang chuyển hướng đến trang đăng nhập...";
                    return View();
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để kiểm tra sau (tùy vào cách bạn lưu log)
                Console.WriteLine("Lỗi đăng ký tài khoản: " + ex.Message);

                // Hiển thị thông báo lỗi chung cho người dùng
                TempData["ThongBaoLoi"] = "Hệ thống đang lỗi, vui lòng thử lại sau!";
            }

            return View(model);
        }

        // GET: Logout
        public async Task<IActionResult> Logout()
        {
            // Xóa Session khi đăng xuất
            HttpContext.Session.Clear();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home", new { Area = "" });

        }

        // GET: Forbidden
        public IActionResult Forbidden()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
