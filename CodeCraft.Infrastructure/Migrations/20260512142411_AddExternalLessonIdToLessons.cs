using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeCraft.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalLessonIdToLessons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalLessonId",
                table: "Lessons",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalLessonId",
                table: "Lessons");
        }
    }
}
