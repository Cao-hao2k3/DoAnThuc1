namespace ThanTai.Models
{
    public class DanhGiaSanPham
    {
        public int ID { get; set; }
        public int SanPhamID { get; set; }
        public int NguoiDungID { get; set; }
        public int SoSao { get; set; }
        public string BinhLuan { get; set; }
        public DateTime NgayDanhGia { get; set; } = DateTime.Now;
        public SanPham SanPham { get; set; }
        public NguoiDung NguoiDung { get; set; }
    }
}
