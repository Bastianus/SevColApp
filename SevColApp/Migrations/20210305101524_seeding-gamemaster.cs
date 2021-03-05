using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SevColApp.Migrations
{
    public partial class seedinggamemaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transfers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayingAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceivingAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfers", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Banks",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Abbreviation", "Name" },
                values: new object[] { "RSF", "Rock Steady Finance" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "FirstName", "LastName", "LoginName", "PasswordHash", "Prefixes" },
                values: new object[] { 7777777, "SevCol", "Master", "GameMaster", new byte[] { 92, 108, 153, 233, 172, 89, 109, 109, 73, 34, 120, 114, 10, 78, 6, 6, 23, 244, 108, 223, 240, 91, 44, 24, 224, 247, 90, 97, 186, 156, 70, 239, 103, 78, 98, 107, 120, 0, 73, 97, 179, 112, 148, 154, 9, 251, 230, 73, 252, 109, 40, 116, 98, 159, 54, 98, 8, 32, 41, 41, 228, 151, 201, 183 }, "Game" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transfers");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7777777);

            migrationBuilder.UpdateData(
                table: "Banks",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Abbreviation", "Name" },
                values: new object[] { "PMB", "Pure Money Banking" });
        }
    }
}
