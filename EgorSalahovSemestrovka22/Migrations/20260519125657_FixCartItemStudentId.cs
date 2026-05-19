using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EgorSalahovSemestrovka22.Migrations
{
    /// <inheritdoc />
    public partial class FixCartItemStudentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_AspNetUsers_StudentId1",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_StudentId1",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "StudentId1",
                table: "CartItems");

            migrationBuilder.AlterColumn<string>(
                name: "AvatarPath",
                table: "Instructors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "CartItems",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_StudentId",
                table: "CartItems",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_AspNetUsers_StudentId",
                table: "CartItems",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_AspNetUsers_StudentId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_StudentId",
                table: "CartItems");

            migrationBuilder.AlterColumn<string>(
                name: "AvatarPath",
                table: "Instructors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "CartItems",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "StudentId1",
                table: "CartItems",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_StudentId1",
                table: "CartItems",
                column: "StudentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_AspNetUsers_StudentId1",
                table: "CartItems",
                column: "StudentId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
