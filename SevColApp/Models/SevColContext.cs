using Microsoft.EntityFrameworkCore;

namespace SevColApp.Models
{
    public class SevColContext : DbContext
    {
        public SevColContext(DbContextOptions<SevColContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
