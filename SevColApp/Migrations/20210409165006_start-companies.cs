using Microsoft.EntityFrameworkCore.Migrations;

namespace SevColApp.Migrations
{
    public partial class startcompanies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Endeavour");

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Name", "NumberOfStocks" },
                values: new object[,]
                {
                    { 2, "Evol", 0 },
                    { 3, "In-Gentriment", 0 },
                    { 4, "OCP", 0 },
                    { 5, "UAC", 0 },
                    { 6, "Phalanx", 0 },
                    { 7, "PMC", 0 },
                    { 8, "Wendall's Guards", 0 },
                    { 9, "Hand of Eranon", 0 },
                    { 10, "Hypercity Trading Company", 0 },
                    { 11, "WEAK", 0 },
                    { 12, "New Luna Jones", 0 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "TestCompany");
        }
    }
}
