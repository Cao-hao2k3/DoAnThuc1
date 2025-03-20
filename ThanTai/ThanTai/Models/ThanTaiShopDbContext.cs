using Microsoft.EntityFrameworkCore;
using ThanTai.Models;

namespace ThanTai.Models
{
    public class ThanTaiShopDbContext : DbContext
    {
       public ThanTaiShopDbContext(DbContextOptions<ThanTaiShopDbContext> options) : base(options) { }

        public DbSet<LoaiSanPham> LoaiSanPham { get; set; }
        public DbSet<SanPham> SanPham { get; set; }

        public DbSet<DatHang> DatHang { get; set; }

        public DbSet<TinhTrang> TinhTrang { get; set; }

        public DbSet<DatHangChiTiet> DatHangChiTiet { get; set; }

        public DbSet<NguoiDung> NguoiDung { get; set; }
        public DbSet<ThuocTinh> ThuocTinh { get; set; }
        public DbSet<GiaTriThuocTinh> GiaTriThuocTinh { get; set; }
        public DbSet<GioHang> GioHang { get; set; } 
        public DbSet<KhuyenMai> KhuyenMai { get; set; }
        public DbSet<ThuongHieu> ThuongHieu { get; set; }
        public DbSet<BanTin> BanTin { get; set; }
        
        public DbSet<QuanLyKhoHang> QuanLyKhoHang { get; set; }
        public DbSet<ThanTai.Models.HinhAnhSanPham> HinhAnhSanPham { get; set; } = default!;
    }
}
