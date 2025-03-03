using System.ComponentModel;

namespace ThanTai.Models
{
    public class GioHang
    {
        [DisplayName("Mã giỏ hàng")]
        public int ID { get; set; }
         
        public int NguoiDungID { get; set; }

        public int SanPhamID { get; set; }
        
        public int SoLuong { get;set; }

        public NguoiDung? NguoiDung { get; set; }

        public SanPham? SanPham { get; set; }
    }
}
