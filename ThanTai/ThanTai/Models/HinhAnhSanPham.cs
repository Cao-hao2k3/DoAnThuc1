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
        [StringLength(2000)]
        [DisplayName("Ảnh sản phẩm")]
        public string AnhSanPham { get; set; } = "[]"; // Lưu danh sách ảnh dưới dạng JSON

        [StringLength(500)]
        [DisplayName("Ảnh thông số sản phẩm")]
        public string? AnhThongSo { get; set; }

        [StringLength(500)]
        [DisplayName("Link video review youtube")]
        public string? VideoReview { get; set; }

        [ForeignKey("SanPhamID")]
        public virtual SanPham? SanPham { get; set; }

        // Thuộc tính không ánh xạ vào database, dùng để thao tác trong controller
        [NotMapped]
        public List<string> AnhSanPhamList
        {
            get => string.IsNullOrEmpty(AnhSanPham) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(AnhSanPham);
            set => AnhSanPham = JsonConvert.SerializeObject(value);
        }

        // Thuộc tính dùng để upload file trong form
        [NotMapped]
        [DisplayName("Danh sách ảnh sản phẩm")]
        public List<IFormFile>? AnhSanPhamFiles { get; set; }

        [NotMapped]
        [DisplayName("Ảnh thông số sản phẩm")]
        public IFormFile? AnhThongSoFile { get; set; }

        [NotMapped]
        public string? VideoReviewEmbed
        {
            get
            {
                if (!string.IsNullOrEmpty(VideoReview) && VideoReview.Contains("watch?v="))
                {
                    return VideoReview.Replace("watch?v=", "embed/");
                }
                return VideoReview;
            }
        }

    }
}
