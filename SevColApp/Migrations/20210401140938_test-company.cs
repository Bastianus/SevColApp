using Microsoft.EntityFrameworkCore.Migrations;

namespace SevColApp.Migrations
{
    public partial class testcompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Name", "NumberOfStocks" },
                values: new object[] { 1, "TestCompany", 0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
