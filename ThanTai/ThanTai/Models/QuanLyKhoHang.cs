using System.ComponentModel.DataAnnotations;

namespace ThanTai.Models
{
    public class QuanLyKhoHang
    {
        public int ID { get; set; }
        public int SanPhamID { get; set; }
        public int NguoiDungID { get; set; }
        public int LoaiGiaoDich { get;set; } //1 là nhập 2 là xuất

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int SoLuong { get; set; }
        public DateTime ThoiGian { get; set; }
        public string? GhiChu { get; set;}

        public SanPham? SanPham { get; set; }

        public NguoiDung? NguoiDung { get; set; }
    }
}
