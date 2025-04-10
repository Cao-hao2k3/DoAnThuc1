using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThanTai.Models
{
    public class NguoiDung
    {
        [DisplayName("Mã người dùng")]
        public int ID { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "Họ và tên không được bỏ trống.")]
        [DisplayName("Họ và tên")]
        public string HoVaTen { get; set; }

        [StringLength(255)]
        [Required(ErrorMessage = "Email không được bỏ trống.")]
        [DisplayName("Email")]
        public string Email { get; set; }

        [StringLength(20)]
        [Required(ErrorMessage = "Số điện thoại không được bỏ trống.")]
        [DisplayName("Số điện thoại")]
        public string DienThoai { get; set; }

        [StringLength(255)]
        [Required(ErrorMessage = "Địa chỉ không được bỏ trống.")]
        [DisplayName("Địa chỉ người dùng")]
        public string DiaChi { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "Tên đăng nhập không được bỏ trống.")]
        [DisplayName("Tên đăng nhập")]
        public string TenDangNhap { get; set; }

        [StringLength(255)]
        [Required(ErrorMessage = "Mật khẩu không được bỏ trống.")]
        [DisplayName("Mật khâu")]
        public string MatKhau { get; set; }

        [NotMapped]
        [Display(Name = "Xác nhận mật khẩu")]
        [Required(ErrorMessage = "Xác nhận mật khẩu không được bỏ trống!")]
        [Compare("MatKhau", ErrorMessage = "Xác nhận mật khẩu không chính xác!")]
        [DataType(DataType.Password)]
        public string XacNhanMatKhau { get; set; }

        [DisplayName("Quyền hạn admin")]
        public bool Quyen { get; set; }

        [DisplayName("Ảnh tài khoản")]
        public string? Anh { get; set; }

        [NotMapped]
        [Display(Name = "Hình ảnh người dùng")]
        public IFormFile? DuLieuHinhAnh { get; set; }

        [DisplayName("Token đặt lại mật khẩu")]
        [StringLength(255)]
        public string? ResetPasswordToken { get; set; }

        [DisplayName("Thời gian hết hạn token")]
        public DateTime? TokenExpiryTime { get; set; }

        public ICollection<DatHang>? DatHang { get; set; }

        public ICollection<GioHang>? GioHang { get; set; }

        public ICollection<QuanLyKhoHang>? QuanLyKhoHang { get;set; }

        public ICollection<DanhGiaSanPham>? DanhGiaSanPham { get; set; }
    }

    [NotMapped]
    public class NguoiDung_ChinhSua
    {
        public NguoiDung_ChinhSua()
        {
        }

        public NguoiDung_ChinhSua(NguoiDung n)
        {
            ID = n.ID;
            HoVaTen = n.HoVaTen;
            Email = n.Email;
            DienThoai = n.DienThoai;
            DiaChi = n.DiaChi;
            TenDangNhap = n.TenDangNhap;
            Quyen = n.Quyen;
            Anh = n.Anh;
        }

        [DisplayName("Mã ND")]
        public int ID { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "Họ và tên không được bỏ trống.")]
        [DisplayName("Họ và tên")]
        public string HoVaTen { get; set; }

        [StringLength(255)]
        [Required(ErrorMessage = "Email không được bỏ trống.")]
        [DisplayName("Email")]
        public string Email { get; set; }

        [StringLength(20)]
        [DisplayName("Điện thoại")]
        public string? DienThoai { get; set; }

        [StringLength(255)]
        [DisplayName("Địa chỉ")]
        public string? DiaChi { get; set; }

        [StringLength(50, ErrorMessage = "{0} phải ít nhất {2} ký tự.", MinimumLength = 4)]
        [Required(ErrorMessage = "Tên đăng nhập không được bỏ trống.")]
        [DisplayName("Tên đăng nhập")]
        public string TenDangNhap { get; set; }

        [StringLength(255)]
        [DataType(DataType.Password)]
        [DisplayName("Mật khẩu mới")]
        public string? MatKhauMoi { get; set; }

        [NotMapped]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("MatKhauMoi", ErrorMessage = "Mật khẩu không trùng khớp với mật khẩu mới ! Vui lòng thử lại !")]
        [DataType(DataType.Password)]
        public string? XacNhanMatKhau { get; set; }

        [DisplayName("Quyền hạn")]
        public bool Quyen { get; set; }

        [DisplayName("Ảnh tài khoản")]
        public string? Anh { get; set; }

        [NotMapped]
        [Display(Name = "Hình ảnh người dùng")]
        public IFormFile? DuLieuHinhAnh { get; set; }
    }

    [NotMapped]
    public class DangNhap
    {
        [StringLength(50, ErrorMessage = "{0} phải ít nhất {2} ký tự.", MinimumLength = 4)]
        [Required(ErrorMessage = "Tên đăng nhập không được bỏ trống!")]
        [DisplayName("Tên đăng nhập")]
        public string TenDangNhap { get; set; }

        [StringLength(255)]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Mật khẩu không được bỏ trống!")]
        [DisplayName("Mật khẩu")]
        public string MatKhau { get; set; }

        [DisplayName("Duy trì đăng nhập")]
        public bool DuyTriDangNhap { get; set; }

        [StringLength(255)]
        [DisplayName("Liên kết chuyển trang")]
        public string? LienKetChuyenTrang { get; set; }
    }
}
