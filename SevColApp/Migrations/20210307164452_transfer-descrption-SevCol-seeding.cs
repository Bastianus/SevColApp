using Microsoft.EntityFrameworkCore.Migrations;

namespace SevColApp.Migrations
{
    public partial class transferdescrptionSevColseeding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Transfers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Colonies",
                columns: new[] { "Id", "Name" },
                values: new object[] { 8, "SevCol" });

            migrationBuilder.InsertData(
                table: "Banks",
                columns: new[] { "Id", "Abbreviation", "ColonyId", "Name" },
                values: new object[] { 13, "SCB", 8, "SevCol Bank" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Banks",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Colonies",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Transfers");
        }
    }
}
