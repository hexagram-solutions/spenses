using Microsoft.EntityFrameworkCore;
using Spenses.Resources.Relational.DigestModels;
using Spenses.Resources.Relational.Infrastructure;
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

    public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();

    public DbSet<Expense> Expenses => Set<Expense>();

    public DbSet<Home> Homes => Set<Home>();

    public DbSet<Member> Members => Set<Member>();

    public DbSet<Payment> Payments => Set<Payment>();

    public DbSet<UserIdentity> Users => Set<UserIdentity>();

    public DbSet<PaymentDigest> PaymentDigests => Set<PaymentDigest>();

    public DbSet<ExpenseDigest> ExpenseDigests => Set<ExpenseDigest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureTableNames(modelBuilder);
        ConfigureAuditingNavigationProperties(modelBuilder);

        modelBuilder.Entity<Expense>()
            .HasOne(x => x.PaidByMember)
            .WithMany(x => x.ExpensesPaid)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Expense>()
            .HasOne(x => x.Category)
            .WithMany(x => x.Expenses)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ExpenseShare>()
            .HasOne(x => x.OwedByMember)
            .WithMany(x => x.ExpenseShares)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ExpenseTag>()
            .HasKey(e => new { e.Name, e.ExpenseId });

        modelBuilder.Entity<Payment>()
            .HasOne(x => x.PaidByMember)
            .WithMany(x => x.PaymentsPaid)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
            .HasOne(x => x.PaidToMember)
            .WithMany(x => x.PaymentsReceived)
            .OnDelete(DeleteBehavior.Restrict);

        ConfigureDigestModel<PaymentDigest>(modelBuilder);
        ConfigureDigestModel<ExpenseDigest>(modelBuilder);
    }

    private static void ConfigureTableNames(ModelBuilder modelBuilder)
    {
        //Sets table names for entities to their CLR type name, bypassing rule to use the name of the DbSet.
        foreach (var mutableEntityType in modelBuilder.Model.GetEntityTypes())
        {
            if (mutableEntityType.IsOwned())
                continue;

            mutableEntityType.SetTableName(mutableEntityType.ClrType.Name);
        }
    }

    private static void ConfigureAuditingNavigationProperties(ModelBuilder modelBuilder)
    {
        var auditableNavigationProperties = typeof(UserIdentity).GetProperties()
            .Where(np => np.PropertyType.IsGenericType &&
                np.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>) &&
                np.PropertyType.GenericTypeArguments.Single().IsSubclassOf(typeof(AggregateRoot)));

        modelBuilder.Entity<UserIdentity>(builder =>
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
