using System.ComponentModel.DataAnnotations;

public class DatLaiMatKhauViewModel
{
    public string Token { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string MatKhauMoi { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("MatKhauMoi", ErrorMessage = "Mật khẩu không khớp.")]
    public string XacNhanMatKhau { get; set; }
}
