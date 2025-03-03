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

        // Quan hệ 1 - N (một danh mục cha có nhiều danh mục con)
        // Liên kết đến danh mục cha
        [ForeignKey("ParentID")]
        public virtual LoaiSanPham? ParentCategory { get; set; }
        // Danh sách các danh mục con
        public virtual ICollection<LoaiSanPham>? SubCategories { get; set; } = new List<LoaiSanPham>();
        public virtual ICollection<SanPham>? SanPham { get; set; } = new List<SanPham>();

    }
}
