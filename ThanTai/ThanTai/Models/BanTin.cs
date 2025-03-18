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

    [NotMapped]
    public class BanTin_ChinhSua
    {
        public BanTin_ChinhSua()
        {
        }

        public BanTin_ChinhSua(BanTin banTin)
        {
            ID = banTin.ID;
            Title = banTin.Title;
            Slug = banTin.Slug;
            Image = banTin.Image;
            Banner = banTin.Banner;
            Category = banTin.Category;
            Content = banTin.Content;
            CreatedAt = banTin.CreatedAt;
        }

        [DisplayName("Mã bản tin")]
        public int ID { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được bỏ trống.")]
        [StringLength(255)]
        [DisplayName("Tiêu đề")]
        public string Title { get; set; }

        [DisplayName("URL thân thiện")]
        public string Slug { get; set; }

        [DisplayName("Ảnh đại diện")]
        public string? Image { get; set; }

        [DisplayName("Ảnh banner")]
        public string? Banner { get; set; }

        [Required(ErrorMessage = "Danh mục không được bỏ trống.")]
        [DisplayName("Danh mục")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Nội dung không được bỏ trống.")]
        [DisplayName("Nội dung tin tức")]
        public string Content { get; set; }

        [DisplayName("Ngày tạo")]
        public DateTime CreatedAt { get; set; }

        [NotMapped]
        [Display(Name = "Chọn ảnh đại diện mới")]
        public IFormFile? DuLieuHinhAnhImage { get; set; }

        [NotMapped]
        [Display(Name = "Chọn ảnh banner mới")]
        public IFormFile? DuLieuHinhAnhBanner { get; set; }
    }
}
