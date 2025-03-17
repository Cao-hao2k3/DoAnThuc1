using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThanTai.Models
{
    public class BanTin
    {
        public int ID { get; set; }

        [Required, StringLength(255)]
        public string Title { get; set; } // Tiêu đề

        public string Slug { get; set; } // URL thân thiện

        public string? Image { get; set; } // Ảnh đại diện

        public string? Banner { get; set; } // Ảnh banner

        public string Category { get; set; } // Danh mục (Đồng hồ, Điện thoại,...)

        [Required]
        public string Content { get; set; } // Nội dung tin

        [NotMapped]
        public IFormFile? DuLieuHinhAnhImage { get; set; }

        [NotMapped]
        public IFormFile? DuLieuHinhAnhBanner { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
