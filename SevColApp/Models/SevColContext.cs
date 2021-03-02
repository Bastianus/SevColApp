using Microsoft.EntityFrameworkCore;

namespace SevColApp.Models
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Colony>().HasData(
                new Colony(1, "Earth"),
                new Colony(2, "Luna"),
                new Colony(3, "Mars"),
                new Colony(4, "Jupiter"),
                new Colony(5, "Saturn"),
                new Colony(6, "Eden and Kordoss"),
                new Colony(7, "The Worlds of Light")
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
                new Bank(10, "Pure Money Banking", "PMB", 6),
                new Bank(11, "Sock Drawer Bank", "SDB", 6),
                new Bank(12, "The Enlightened Bank", "TEB", 7)
                );
        }
    }
}
