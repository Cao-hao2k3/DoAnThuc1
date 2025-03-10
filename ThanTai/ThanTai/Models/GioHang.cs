using System.ComponentModel;

namespace ThanTai.Models
{
    public class GioHang
    {
        internal decimal DonGia;

        [DisplayName("Mã giỏ hàng")]
        public int ID { get; set; }
         
        public int NguoiDungID { get; set; }

        public int SanPhamID { get; set; }

        public int SoLuong { get; set; }
        
        public NguoiDung? NguoiDung { get; set; }

        public virtual SanPham? SanPham { get; set; }
    }
}
