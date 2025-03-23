using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThanTai.ViewModels
{
    [NotMapped]
    public class DoiMatKhauViewModels
    {
        [Required(ErrorMessage = "Mật khẩu hiện tại không được bỏ trống.")]
        [DataType(DataType.Password)]
        [DisplayName("Mật khẩu hiện tại")]
        public string MatKhauHienTai { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới không được bỏ trống.")]
        [DataType(DataType.Password)]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự.")]
        [DisplayName("Mật khẩu mới")]
        public string MatKhauMoi { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu mới không được bỏ trống.")]
        [DataType(DataType.Password)]
        [Compare("MatKhauMoi", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        [DisplayName("Xác nhận mật khẩu mới")]
        public string XacNhanMatKhauMoi { get; set; }
    }
}
