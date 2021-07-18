using Microsoft.EntityFrameworkCore.Migrations;

namespace SevColApp.Migrations
{
    public partial class company_stock_values_initial_set : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CompanyTrendFactor", "CompanyVolatility" },
                values: new object[] { 1f, 0.6f });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CompanyTrendFactor", "CompanyVolatility" },
                values: new object[] { 1f, 0.68f });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CompanyTrendFactor", "CompanyVolatility" },
                values: new object[] { 1f, 0.56f });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CompanyTrendFactor", "CompanyVolatility" },
                values: new object[] { 1f, 0.47f });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CompanyTrendFactor", "CompanyVolatility" },
                values: new object[] { 1f, 0.52f });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CompanyTrendFactor", "CompanyVolatility" },
                values: new object[] { 1f, 1.4f });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CompanyTrendFactor", "CompanyVolatility" },
                values: new object[] { 1f, 1.13f });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CompanyTrendFactor", "CompanyVolatility" },
                values: new object[] { 1f, 1.45f });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CompanyTrendFactor", "CompanyVolatility" },
                values: new object[] { 1f, 1.76f });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CompanyTrendFactor", "CompanyVolatility" },
                values: new object[] { 1f, 0.8f });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CompanyTrendFactor", "CompanyVolatility" },
                values: new object[] { 1f, 1.37f });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CompanyTrendFactor", "CompanyVolatility" },
                values: new object[] { 1f, 1.12f });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CompanyTrendFactor", "CompanyVolatility" },
                values: new object[] { 0f, 0f });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CompanyTrendFactor", "CompanyVolatility" },
                values: new object[] { 0f, 0f });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CompanyTrendFactor", "CompanyVolatility" },
                values: new object[] { 0f, 0f });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CompanyTrendFactor", "CompanyVolatility" },
                values: new object[] { 0f, 0f });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CompanyTrendFactor", "CompanyVolatility" },
                values: new object[] { 0f, 0f });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CompanyTrendFactor", "CompanyVolatility" },
                values: new object[] { 0f, 0f });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CompanyTrendFactor", "CompanyVolatility" },
                values: new object[] { 0f, 0f });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CompanyTrendFactor", "CompanyVolatility" },
                values: new object[] { 0f, 0f });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CompanyTrendFactor", "CompanyVolatility" },
                values: new object[] { 0f, 0f });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CompanyTrendFactor", "CompanyVolatility" },
                values: new object[] { 0f, 0f });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CompanyTrendFactor", "CompanyVolatility" },
                values: new object[] { 0f, 0f });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CompanyTrendFactor", "CompanyVolatility" },
                values: new object[] { 0f, 0f });
        }
    }
}
