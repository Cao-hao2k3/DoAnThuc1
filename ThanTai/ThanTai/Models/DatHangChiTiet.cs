﻿namespace ThanTai.Models
{
    public class DatHangChiTiet
    {
        public int ID { get; set; }
        public int DatHangID { get; set; }
        public int SanPhamID { get; set; }
        public short SoLuong { get; set; }
        public int DonGia { get; set; }

        public DatHang? DatHang { get; set; }
        public SanPham? SanPham { get; set; }
    }
}
