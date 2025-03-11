using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Logic;
using ThanTai.Models;
using WebBanHang.Services.VNPAY;
using ThanTai.Logic;

var builder = WebApplication.CreateBuilder(args);

// 1️ Thêm dịch vụ Session vào ứng dụng
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session hết hạn sau 30 phút
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Bắt buộc với GDPR
});

// 2️ Thêm dịch vụ HTTP Context Accessor để dùng Session trong Views và Controllers
builder.Services.AddHttpContextAccessor();

// 3️ Cấu hình Entity Framework với SQL Server
builder.Services.AddDbContext<ThanTaiShopDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ThanTaiShopConnection")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 4️ Cấu hình Authentication bằng Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "ThanTaiShop.Cookie";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
        options.SlidingExpiration = true;
        options.LoginPath = "/Home/Login";
        options.LogoutPath = "/Home/Logout";
        options.AccessDeniedPath = "/Home/Forbidden";
    });

builder.Services.AddControllersWithViews();

//Connect VNPAY API
builder.Services.AddScoped<IVnPayService, VnPayService>();

//Email
builder.Services.AddTransient<IMailLogic, MailLogic>();
// Lấy thông tin cấu hình trong tập tin appsettings.json và gán vào đối tượng MailSettings
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

var app = builder.Build();

// 5️ Cấu hình Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 6️ Kích hoạt Session trước Authentication
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// 7️ Cấu hình Routing
app.MapControllerRoute(
    name: "adminareas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
