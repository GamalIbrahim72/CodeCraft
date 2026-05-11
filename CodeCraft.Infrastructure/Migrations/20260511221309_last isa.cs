using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeCraft.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class lastisa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VideoUrl2",
                table: "Lessons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl3",
                table: "Lessons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoUrl2",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "VideoUrl3",
                table: "Lessons");
        }
    }
}
