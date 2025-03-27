using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThanTai.Models
{
    public class LoaiSanPham
    {
        [Key]
        [DisplayName("Mã loại sản phẩm")]
        public int ID { get; set; }

        [Required(ErrorMessage = "Tên loại không được bỏ trống.")]
        [StringLength(255)]
        [DisplayName("Tên lọai")]
        public string Tenloai { get; set; }

        [DisplayName("Mã sản phẩm cha")]
        public int? ParentID { get; set; }

        [ForeignKey("ParentID")]
        public virtual LoaiSanPham? ParentCategory { get; set; }

        public virtual ICollection<LoaiSanPham>? SubCategories { get; set; } = new List<LoaiSanPham>();

        public virtual ICollection<SanPham>? SanPham { get; set; } = new List<SanPham>();

        public virtual ICollection<ThuocTinh>? ThuocTinh { get; set; } = new List<ThuocTinh>(); // Liên kết với thuộc tính
    }

}
