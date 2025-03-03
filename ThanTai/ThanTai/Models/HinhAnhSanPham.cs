using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using Newtonsoft.Json;

namespace ThanTai.Models
{
    public class HinhAnhSanPham
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [DisplayName("Mã sản phẩm")]
        public int SanPhamID { get; set; }

        [Required]
        [StringLength(500)]
        [DisplayName("Ảnh sản phẩm")]
        public string AnhSanPham { get; set; } = "[]";

        [StringLength(500)]
        [DisplayName("Ảnh thông số sản phẩm")]
        public string? AnhThongSo { get; set; }

        [ForeignKey("SanPhamID")]
        public virtual SanPham? SanPham { get; set; }

        [NotMapped]
        public List<string> AnhSanPhamList
        {
            get => string.IsNullOrEmpty(AnhSanPham) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(AnhSanPham);
            set => AnhSanPham = JsonConvert.SerializeObject(value);
        }
    }
}
