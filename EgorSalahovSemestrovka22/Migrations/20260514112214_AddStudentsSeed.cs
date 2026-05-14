using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EgorSalahovSemestrovka22.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentsSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "AvatarPath", "Bio", "DateOfBirth", "Email", "FirstName", "Gender", "LastName", "PhoneNumber", "RegistrationDate", "UserName" },
                values: new object[,]
                {
                    { 2, "student-2.png", "Frontend enthusiast", new DateTime(1999, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "maria@example.com", "Maria", "Female", "Sokolova", "+1234567891", new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "maria_dev" },
                    { 3, "student-3.png", "Backend developer", new DateTime(2001, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "alex@example.com", "Alexey", "Male", "Petrov", "+1234567892", new DateTime(2026, 1, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "alex_p" },
                    { 4, "student-4.png", "Fullstack learner", new DateTime(1998, 11, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "olga@example.com", "Olga", "Female", "Ivanova", "+1234567893", new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "olga_i" },
                    { 5, "student-5.png", "JavaScript fan", new DateTime(2002, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "dmitry@example.com", "Dmitry", "Male", "Kozlov", "+1234567894", new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "dmitry_k" },
                    { 6, "student-6.png", "React developer", new DateTime(1997, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "elena@example.com", "Elena", "Female", "Smirnova", "+1234567895", new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "elena_s" },
                    { 7, "student-7.png", "Python & C#", new DateTime(2000, 12, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "sergey@example.com", "Sergey", "Male", "Volkov", "+1234567896", new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "sergey_v" },
                    { 8, "student-8.png", "UI/UX designer", new DateTime(2001, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "anna2@example.com", "Anna", "Female", "Kuznetsova", "+1234567897", new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "anna_k" },
                    { 9, "student-9.png", "Game dev interested", new DateTime(1999, 8, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "pavel@example.com", "Pavel", "Male", "Morozov", "+1234567898", new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "pavel_m" },
                    { 10, "student-10.png", "Data Science student", new DateTime(2002, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "tatiana@example.com", "Tatiana", "Female", "Orlova", "+1234567899", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "tatiana_o" },
                    { 11, "student-11.png", "ASP.NET Core fan", new DateTime(1998, 6, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "nikolay@example.com", "Nikolay", "Male", "Fedorov", "+1234567810", new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "nikolay_f" },
                    { 12, "student-12.png", "Mobile developer", new DateTime(2000, 10, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "ekaterina@example.com", "Ekaterina", "Female", "Popova", "+1234567811", new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ekaterina_p" },
                    { 13, "student-13.png", "DevOps learner", new DateTime(2001, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "andrey@example.com", "Andrey", "Male", "Sidorov", "+1234567812", new DateTime(2026, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "andrey_s" },
                    { 14, "student-14.png", "QA Automation", new DateTime(1999, 7, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "yulia@example.com", "Yulia", "Female", "Vasilieva", "+1234567813", new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "yulia_v" },
                    { 15, "student-15.png", "Cloud computing", new DateTime(2002, 4, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "maxim@example.com", "Maxim", "Male", "Belov", "+1234567814", new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "maxim_b" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 15);
        }
    }
}
