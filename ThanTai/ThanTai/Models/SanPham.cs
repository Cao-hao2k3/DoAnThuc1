using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ThanTai.Models
{
    public class SanPham
    {
        [Key]
        [DisplayName("Mã sản phẩm")]
        public int ID { get; set; }

        [DisplayName("Mã loại sản phẩm")]
        public int LoaiSanPhamID { get; set; }

        [DisplayName("Mã thương hiệu")]
        public int ThuongHieuID { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được bỏ trống")]
        [StringLength(255)]
        [DisplayName("Tên sản phẩm")]
        public string TenSanPham { get; set; }

        [Required(ErrorMessage = "Đơn giá không được bỏ trống")]
        [Column(TypeName = "decimal(18,2)")]
        [DisplayName("Đơn giá")]
        public decimal DonGia { get; set; }

        [DisplayName("Số lượng")]
        [Required(ErrorMessage = "Số lượng không được bỏ trống")]
        public int SoLuong { get; set; }

        [Column(TypeName = "ntext")]
        [DataType(DataType.MultilineText)]
        public string? MoTa { get; set; }

        [DisplayName("Giảm giá (%)")]
        [Range(0, 100, ErrorMessage = "Giảm giá phải từ 0% đến 100%")]
        public int? GiamGia { get; set; } = 0; // Đặt giá trị mặc định là 0

        public int LuotDanhGia { get; set; } = 0;
        public int LuotBan { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        [DisplayName("Giá sau khi giảm")]
        public decimal GiaSauKhiGiam { get; set; }

        [DisplayName("Thông tin thông số")]
        public string? ThongTinThongSo { get; set; }

        [ForeignKey("LoaiSanPhamID")]
        public virtual LoaiSanPham? LoaiSanPham { get; set; }

        public virtual ThuongHieu? ThuongHieu { get; set; }
        public virtual ICollection<DatHangChiTiet>? DatHangChiTiet { get; set; } = new List<DatHangChiTiet>();
        public virtual ICollection<GioHang>? GioHang { get; set; } = new List<GioHang>();
        public virtual ICollection<HinhAnhSanPham>? HinhAnhSanPham { get; set; } = new List<HinhAnhSanPham>();
        public virtual ICollection<GiaTriThuocTinh>? GiaTriThuocTinhs { get; set; } = new List<GiaTriThuocTinh>();
        public virtual ICollection<KhuyenMai>? KhuyenMais { get; set; } = new List<KhuyenMai>(); 
        public virtual ICollection<QuanLyKhoHang>? QuanLyKhoHang { get; set;} = new List<QuanLyKhoHang>();
    }

    public class ThuocTinh
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(255)]
        [DisplayName("Tên thuộc tính")]
        public string TenThuocTinh { get; set; }

        public virtual ICollection<GiaTriThuocTinh>? GiaTriThuocTinhs { get; set; } = new List<GiaTriThuocTinh>();
    }

    public class GiaTriThuocTinh
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [DisplayName("Mã sản phẩm")]
        public int SanPhamID { get; set; }

        [Required]
        [DisplayName("Mã thuộc tính")]
        public int ThuocTinhID { get; set; }

        [Required]
        [StringLength(255)]
        [DisplayName("Giá trị thuộc tính")]
        public string GiaTri { get; set; }

        [ForeignKey("SanPhamID")]
        public virtual SanPham? SanPham { get; set; }

        [ForeignKey("ThuocTinhID")]
        public virtual ThuocTinh? ThuocTinh { get; set; }
    }

}
