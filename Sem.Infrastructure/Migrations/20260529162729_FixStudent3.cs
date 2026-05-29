using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EgorSalahovSemestrovka22.Migrations
{
    /// <inheritdoc />
    public partial class FixStudent3 : Migration
    {
        /// <inheritdoc />
            protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Принудительно заполняем все NULL значения ID реального пользователя
            migrationBuilder.Sql("UPDATE dbo.Orders SET StudentId = '103303d3-9f50-41aa-89c6-7fc36ea290db' WHERE StudentId IS NULL;");

            // Дальше идет ваш стандартный код миграции, генерируемый EF...
            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: false, // Теперь база не будет ругаться, так как NULL больше нет!
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            // Код добавления внешнего ключа, который раньше падал:
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

        }
    }
}
