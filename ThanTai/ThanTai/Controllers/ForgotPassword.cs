using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThanTai.Models;
using ThanTai.Logic;
using WebBanHang.Logic;
using BC = BCrypt.Net.BCrypt;

namespace ThanTai.Controllers
{
    public class ForgotPasswordController : Controller
    {
        private readonly ThanTaiShopDbContext _context;
        private readonly IMailLogic _mailLogic;

        public ForgotPasswordController(ThanTaiShopDbContext context, IMailLogic mailLogic)
        {
            _context = context;
            _mailLogic = mailLogic;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("", "Vui lòng nhập email.");
                return View();
            }

            var user = await _context.NguoiDung.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email không tồn tại trong hệ thống.");
                return View();
            }

            // Tạo token reset password
            var token = Guid.NewGuid().ToString();
            user.ResetPasswordToken = token;
            user.TokenExpiryTime = DateTime.Now.AddMinutes(30);

            _context.NguoiDung.Update(user);
            await _context.SaveChangesAsync();

            // Tạo link reset password
            var callbackUrl = Url.Action("DatLaiMatKhau", "ForgotPassword", new { token = token }, Request.Scheme);


            var mailInfo = new MailInfo
            {
                ToEmail = user.Email,
                Subject = "Đặt lại mật khẩu",
                Body = $"<p>Xin chào <strong>{user.HoVaTen}</strong>,</p>" +
              "<p>Bạn vừa yêu cầu đặt lại mật khẩu tài khoản của mình.</p>" +
              $"<p>Vui lòng bấm vào liên kết dưới đây để đặt lại mật khẩu (có hiệu lực trong 30 phút):</p>" +
              $"<p><a href='{callbackUrl}' style='color: blue'>{callbackUrl}</a></p>" +
              "<p>Nếu bạn không yêu cầu điều này, vui lòng bỏ qua email này.</p>" +
              "<p>Trân trọng,<br/>Đội ngũ hỗ trợ ThanTai</p>"
            };
            await _mailLogic.GoiEmail(mailInfo);


            await _mailLogic.GoiEmail(mailInfo);

            ViewBag.Message = "Chúng tôi đã gửi một liên kết đặt lại mật khẩu đến email của bạn.";
            return View();
        }

        public IActionResult DatLaiMatKhau(string token)
        {
            var user = _context.NguoiDung.FirstOrDefault(x => x.ResetPasswordToken == token && x.TokenExpiryTime > DateTime.Now);
            if (user == null)
            {
                return Content("Liên kết không hợp lệ hoặc đã hết hạn.");
            }

            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatLaiMatKhau(DatLaiMatKhauViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Token = model.Token;
                return View(model);
            }

            var user = await _context.NguoiDung
                .FirstOrDefaultAsync(x => x.ResetPasswordToken == model.Token && x.TokenExpiryTime > DateTime.Now);

            if (user == null)
            {
                ViewBag.ThongBao = "Token không hợp lệ hoặc đã hết hạn.";
                return View("ThongBao");
            }

            // Mã hóa mật khẩu mới bằng BCrypt
            user.MatKhau = BC.HashPassword(model.MatKhauMoi);

            // Xóa token sau khi sử dụng
            user.ResetPasswordToken = null;
            user.TokenExpiryTime = null;

            _context.Update(user);
            await _context.SaveChangesAsync();

            return View("DatLaiMatKhauThanhCong");
        }

        public IActionResult DatLaiMatKhauThanhCong()
        {
            return View();
        }
    }
}