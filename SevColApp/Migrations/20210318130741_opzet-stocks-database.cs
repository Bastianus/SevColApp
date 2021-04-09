using Microsoft.EntityFrameworkCore.Migrations;

namespace SevColApp.Migrations
{
    public partial class opzetstocksdatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Banks_BankId",
                table: "BankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Users_userId",
                table: "BankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Banks_Colonies_ColonyId",
                table: "Banks");

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberOfStocks = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockExchangeBuyRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferPerStock = table.Column<int>(type: "int", nullable: false),
                    NumberOfStocks = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: false),
                    companyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockExchangeBuyRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockExchangeBuyRequests_Companies_companyId",
                        column: x => x.companyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockExchangeBuyRequests_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockExchangesCompleted",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumberOfStocks = table.Column<int>(type: "int", nullable: false),
                    AmountPerStock = table.Column<int>(type: "int", nullable: false),
                    companyId = table.Column<int>(type: "int", nullable: false),
                    sellerId = table.Column<int>(type: "int", nullable: false),
                    buyerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockExchangesCompleted", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockExchangesCompleted_Companies_companyId",
                        column: x => x.companyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockExchangesCompleted_Users_buyerId",
                        column: x => x.buyerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockExchangesCompleted_Users_sellerId",
                        column: x => x.sellerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockExchangeSellRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MinimumPerStock = table.Column<int>(type: "int", nullable: false),
                    NumberOfStocks = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: false),
                    companyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockExchangeSellRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockExchangeSellRequests_Companies_companyId",
                        column: x => x.companyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockExchangeSellRequests_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserCompanyStocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumberOfStocksOwned = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: false),
                    companyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCompanyStocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCompanyStocks_Companies_companyId",
                        column: x => x.companyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserCompanyStocks_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockExchangeBuyRequests_companyId",
                table: "StockExchangeBuyRequests",
                column: "companyId");

            migrationBuilder.CreateIndex(
                name: "IX_StockExchangeBuyRequests_userId",
                table: "StockExchangeBuyRequests",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_StockExchangesCompleted_buyerId",
                table: "StockExchangesCompleted",
                column: "buyerId");

            migrationBuilder.CreateIndex(
                name: "IX_StockExchangesCompleted_companyId",
                table: "StockExchangesCompleted",
                column: "companyId");

            migrationBuilder.CreateIndex(
                name: "IX_StockExchangesCompleted_sellerId",
                table: "StockExchangesCompleted",
                column: "sellerId");

            migrationBuilder.CreateIndex(
                name: "IX_StockExchangeSellRequests_companyId",
                table: "StockExchangeSellRequests",
                column: "companyId");

            migrationBuilder.CreateIndex(
                name: "IX_StockExchangeSellRequests_userId",
                table: "StockExchangeSellRequests",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCompanyStocks_companyId",
                table: "UserCompanyStocks",
                column: "companyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCompanyStocks_userId",
                table: "UserCompanyStocks",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Banks_BankId",
                table: "BankAccounts",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Users_userId",
                table: "BankAccounts",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Banks_Colonies_ColonyId",
                table: "Banks",
                column: "ColonyId",
                principalTable: "Colonies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Banks_BankId",
                table: "BankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Users_userId",
                table: "BankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Banks_Colonies_ColonyId",
                table: "Banks");

            migrationBuilder.DropTable(
                name: "StockExchangeBuyRequests");

            migrationBuilder.DropTable(
                name: "StockExchangesCompleted");

            migrationBuilder.DropTable(
                name: "StockExchangeSellRequests");

            migrationBuilder.DropTable(
                name: "UserCompanyStocks");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Banks_BankId",
                table: "BankAccounts",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Users_userId",
                table: "BankAccounts",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Banks_Colonies_ColonyId",
                table: "Banks",
                column: "ColonyId",
                principalTable: "Colonies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
