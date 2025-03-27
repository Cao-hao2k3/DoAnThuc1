using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThanTai.Migrations
{
    /// <inheritdoc />
    public partial class CSDL32 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoaiSanPhamID",
                table: "ThuocTinh",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ThuocTinh_LoaiSanPhamID",
                table: "ThuocTinh",
                column: "LoaiSanPhamID");

            migrationBuilder.AddForeignKey(
                name: "FK_ThuocTinh_LoaiSanPham_LoaiSanPhamID",
                table: "ThuocTinh",
                column: "LoaiSanPhamID",
                principalTable: "LoaiSanPham",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThuocTinh_LoaiSanPham_LoaiSanPhamID",
                table: "ThuocTinh");

            migrationBuilder.DropIndex(
                name: "IX_ThuocTinh_LoaiSanPhamID",
                table: "ThuocTinh");

            migrationBuilder.DropColumn(
                name: "LoaiSanPhamID",
                table: "ThuocTinh");
        }
    }
}
