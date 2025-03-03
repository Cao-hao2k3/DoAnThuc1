using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThanTai.Migrations
{
    /// <inheritdoc />
    public partial class CSDL7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ThuongHieuID",
                table: "SanPham",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ThuongHieu",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenThuongHieu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThuongHieu", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SanPham_ThuongHieuID",
                table: "SanPham",
                column: "ThuongHieuID");

            migrationBuilder.AddForeignKey(
                name: "FK_SanPham_ThuongHieu_ThuongHieuID",
                table: "SanPham",
                column: "ThuongHieuID",
                principalTable: "ThuongHieu",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SanPham_ThuongHieu_ThuongHieuID",
                table: "SanPham");

            migrationBuilder.DropTable(
                name: "ThuongHieu");

            migrationBuilder.DropIndex(
                name: "IX_SanPham_ThuongHieuID",
                table: "SanPham");

            migrationBuilder.DropColumn(
                name: "ThuongHieuID",
                table: "SanPham");
        }
    }
}
