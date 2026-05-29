using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EgorSalahovSemestrovka22.Migrations
{
    /// <inheritdoc />
    public partial class FixStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_StudentId",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.Sql("ALTER TABLE [dbo].[Orders] WITH NOCHECK ADD CONSTRAINT [FK_Orders_AspNetUsers_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[AspNetUsers] ([Id]);");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_StudentId",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_StudentId",
                table: "Orders",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
