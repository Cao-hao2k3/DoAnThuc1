using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ThanTai.Models
{
    public class DatHang
    {
        [DisplayName("Mã đặt hàng")]
        public int ID { get; set; }

        [DisplayName("Mã người dùng")]
        public int NguoiDungID { get; set; }

        [DisplayName("Mã tình trạng")]
        public int TinhTrangID { get; set; }

        [StringLength(225)]
        [DisplayName("Tên người đặt")]
        public string TenNguoiDat { get; set; }

        [StringLength(20)]
        [DisplayName("Điện thoại giao hàng")]
        public string DienThoaiNguoiDat { get; set; }

        [StringLength(255)]
        [DisplayName("Địa chỉ giao hàng")]
        public string DiaChiGiaoHang { get; set; }

        [DisplayName("Ngày đặt hàng")]
        public DateTime NgayDatHang { get; set; }

        [DisplayName("Tình trạng thanh toán")]
        public int TinhTrangThanhToan { get; set; } // 1 là đã thanh toán 2 là chưa 

        [DisplayName("Hình thức giao hàng")]
        public int HinhThucGiaoHang { get; set; } // 1 là giao hàng tận nơi 2 là nhận tại cửa hàng

        [StringLength(225)]
        [DisplayName("Tên người nhận khác")]
        public string? TenNguoiNhanHangKhac { get; set; }

        [StringLength(225)]
        [DisplayName("Số điện thoại người nhận khác")]
        public string? SoDienThoaiNguoiNhanKhac { get; set; }

        [DisplayName("Số điện thoại người nhận khác")]
        public int HinhThucThanhToan { get; set; } // 1 là thanh toán khi nhận hàng,2 là chuyển khoản ngân hàng, 3 là qua VNPAY
        public NguoiDung? NguoiDung { get; set; }
        public TinhTrang? TinhTrang { get; set; }
        public ICollection<DatHangChiTiet>? DatHangChiTiet { get; set; }
    }
}
