using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Spenses.Resources.Relational.DigestModels;
using Spenses.Resources.Relational.Infrastructure;
using Spenses.Resources.Relational.Models;

namespace Spenses.Resources.Relational;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
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

    public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();

    public DbSet<Expense> Expenses => Set<Expense>();

    public DbSet<Home> Homes => Set<Home>();

    public DbSet<Member> Members => Set<Member>();

    public DbSet<Payment> Payments => Set<Payment>();

    public DbSet<PaymentDigest> PaymentDigests => Set<PaymentDigest>();

    public DbSet<ExpenseDigest> ExpenseDigests => Set<ExpenseDigest>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureTableNames(builder);
        ConfigureAuditingNavigationProperties(builder);

        builder.Entity<Expense>()
            .HasOne(x => x.PaidByMember)
            .WithMany(x => x.ExpensesPaid)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Expense>()
            .HasOne(x => x.Category)
            .WithMany(x => x.Expenses)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ExpenseShare>()
            .HasOne(x => x.OwedByMember)
            .WithMany(x => x.ExpenseShares)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ExpenseTag>()
            .HasKey(e => new { e.Name, e.ExpenseId });

        builder.Entity<Payment>()
            .HasOne(x => x.PaidByMember)
            .WithMany(x => x.PaymentsPaid)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Payment>()
            .HasOne(x => x.PaidToMember)
            .WithMany(x => x.PaymentsReceived)
            .OnDelete(DeleteBehavior.Restrict);

        ConfigureDigestModel<PaymentDigest>(builder);
        ConfigureDigestModel<ExpenseDigest>(builder);
    }

    private static void ConfigureTableNames(ModelBuilder modelBuilder)
    {
        var entityTypes = modelBuilder.Model.GetEntityTypes()
            .Where(met => !met.ClrType.Namespace!.Contains("Microsoft.AspNetCore.Identity"));

        //Sets table names for entities to their CLR type name, bypassing rule to use the name of the DbSet.
        foreach (var mutableEntityType in entityTypes)
        {
            if (mutableEntityType.IsOwned())
                continue;

            mutableEntityType.SetTableName(mutableEntityType.ClrType.Name);
        }
    }

    private static void ConfigureAuditingNavigationProperties(ModelBuilder modelBuilder)
    {
        var auditableNavigationProperties = typeof(ApplicationUser).GetProperties()
            .Where(np => np.PropertyType.IsGenericType &&
                np.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>) &&
                np.PropertyType.GenericTypeArguments.Single().IsSubclassOf(typeof(AggregateRoot)));

        modelBuilder.Entity<ApplicationUser>(builder =>
        {
            foreach (var navigationProperty in auditableNavigationProperties)
            {
                var incomingNavigationPropertyName = navigationProperty.Name.Contains("Created")
                    ? nameof(AggregateRoot.CreatedBy)
                    : nameof(AggregateRoot.ModifiedBy);

                builder.HasMany(navigationProperty.Name)
                    .WithOne(incomingNavigationPropertyName)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        });
    }

    private static void ConfigureDigestModel<TDigest>(ModelBuilder modelBuilder)
        where TDigest : class
    {
        modelBuilder.Entity<TDigest>()
            // We map digests to views, but we have to use .ToTable() here so we can exclude the view from migrations
            .ToTable(typeof(TDigest).Name, t => t.ExcludeFromMigrations())
            .HasNoKey();
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
