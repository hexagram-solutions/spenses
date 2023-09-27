using Microsoft.EntityFrameworkCore;
using Spenses.Resources.Relational.Infrastructure;
using Spenses.Resources.Relational.Interceptors;
using Spenses.Resources.Relational.Models;

namespace Spenses.Resources.Relational;

public class ApplicationDbContext : DbContext
{
    private readonly AuditableEntitySaveChangesInterceptor? _auditableEntitySaveChangesInterceptor;

    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor)
        : base(options)
    {
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_auditableEntitySaveChangesInterceptor is not null)
            optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);

        if (optionsBuilder.IsConfigured)
            return;

        optionsBuilder.UseSqlServer("Server=.;Database=Spenses;Trusted_Connection=True;Encrypt=False;");
    }

    public DbSet<Expense> Expenses => Set<Expense>();

    public DbSet<Home> Homes => Set<Home>();

    public DbSet<Member> Members => Set<Member>();

    public DbSet<UserIdentity> Users => Set<UserIdentity>();

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

        //static void ConfigureUserIdentity<TEntity>(ModelBuilder modelBuilder)
        //    where TEntity : UserIdentity
        //{
        //    // get model properties by reflection
        //    var navigationProperties = typeof(UserIdentity).GetProperties()
        //        .Where(p => p.PropertyType == typeof(ICollection<>));

        //    modelBuilder.Entity<UserIdentity>(builder =>
        //    {
        //        foreach (var navigationProperty in navigationProperties)
        //        {
        //            var navigationPropertyEntity = navigationProperty.PropertyType.GetGenericArguments().Single();

        //            builder.HasMany(navigationProperty.Name)
        //                .WithOne(navigationPropertyEntity.Name)
        //                .OnDelete(DeleteBehavior.Restrict);
        //        }
        //    });
        //}

        foreach (var mutableEntityType in modelBuilder.Model.GetEntityTypes().Where(et => et.ClrType.BaseType == typeof(AggregateRoot)))
        {
            if (mutableEntityType.IsOwned())
                continue;

            mutableEntityType.SetTableName(mutableEntityType.ClrType.Name);
        }

        modelBuilder.Entity<Expense>()
            .HasOne(x => x.IncurredByMember)
            .WithMany(x => x.Expenses)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserIdentity>()
            .HasMany(x => x.CreatedExpenses)
            .WithOne(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserIdentity>()
            .HasMany(x => x.ModifiedMembers)
            .WithOne(x => x.ModifiedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserIdentity>()
            .HasMany(x => x.CreatedMembers)
            .WithOne(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserIdentity>()
            .HasMany(x => x.ModifiedMembers)
            .WithOne(x => x.ModifiedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserIdentity>()
            .HasMany(x => x.CreatedHomes)
            .WithOne(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserIdentity>()
            .HasMany(x => x.ModifiedHomes)
            .WithOne(x => x.ModifiedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateOnly>()
            .HaveConversion<DateOnlyConverter>()
            .HaveColumnType("date");

        configurationBuilder.Properties<DateOnly?>()
            .HaveConversion<NullableDateOnlyConverter>()
            .HaveColumnType("date");
    }
}
