using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EgorSalahovSemestrovka22.Migrations
{
    /// <inheritdoc />
    public partial class FixOrderStudentIdToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_StudentId1",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_StudentId1",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "StudentId1",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                column: "StudentId",
                value: "021914cc-ba3c-4bf5-aa53-6fc9bb467f1a");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_StudentId",
                table: "Orders",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_StudentId",
                table: "Orders",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_StudentId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_StudentId",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "StudentId1",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "StudentId", "StudentId1" },
                values: new object[] { 1, null });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_StudentId1",
                table: "Orders",
                column: "StudentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_StudentId1",
                table: "Orders",
                column: "StudentId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
