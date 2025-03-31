using System;
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
        public int SanPhamID { get; set; }

        [DisplayName("Ngày bắt đầu")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime NgayBatDau { get; set; }

        [DisplayName("Ngày kết thúc")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime NgayKetThuc { get; set; }

        [DisplayName("Số lượng")]
        public int SoLuong { get; set; }

        [DisplayName("Mô tả")]
        public string? MoTa { get; set; }

        [DisplayName("Kiểu khuyến mãi")]
        public int KieuKhuyenMai { get; set; }  // 0 = %, 1 = số tiền

        [Column(TypeName = "decimal(18,2)")]
        [DisplayName("Giá trị giảm")]
        public decimal GiaTriGiam { get; set; }

        [DisplayName("Trạng thái")]
        public int TrangThai { get; set; } = 1; // 1 là còn khuyến mãi,2 là hết

        // Liên kết với sản phẩm
        public virtual SanPham? SanPham { get; set; }

        [NotMapped]
        public int TrangThaiTinhToan
        {
            get
            {
                if (NgayKetThuc <= DateTime.Now || SoLuong <= 0)
                {
                    return 2; // Hết hạn hoặc hết số lượng
                }
                return TrangThai; // Giữ nguyên trạng thái trong DB
            }
        }
    }
}
