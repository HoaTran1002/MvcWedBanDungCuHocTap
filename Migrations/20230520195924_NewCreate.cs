using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcWedBanDungCuHocTap.Migrations
{
    /// <inheritdoc />
    public partial class NewCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id_HinhAnh",
                table: "SanPhams");

            migrationBuilder.AddColumn<int>(
                name: "IdSP",
                table: "HinhAnhs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdSP",
                table: "HinhAnhs");

            migrationBuilder.AddColumn<int>(
                name: "Id_HinhAnh",
                table: "SanPhams",
                type: "int",
                nullable: true);
        }
    }
}
