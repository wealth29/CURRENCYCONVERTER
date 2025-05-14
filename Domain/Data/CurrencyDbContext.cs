using Microsoft.EntityFrameworkCore;
using CurrencyExchange.Domain.Entities;
    public class CurrencyDbContext : DbContext
    {
        public CurrencyDbContext(DbContextOptions<CurrencyDbContext> options) : base(options) { }

        public DbSet<RealTimeRate> RealTimeRates => Set<RealTimeRate>();
        public DbSet<HistoricalRate> HistoricalRates => Set<HistoricalRate>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HistoricalRate>()
                .HasIndex(hr => new { hr.BaseCurrency, hr.TargetCurrency, hr.Date })
                .IsUnique();
        }
    }