using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThanTai.Migrations
{
    /// <inheritdoc />
    public partial class CSDL12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HinhThucGiaoHang",
                table: "DatHang",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HinhThucGiaoHang",
                table: "DatHang");
        }
    }
}
