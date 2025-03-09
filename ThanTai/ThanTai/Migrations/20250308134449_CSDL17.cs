using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThanTai.Migrations
{
    /// <inheritdoc />
    public partial class CSDL17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DienThoaiGiaoHang",
                table: "DatHang");

            migrationBuilder.AlterColumn<decimal>(
                name: "GiaSauKhiGiam",
                table: "SanPham",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenNguoiDat",
                table: "DatHang",
                type: "nvarchar(225)",
                maxLength: 225,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(225)",
                oldMaxLength: 225,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DiaChiGiaoHang",
                table: "DatHang",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DienThoaiNguoiDat",
                table: "DatHang",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HinhThucThanhToan",
                table: "DatHang",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SoDienThoaiNguoiNhanKhac",
                table: "DatHang",
                type: "nvarchar(225)",
                maxLength: 225,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenNguoiNhanHangKhac",
                table: "DatHang",
                type: "nvarchar(225)",
                maxLength: 225,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DienThoaiNguoiDat",
                table: "DatHang");

            migrationBuilder.DropColumn(
                name: "HinhThucThanhToan",
                table: "DatHang");

            migrationBuilder.DropColumn(
                name: "SoDienThoaiNguoiNhanKhac",
                table: "DatHang");

            migrationBuilder.DropColumn(
                name: "TenNguoiNhanHangKhac",
                table: "DatHang");

            migrationBuilder.AlterColumn<decimal>(
                name: "GiaSauKhiGiam",
                table: "SanPham",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "TenNguoiDat",
                table: "DatHang",
                type: "nvarchar(225)",
                maxLength: 225,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(225)",
                oldMaxLength: 225);

            migrationBuilder.AlterColumn<string>(
                name: "DiaChiGiaoHang",
                table: "DatHang",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "DienThoaiGiaoHang",
                table: "DatHang",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }
    }
}
