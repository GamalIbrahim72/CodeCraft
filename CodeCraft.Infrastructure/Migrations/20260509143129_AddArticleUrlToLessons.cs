using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeCraft.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddArticleUrlToLessons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArticleUrl",
                table: "Lessons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArticleUrl",
                table: "Lessons");
        }
    }
}
