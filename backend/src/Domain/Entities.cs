namespace AutoMarket.Domain;

public abstract class Entity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
public sealed class User : Entity
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "BUYER";
    public bool EmailVerified { get; set; }
    public bool IsActive { get; set; } = true;
    public string ReferralCode { get; set; } = Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();
    public Guid? ReferrerId { get; set; }
}
public sealed class RefreshSession : Entity
{
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = "";
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
}
public sealed class City : Entity
{
    public string Name { get; set; } = "";
    public string State { get; set; } = "";
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double ServiceRadiusKm { get; set; } = 50;
    public bool IsActive { get; set; } = true;
}
public sealed class ServiceCenter : Entity
{
    public Guid CityId { get; set; }
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
    public string Kind { get; set; } = "INSPECTION";
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
public sealed class Car : Entity
{
    public Guid? SellerId { get; set; }
    public Guid CityId { get; set; }
    public string Brand { get; set; } = "";
    public string Model { get; set; } = "";
    public string Variant { get; set; } = "";
    public int ManufacturingYear { get; set; }
    public int RegistrationYear { get; set; }
    public int Kilometers { get; set; }
    public int OwnershipCount { get; set; } = 1;
    public string FuelType { get; set; } = "PETROL";
    public string Transmission { get; set; } = "MANUAL";
    public string BodyType { get; set; } = "HATCHBACK";
    public decimal Price { get; set; }
    public string Description { get; set; } = "";
    public string Status { get; set; } = "DRAFT";
    // Internal reference used for safely rerunnable partner/demo inventory imports; never exposed as a public listing identifier.
    public string? InventoryKey { get; set; }
    public bool IsFeatured { get; set; }
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
public sealed class CarImage : Entity
{
    public Guid CarId { get; set; }
    public string StorageKey { get; set; } = "";
    // Optional public image and provenance fields. Demo inventory uses Commons-hosted, licensed photos.
    public string? PublicUrl { get; set; }
    public string? SourceUrl { get; set; }
    public string? Attribution { get; set; }
    public string ImageType { get; set; } = "EXTERIOR";
    public int SortOrder { get; set; }
}
public sealed class SellerSubmission : Entity
{
    public Guid SellerId { get; set; }
    public Guid CarId { get; set; }
    public string Status { get; set; } = "DRAFT";
    public string ReviewNotes { get; set; } = "";
    public DateTimeOffset? SubmittedAt { get; set; }
    public Guid? ReviewedBy { get; set; }
}
public sealed class Appointment : Entity
{
    public Guid UserId { get; set; }
    public Guid CarId { get; set; }
    public Guid ServiceCenterId { get; set; }
    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }
    public string AppointmentType { get; set; } = "VIEWING";
    public string Status { get; set; } = "PENDING";
    public string Notes { get; set; } = "";
}
public sealed class Booking : Entity
{
    public Guid UserId { get; set; }
    public Guid CarId { get; set; }
    public Guid AppointmentId { get; set; }
    public string BookingReference { get; set; } = "AM-" + Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();
    public string Status { get; set; } = "PENDING";
    public decimal Amount { get; set; }
    public string IdempotencyKey { get; set; } = "";
}
public sealed class SavedCar : Entity
{
    public Guid UserId { get; set; }
    public Guid CarId { get; set; }
    public string Kind { get; set; } = "FAVOURITE";
}
public sealed class Notification : Entity
{
    public Guid UserId { get; set; }
    public string Type { get; set; } = "";
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
    public string DeepLink { get; set; } = "/profile";
    public string EventKey { get; set; } = "";
    public DateTimeOffset? ReadAt { get; set; }
}
public sealed class NotificationPreference : Entity
{
    public Guid UserId { get; set; }
    public string Category { get; set; } = "MARKETING";
    public bool InApp { get; set; } = true;
    public bool Push { get; set; }
}
public sealed class Referral : Entity
{
    public Guid ReferrerId { get; set; }
    public Guid ReferredUserId { get; set; }
    public string CodeSnapshot { get; set; } = "";
    public string Status { get; set; } = "ATTRIBUTED";
    public Guid? QualifyingBookingId { get; set; }
    public DateTimeOffset? QualifiedAt { get; set; }
}
public sealed class WalletTransaction : Entity
{
    public Guid UserId { get; set; }
    public int Points { get; set; }
    public string Type { get; set; } = "REWARD";
    public string Status { get; set; } = "POSTED";
    public string EventKey { get; set; } = "";
    public Guid? ReferenceId { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
}
public sealed class PricingRule : Entity
{
    public Guid CityId { get; set; }
    public string BodyType { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal Multiplier { get; set; } = 1;
    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }
    public string Kind { get; set; } = "REGIONAL";
}
public sealed class MaintenanceProfile : Entity
{
    public string Brand { get; set; } = "";
    public string Model { get; set; } = "";
    public string Version { get; set; } = "";
    public string Source { get; set; } = "";
    public int ServiceIntervalKm { get; set; }
    public decimal ServiceCost { get; set; }
    public decimal AnnualConsumablesCost { get; set; }
    public decimal OlderVehicleMultiplier { get; set; } = 1;
    public int HighMaintenanceAge { get; set; } = 6;
    public int HighMaintenanceKm { get; set; } = 80000;
}
public sealed class AuditLog : Entity
{
    public Guid ActorUserId { get; set; }
    public string Action { get; set; } = "";
    public Guid EntityId { get; set; }
    public string EntityType { get; set; } = "";
}
