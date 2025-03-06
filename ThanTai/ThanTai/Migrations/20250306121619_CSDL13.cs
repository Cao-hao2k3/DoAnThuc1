using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThanTai.Migrations
{
    /// <inheritdoc />
    public partial class CSDL13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoLuong",
                table: "GioHang");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SoLuong",
                table: "GioHang",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
