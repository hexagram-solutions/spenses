using Microsoft.EntityFrameworkCore;
using Spenses.Resources.Relational.Models;

namespace Spenses.Resources.Relational;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
            return;

        optionsBuilder.UseSqlServer("Server=.;Database=Spenses;Trusted_Connection=True;Encrypt=False;");
    }

    public DbSet<Home> Homes => Set<Home>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //Sets table names for entities to their CLR type name, bypassing rule to use the name of the DbSet.
        foreach (var mutableEntityType in modelBuilder.Model.GetEntityTypes())
        {
            if (mutableEntityType.IsOwned())
                continue;

            mutableEntityType.SetTableName(mutableEntityType.ClrType.Name);
        }
    }
}
