using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EgorSalahovSemestrovka22.Migrations
{
    /// <inheritdoc />
    public partial class ForRel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_StudentId",
                table: "Orders");

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1);

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
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "City", "Country", "FirstName", "LastName", "OrderDate", "OrderStatus", "PaymentMethod", "State", "StudentId", "Tax", "TotalAmount" },
                values: new object[] { 1, "Lenina st. 1", null, "Moscow", "Russia", "Ivan", "Tester", new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Completed", "Card", "MSK", "021914cc-ba3c-4bf5-aa53-6fc9bb467f1a", 10.00m, 150.00m });

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_StudentId",
                table: "Orders",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
