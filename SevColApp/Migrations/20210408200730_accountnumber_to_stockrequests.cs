using Microsoft.EntityFrameworkCore.Migrations;

namespace SevColApp.Migrations
{
    public partial class accountnumber_to_stockrequests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "StockExchangeSellRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "StockExchangeBuyRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "StockExchangeSellRequests");

            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "StockExchangeBuyRequests");
        }
    }
}
