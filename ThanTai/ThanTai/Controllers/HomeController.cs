using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ThanTai.Models;
using BC = BCrypt.Net.BCrypt;

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
            return View();
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
        public IActionResult Register(NguoiDung model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra tên đăng nhập đã tồn tại chưa
                if (_context.NguoiDung.Any(x => x.TenDangNhap == model.TenDangNhap))
                {
                    TempData["ThongBaoLoi"] = "Tên đăng nhập đã tồn tại!";
                    return View(model);
                }

                // Mã hóa mật khẩu bằng BCrypt trước khi lưu vào CSDL
                model.MatKhau = BC.HashPassword(model.MatKhau);
                _context.NguoiDung.Add(model);
                _context.SaveChanges();

                TempData["ThongBao"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
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
