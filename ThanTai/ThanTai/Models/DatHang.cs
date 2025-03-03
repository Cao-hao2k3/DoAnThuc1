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
        public string? TenNguoiDat { get; set; }

        [StringLength(20)]
        [DisplayName("Điện thoại giao hàng")]
        public string? DienThoaiGiaoHang { get; set; }

        [StringLength(255)]
        [DisplayName("Địa chỉ giao hàng")]
        public string? DiaChiGiaoHang { get; set; }

        [DisplayName("Ngày đặt hàng")]
        public DateTime NgayDatHang { get; set; }

        [DisplayName("Tình trạng thanh toán")]
        public bool TinhTrangThanhToan { get; set; }
        public NguoiDung? NguoiDung { get; set; }
        public TinhTrang? TinhTrang { get; set; }
        public ICollection<DatHangChiTiet>? DatHangChiTiet { get; set; }
    }
}
