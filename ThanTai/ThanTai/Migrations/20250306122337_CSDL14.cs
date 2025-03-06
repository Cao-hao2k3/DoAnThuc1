using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThanTai.Migrations
{
    /// <inheritdoc />
    public partial class CSDL14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SoLuong",
                table: "GioHang",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TongTien",
                table: "DatHangChiTiet",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoLuong",
                table: "GioHang");

            migrationBuilder.DropColumn(
                name: "TongTien",
                table: "DatHangChiTiet");
        }
    }
}
