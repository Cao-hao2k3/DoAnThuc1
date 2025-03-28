﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThanTai.Migrations
{
    /// <inheritdoc />
    public partial class CSDL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoaiSanPham",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tenloai = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiSanPham", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoVaTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Quyen = table.Column<bool>(type: "bit", nullable: false),
                    Anh = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TinhTrang",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TinhTrang", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SanPham",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoaiSanPhamID = table.Column<int>(type: "int", nullable: false),
                    TenSanPham = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DonGia = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MoTa = table.Column<string>(type: "ntext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanPham", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SanPham_LoaiSanPham_LoaiSanPhamID",
                        column: x => x.LoaiSanPhamID,
                        principalTable: "LoaiSanPham",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DatHang",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDungID = table.Column<int>(type: "int", nullable: false),
                    TinhTrangID = table.Column<int>(type: "int", nullable: false),
                    DienThoaiGiaoHang = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DiaChiGiaoHang = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NgayDatHang = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatHang", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DatHang_NguoiDung_NguoiDungID",
                        column: x => x.NguoiDungID,
                        principalTable: "NguoiDung",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DatHang_TinhTrang_TinhTrangID",
                        column: x => x.TinhTrangID,
                        principalTable: "TinhTrang",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DatHangChiTiet",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatHangID = table.Column<int>(type: "int", nullable: false),
                    SanPhamID = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<short>(type: "smallint", nullable: false),
                    DonGia = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatHangChiTiet", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DatHangChiTiet_DatHang_DatHangID",
                        column: x => x.DatHangID,
                        principalTable: "DatHang",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DatHangChiTiet_SanPham_SanPhamID",
                        column: x => x.SanPhamID,
                        principalTable: "SanPham",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DatHang_NguoiDungID",
                table: "DatHang",
                column: "NguoiDungID");

            migrationBuilder.CreateIndex(
                name: "IX_DatHang_TinhTrangID",
                table: "DatHang",
                column: "TinhTrangID");

            migrationBuilder.CreateIndex(
                name: "IX_DatHangChiTiet_DatHangID",
                table: "DatHangChiTiet",
                column: "DatHangID");

            migrationBuilder.CreateIndex(
                name: "IX_DatHangChiTiet_SanPhamID",
                table: "DatHangChiTiet",
                column: "SanPhamID");

            migrationBuilder.CreateIndex(
                name: "IX_SanPham_LoaiSanPhamID",
                table: "SanPham",
                column: "LoaiSanPhamID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatHangChiTiet");

            migrationBuilder.DropTable(
                name: "DatHang");

            migrationBuilder.DropTable(
                name: "SanPham");

            migrationBuilder.DropTable(
                name: "NguoiDung");

            migrationBuilder.DropTable(
                name: "TinhTrang");

            migrationBuilder.DropTable(
                name: "LoaiSanPham");
        }
    }
}
