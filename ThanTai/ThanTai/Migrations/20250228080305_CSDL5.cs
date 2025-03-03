using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThanTai.Migrations
{
    /// <inheritdoc />
    public partial class CSDL5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HinhAnh",
                table: "SanPham");

            migrationBuilder.DropColumn(
                name: "LoaiSanPhamParentID",
                table: "SanPham");

            migrationBuilder.CreateTable(
                name: "HinhAnhSanPham",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SanPhamID = table.Column<int>(type: "int", nullable: false),
                    AnhSanPham = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AnhThongSo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HinhAnhSanPham", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HinhAnhSanPham_SanPham_SanPhamID",
                        column: x => x.SanPhamID,
                        principalTable: "SanPham",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThuocTinh",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenThuocTinh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThuocTinh", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "GiaTriThuocTinh",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SanPhamID = table.Column<int>(type: "int", nullable: false),
                    ThuocTinhID = table.Column<int>(type: "int", nullable: false),
                    GiaTri = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiaTriThuocTinh", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GiaTriThuocTinh_SanPham_SanPhamID",
                        column: x => x.SanPhamID,
                        principalTable: "SanPham",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GiaTriThuocTinh_ThuocTinh_ThuocTinhID",
                        column: x => x.ThuocTinhID,
                        principalTable: "ThuocTinh",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GiaTriThuocTinh_SanPhamID",
                table: "GiaTriThuocTinh",
                column: "SanPhamID");

            migrationBuilder.CreateIndex(
                name: "IX_GiaTriThuocTinh_ThuocTinhID",
                table: "GiaTriThuocTinh",
                column: "ThuocTinhID");

            migrationBuilder.CreateIndex(
                name: "IX_HinhAnhSanPham_SanPhamID",
                table: "HinhAnhSanPham",
                column: "SanPhamID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GiaTriThuocTinh");

            migrationBuilder.DropTable(
                name: "HinhAnhSanPham");

            migrationBuilder.DropTable(
                name: "ThuocTinh");

            migrationBuilder.AddColumn<string>(
                name: "HinhAnh",
                table: "SanPham",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoaiSanPhamParentID",
                table: "SanPham",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
