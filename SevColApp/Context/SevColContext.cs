using Microsoft.EntityFrameworkCore;
using SevColApp.Helpers;
using SevColApp.Models;
using System.Linq;

namespace SevColApp.Context
{
    public class SevColContext : DbContext
    {
        public SevColContext(DbContextOptions<SevColContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Colony> Colonies { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<StockExchangeBuyRequest> StockExchangeBuyRequests { get; set; }
        public DbSet<StockExchangeSellRequest> StockExchangeSellRequests { get; set; }
        public DbSet<StockExchangeCompleted> StockExchangesCompleted { get; set; }
        public DbSet<UserCompanyStocks> UserCompanyStocks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }


            modelBuilder.Entity<Colony>().HasData(
                new Colony(1, "Earth"),
                new Colony(2, "Luna"),
                new Colony(3, "Mars"),
                new Colony(4, "Jupiter"),
                new Colony(5, "Saturn"),
                new Colony(6, "Eden and Kordoss"),
                new Colony(7, "The Worlds of Light"),
                new Colony(8, "SevCol")
                );

            modelBuilder.Entity<Bank>().HasData(
                new Bank(1, "Earth Financial Group", "EFG", 1),
                new Bank(2, "Monetary Optimisation Unit", "MOU", 2),
                new Bank(3, "Bank of Mars", "BOM", 3),
                new Bank(4, "Endeavour for Financing", "EFF", 4),
                new Bank(5, "Technically Evol Bank", "TEB", 4),
                new Bank(6, "Green Foundation", "GRF", 4),
                new Bank(7, "Wells-Morgan", "WEM", 4),
                new Bank(8, "Union Faction Bank", "UFB", 4),
                new Bank(9, "Saturnians Abroad Support", "SAS", 5),
                new Bank(10, "Rock Steady Finance", "RSF", 6),
                new Bank(11, "Sock Drawer Bank", "SDB", 6),
                new Bank(12, "The Enlightened Bank", "TEB", 7),
                new Bank(13, "SevCol Bank", "SCB", 8)
                );

            modelBuilder.Entity<User>().HasData(
                new User() { Id = 7777777, LoginName = "GameMaster",FirstName = "SevCol", Prefixes = "Game", LastName = "Master", PasswordHash = PasswordHelper.GetPasswordHash("ForRealsies") });
        }
    }
}
