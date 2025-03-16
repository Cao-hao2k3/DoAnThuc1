using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThanTai.Models
{
    public class KhuyenMai
    {
        [DisplayName("Mã số")]
        public int ID { get; set; }

        [DisplayName("Tên sản phẩm")]
        public int SanPhamID { get; set; }  // Liên kết với sản phẩm được giảm giá

        [DisplayName("Ngày bắt đầu")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime NgayBatDau { get; set; }

        [DisplayName("Ngày kết thúc")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime NgayKetThuc { get; set; }

        [DisplayName("Số lượng")]
        public int SoLuong { get; set; }  // Số lượng sản phẩm được áp dụng

        [DisplayName("Mô tả")]
        public string? MoTa { get; set; }  // Mô tả khuyến mãi

        [DisplayName("Kiểu khuyến mãi")]
        public int KieuKhuyenMai { get; set; } // 0 = %, 1 = số tiền, 2 = Mua 1 tặng 1

        [Column(TypeName = "decimal(18,2)")]
        [DisplayName("Giá trị giảm")]
        public decimal GiaTriGiam { get; set; }  // Phần trăm giảm giá hoặc số tiền giảm

        [DisplayName("Trạng thái")]
        public int TrangThai { get; set; } // 0 = Tắt, 1 = Đang diễn ra, 2 = Hết hạn

        // Liên kết với sản phẩm
        public virtual SanPham? SanPham { get; set; }
    }
}
