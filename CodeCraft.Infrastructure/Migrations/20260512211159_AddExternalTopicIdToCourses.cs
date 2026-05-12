using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeCraft.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalTopicIdToCourses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalTopicId",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalTopicId",
                table: "Courses");
        }
    }
}
