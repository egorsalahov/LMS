using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EgorSalahovSemestrovka22.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoLinkToLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VideoLink",
                table: "Lessons",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoLink",
                table: "Lessons");
        }
    }
}
