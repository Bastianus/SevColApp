using Microsoft.EntityFrameworkCore.Migrations;

namespace SevColApp.Migrations
{
    public partial class Stock_Exchange_database_variables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "CompanyTrendFactor",
                table: "Companies",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "CompanyVolatility",
                table: "Companies",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.CreateTable(
                name: "Global",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarketTrendFactor = table.Column<float>(type: "real", nullable: false),
                    MarketVolatility = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Global", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Global",
                columns: new[] { "Id", "MarketTrendFactor", "MarketVolatility" },
                values: new object[] { 1, 1f, 1f });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Global");

            migrationBuilder.DropColumn(
                name: "CompanyTrendFactor",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CompanyVolatility",
                table: "Companies");
        }
    }
}
