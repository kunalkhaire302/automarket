using System.Data;
using AutoMarket.Application;
using AutoMarket.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AutoMarket.Infrastructure;

public sealed class MarketplaceDb(DbContextOptions<MarketplaceDb> options) : DbContext(options), IStore
{
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.HasDefaultSchema("automarket");
        b.Entity<User>().HasIndex(x => x.Email).IsUnique();
        b.Entity<User>().HasIndex(x => x.ReferralCode).IsUnique();
        b.Entity<User>().HasOne<User>().WithMany().HasForeignKey(x => x.ReferrerId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<RefreshSession>().HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<RefreshSession>().HasIndex(x => x.TokenHash).IsUnique();
        b.Entity<City>().HasIndex(x => new { x.Name, x.State }).IsUnique();
        b.Entity<ServiceCenter>().HasOne<City>().WithMany().HasForeignKey(x => x.CityId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Car>().HasOne<User>().WithMany().HasForeignKey(x => x.SellerId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Car>().HasOne<City>().WithMany().HasForeignKey(x => x.CityId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Car>().HasIndex(x => new { x.Status, x.CityId, x.Price });
        b.Entity<Car>().HasIndex(x => new { x.Brand, x.Model });
        b.Entity<Car>().HasIndex(x => x.CreatedAt);
        b.Entity<Car>().HasIndex(x => x.InventoryKey).IsUnique().HasFilter("inventory_key IS NOT NULL");
        b.Entity<Car>().ToTable(t => { t.HasCheckConstraint("car_positive_price", "price > 0"); t.HasCheckConstraint("car_valid_usage", "kilometers >= 0 AND ownership_count >= 1"); });
        b.Entity<CarImage>().HasOne<Car>().WithMany().HasForeignKey(x => x.CarId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<CarImage>().HasIndex(x => x.StorageKey).IsUnique();
        b.Entity<SellerSubmission>().HasOne<Car>().WithMany().HasForeignKey(x => x.CarId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<SellerSubmission>().HasOne<User>().WithMany().HasForeignKey(x => x.SellerId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<SellerSubmission>().HasIndex(x => x.CarId).IsUnique();
        b.Entity<Appointment>().HasOne<Car>().WithMany().HasForeignKey(x => x.CarId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Appointment>().HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Appointment>().HasOne<ServiceCenter>().WithMany().HasForeignKey(x => x.ServiceCenterId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Appointment>().HasIndex(x => new { x.UserId, x.StartsAt });
        b.Entity<Appointment>().HasIndex(x => new { x.CarId, x.StartsAt }).IsUnique().HasFilter("\"status\" NOT IN ('CANCELLED', 'NO_SHOW')");
        b.Entity<Appointment>().ToTable(t => t.HasCheckConstraint("appointment_duration", "ends_at > starts_at"));
        b.Entity<Booking>().HasOne<Car>().WithMany().HasForeignKey(x => x.CarId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Booking>().HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Booking>().HasOne<Appointment>().WithMany().HasForeignKey(x => x.AppointmentId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Booking>().HasIndex(x => x.BookingReference).IsUnique();
        b.Entity<Booking>().HasIndex(x => new { x.UserId, x.IdempotencyKey }).IsUnique();
        b.Entity<Booking>().HasIndex(x => x.CarId).IsUnique().HasFilter("\"status\" NOT IN ('CANCELLED', 'EXPIRED')");
        b.Entity<SavedCar>().HasOne<Car>().WithMany().HasForeignKey(x => x.CarId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<SavedCar>().HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<SavedCar>().HasIndex(x => new { x.UserId, x.CarId, x.Kind }).IsUnique();
        b.Entity<Notification>().HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Notification>().HasIndex(x => new { x.UserId, x.EventKey }).IsUnique();
        b.Entity<Notification>().HasIndex(x => new { x.UserId, x.ReadAt, x.CreatedAt });
        b.Entity<NotificationPreference>().HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<NotificationPreference>().HasIndex(x => new { x.UserId, x.Category }).IsUnique();
        b.Entity<Referral>().HasOne<User>().WithMany().HasForeignKey(x => x.ReferrerId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Referral>().HasOne<User>().WithMany().HasForeignKey(x => x.ReferredUserId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Referral>().HasOne<Booking>().WithMany().HasForeignKey(x => x.QualifyingBookingId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Referral>().HasIndex(x => x.ReferredUserId).IsUnique();
        b.Entity<Referral>().HasIndex(x => new { x.ReferrerId, x.Status });
        b.Entity<WalletTransaction>().HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<WalletTransaction>().HasIndex(x => x.EventKey).IsUnique();
        b.Entity<WalletTransaction>().HasIndex(x => new { x.UserId, x.Status, x.CreatedAt });
        b.Entity<WalletTransaction>().ToTable(t => t.HasCheckConstraint("wallet_nonzero_points", "points <> 0"));
        b.Entity<PricingRule>().HasOne<City>().WithMany().HasForeignKey(x => x.CityId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<PricingRule>().ToTable(t => t.HasCheckConstraint("pricing_rule_bounds", "multiplier BETWEEN 0.5 AND 1.5 AND ends_at > starts_at"));
        b.Entity<MaintenanceProfile>().HasIndex(x => new { x.Brand, x.Model, x.Version }).IsUnique();
        b.Entity<MaintenanceProfile>().ToTable(t => t.HasCheckConstraint("maintenance_positive_costs", "service_interval_km > 0 AND service_cost >= 0 AND annual_consumables_cost >= 0 AND older_vehicle_multiplier >= 1"));
        b.Entity<AuditLog>().HasOne<User>().WithMany().HasForeignKey(x => x.ActorUserId).OnDelete(DeleteBehavior.Restrict);
        foreach (var entity in b.Model.GetEntityTypes())
        {
            var singular = ToSnakeCase(entity.ClrType.Name);
            entity.SetTableName(singular.EndsWith('y') ? singular[..^1] + "ies" : singular + "s");
            foreach (var property in entity.GetProperties()) property.SetColumnName(ToSnakeCase(property.Name));
            foreach (var property in entity.GetProperties().Where(p => p.ClrType == typeof(decimal))) property.SetPrecision(18);
            foreach (var property in entity.GetProperties().Where(p => p.ClrType == typeof(decimal))) property.SetScale(2);
        }
        b.Entity<PricingRule>().Property(x => x.Multiplier).HasPrecision(8, 4);
        b.Entity<MaintenanceProfile>().Property(x => x.OlderVehicleMultiplier).HasPrecision(8, 4);
    }
    private static string ToSnakeCase(string text) => string.Concat(text.Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + char.ToLowerInvariant(c) : char.ToLowerInvariant(c).ToString()));
    public IQueryable<T> Query<T>() where T : Entity => Set<T>();
    public Task<List<T>> List<T>(IQueryable<T> query, CancellationToken ct) => query.ToListAsync(ct);
    public Task<T?> First<T>(IQueryable<T> query, CancellationToken ct) => query.FirstOrDefaultAsync(ct);
    public Task<int> Count<T>(IQueryable<T> query, CancellationToken ct) => query.CountAsync(ct);
    void IStore.Add<T>(T entity) => Set<T>().Add(entity);
    void IStore.Remove<T>(T entity) => Set<T>().Remove(entity);
    public async Task Save(CancellationToken ct) => await SaveChangesAsync(ct);
    public async Task<T> Transaction<T>(Func<Task<T>> action, CancellationToken ct)
    {
        await using var tx = await Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var result = await action();
        await tx.CommitAsync(ct);
        return result;
    }
}
