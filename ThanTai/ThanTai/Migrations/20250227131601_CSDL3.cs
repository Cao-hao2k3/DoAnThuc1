using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThanTai.Migrations
{
    /// <inheritdoc />
    public partial class CSDL3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DonGia",
                table: "SanPham",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "LoaiSanPhamParentID",
                table: "SanPham",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "DienThoai",
                table: "NguoiDung",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DiaChi",
                table: "NguoiDung",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentID",
                table: "LoaiSanPham",
                type: "int",
                nullable: true);

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
                name: "DienThoaiGiaoHang",
                table: "DatHang",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "DiaChiGiaoHang",
                table: "DatHang",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.CreateIndex(
                name: "IX_LoaiSanPham_ParentID",
                table: "LoaiSanPham",
                column: "ParentID");

            migrationBuilder.AddForeignKey(
                name: "FK_LoaiSanPham_LoaiSanPham_ParentID",
                table: "LoaiSanPham",
                column: "ParentID",
                principalTable: "LoaiSanPham",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoaiSanPham_LoaiSanPham_ParentID",
                table: "LoaiSanPham");

            migrationBuilder.DropIndex(
                name: "IX_LoaiSanPham_ParentID",
                table: "LoaiSanPham");

            migrationBuilder.DropColumn(
                name: "LoaiSanPhamParentID",
                table: "SanPham");

            migrationBuilder.DropColumn(
                name: "ParentID",
                table: "LoaiSanPham");

            migrationBuilder.AlterColumn<int>(
                name: "DonGia",
                table: "SanPham",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "DienThoai",
                table: "NguoiDung",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "DiaChi",
                table: "NguoiDung",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

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
                name: "DienThoaiGiaoHang",
                table: "DatHang",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
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
        }
    }
}
