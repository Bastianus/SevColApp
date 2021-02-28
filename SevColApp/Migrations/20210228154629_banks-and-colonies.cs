using Microsoft.EntityFrameworkCore.Migrations;

namespace SevColApp.Migrations
{
    public partial class banksandcolonies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Colonies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colonies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Banks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Abbreviation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ColonyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Banks_Colonies_ColonyId",
                        column: x => x.ColonyId,
                        principalTable: "Colonies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Colonies",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Earth" },
                    { 2, "Luna" },
                    { 3, "Mars" },
                    { 4, "Jupiter" },
                    { 5, "Saturn" },
                    { 6, "Eden and Kordoss" },
                    { 7, "The Worlds of Light" }
                });

            migrationBuilder.InsertData(
                table: "Banks",
                columns: new[] { "Id", "Abbreviation", "ColonyId", "Name" },
                values: new object[,]
                {
                    { 1, "EFG", 1, "Earth Financial Group" },
                    { 2, "MOU", 2, "Monetary Optimisation Unit" },
                    { 3, "BOM", 3, "Bank of Mars" },
                    { 4, "EFF", 4, "Endeavour for Financing" },
                    { 5, "TEB", 4, "Technically Evol Bank" },
                    { 6, "GRF", 4, "Green Foundation" },
                    { 7, "WEM", 4, "Wells-Morgan" },
                    { 8, "UFB", 4, "Union Faction Bank" },
                    { 9, "SAS", 5, "Saturnians Abroad Support" },
                    { 10, "PMB", 6, "Pure Money Banking" },
                    { 11, "SDB", 6, "Sock Drawer Bank" },
                    { 12, "TEB", 7, "The Enlightened Bank" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Banks_ColonyId",
                table: "Banks",
                column: "ColonyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Banks");

            migrationBuilder.DropTable(
                name: "Colonies");
        }
    }
}
