using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ThanTai.Models
{
    public class TinhTrang
    {
        [DisplayName("Mã tình trạng")]
        public int ID { get; set; }

        [StringLength(255)]
        [Required(ErrorMessage = "Tên tình trạng không được bỏ trống.")]
        [DisplayName("Tên tình trạng")]
        public string MoTa { get; set; }

        public ICollection<DatHang>? DatHang { get; set; }
    }
}
