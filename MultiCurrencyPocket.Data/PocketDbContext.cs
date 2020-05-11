using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MultiCurrencyPocket.Data.Models;

namespace MultiCurrencyPocket.Data
{
    public partial class PocketDbContext : DbContext
    {
        public PocketDbContext(DbContextOptions<PocketDbContext> options) : base(options) 
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(local);Database=MultiCurrencyPocketDb;Trusted_connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SeedData(modelBuilder);
        }

        public DbSet<PocketHolder> Holders { get; set; }
        public DbSet<CurrencyAccount> CurrencyAccounts { get; set; }
        public DbSet<Currency> Currencies { get; set; }
    }
}
