
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Mind.Entity.SecurityAccount;
using Mind.Entity.Tables;

namespace Mind.Context.Data.Security;

#nullable disable

public class MindContext : DbContext
{
    #region TABLES

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<LogMovements> LogMovements { get; set; }

    #endregion

    private readonly IConfiguration Configuration;

    public MindContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // connect to sqlite database
       options.UseSqlServer(Configuration.GetConnectionString("MindDatabase"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Global Filters

        modelBuilder.Entity<Customer>().HasQueryFilter(x => !x.Deferred);
        modelBuilder.Entity<Team>().HasQueryFilter(x => !x.Deferred);

        #endregion

        #region Default Values

        modelBuilder.Entity<Customer>().Property(x => x.Deferred).HasDefaultValue(false);
        modelBuilder.Entity<Team>().Property(x => x.Deferred).HasDefaultValue(false);

        #endregion

    }
}