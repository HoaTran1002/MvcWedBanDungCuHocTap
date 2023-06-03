using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcWedBanDungCuHocTap.Migrations
{
    /// <inheritdoc />
    public partial class InitialModels_x2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HinhAnhSP",
                table: "HinhAnhs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HinhAnhSP",
                table: "HinhAnhs");
        }
    }
}
