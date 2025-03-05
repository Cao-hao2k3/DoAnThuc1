using System.ComponentModel;

namespace ThanTai.Models.ViewModels
{
    public class GiaTriThuocTinhViewModel
    {
        [DisplayName("Mã")]
        public int ID { get; set; }

        [DisplayName("Tên sản phẩm")]
        public string TenSanPham { get; set; }
        [DisplayName("Tên thuộc tính")]
        public string TenThuocTinh { get; set; }
        [DisplayName("Giá trị ")]
        public string GiaTri { get; set; }
    }
}
