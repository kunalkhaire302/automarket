using System.Linq.Expressions;
using System.Text.Json;
using AutoMarket.Domain;

namespace AutoMarket.Application;

public interface IStore
{
    IQueryable<T> Query<T>() where T : Entity;
    Task<List<T>> List<T>(IQueryable<T> query, CancellationToken ct);
    Task<T?> First<T>(IQueryable<T> query, CancellationToken ct);
    Task<int> Count<T>(IQueryable<T> query, CancellationToken ct);
    void Add<T>(T entity) where T : Entity;
    void Remove<T>(T entity) where T : Entity;
    Task Save(CancellationToken ct);
    Task<T> Transaction<T>(Func<Task<T>> action, CancellationToken ct);
}
public interface IIdentity
{
    string HashPassword(User user, string password);
    bool VerifyPassword(User user, string password);
    string AccessToken(User user);
    string NewToken();
    string HashToken(string token);
}
public interface IAiService
{
    Task<JsonElement> Models(CancellationToken ct);
    Task<JsonElement> Valuation(AiValuationRequest request, CancellationToken ct);
    Task<JsonElement> Recommendations(AiRecommendationPayload request, CancellationToken ct);
}
public sealed record SessionDto(string AccessToken, string RefreshToken, UserDto User);
public sealed record UserDto(Guid Id, string Name, string Email, string Phone, string Role, string ReferralCode);
public sealed record RegisterRequest(string Name, string Email, string Phone, string Password, string? ReferralCode);
public sealed record LoginRequest(string Email, string Password);
public sealed record TokenRequest(string RefreshToken);
public sealed record CarDto(Guid Id, string Brand, string Model, string Variant, int ManufacturingYear,
    int RegistrationYear, int Kilometers, int OwnershipCount, string FuelType, string Transmission,
    string BodyType, decimal Price, Guid CityId, string City, string State, string Description, string Status,
    bool IsFeatured, string? PrimaryImageUrl);
public sealed record CarImageDto(string ImageType, string Url, int SortOrder, string? SourceUrl = null, string? Attribution = null);
public sealed record CarDetailDto(Guid Id, string Brand, string Model, string Variant, int ManufacturingYear,
    int RegistrationYear, int Kilometers, int OwnershipCount, string FuelType, string Transmission,
    string BodyType, decimal Price, Guid CityId, string City, string State, string Description, string Status,
    bool IsFeatured, string? PrimaryImageUrl, IReadOnlyList<CarImageDto> Images);
public sealed class SearchRequest
{
    public string? Q { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? Variant { get; set; }
    public string? City { get; set; }
    public Guid? CityId { get; set; }
    public string? State { get; set; }
    public string? FuelType { get; set; }
    public string? Transmission { get; set; }
    public string? BodyType { get; set; }
    public int? OwnershipCount { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinYear { get; set; }
    public int? MaxYear { get; set; }
    public int? MinKilometers { get; set; }
    public int? MaxKilometers { get; set; }
    public bool? Featured { get; set; }
    public string Sort { get; set; } = "relevance";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
public sealed record PageResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalItems)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    public bool HasNext => Page < TotalPages;
}
public sealed record CarRequest(string Brand, string Model, string Variant, int ManufacturingYear,
    int RegistrationYear, int Kilometers, int OwnershipCount, string FuelType, string Transmission,
    string BodyType, decimal Price, Guid CityId, string Description);
public sealed record AppointmentRequest(Guid CarId, Guid ServiceCenterId, DateTimeOffset StartsAt, string AppointmentType, string Notes);
public sealed record BookingRequest(Guid CarId, Guid AppointmentId);
public sealed record StatusRequest(string Status, string Notes = "");
public sealed record RedemptionRequest(int Points, string IdempotencyKey);
public sealed record NotificationPreferenceRequest(string Category, bool InApp, bool Push);
public sealed record AiValuationRequest(string Brand, string Model, int RegistrationYear, int Kilometers, int OwnershipCount);
public sealed record AiRecommendationRequest(decimal? MaxPrice, Guid? CityId, string? FuelType, string? Transmission, string? BodyType, int Limit = 10);
public sealed record AiRecommendationCandidate(string CarId, bool Available, double BudgetFit, double PreferenceFit,
    double FeatureFit, double LocationFit, double Recency, double EngagementQuality);
public sealed record AiRecommendationPayload(IReadOnlyList<AiRecommendationCandidate> Candidates, int Limit);
public sealed record AiSemanticSearchRequest(string Query, Guid? CityId);
public sealed record WalletDto(int Balance, IReadOnlyList<WalletTransactionDto> Transactions);
public sealed record WalletTransactionDto(Guid Id, int Points, string Type, string Status, string EventKey, Guid? ReferenceId, DateTimeOffset CreatedAt, DateTimeOffset? ExpiresAt);
public sealed record AppointmentDto(Guid Id, Guid CarId, Guid ServiceCenterId, DateTimeOffset StartsAt,
    DateTimeOffset EndsAt, string AppointmentType, string Status, string Notes);
public sealed record BookingDto(Guid Id, Guid CarId, Guid AppointmentId, string BookingReference,
    string Status, decimal Amount, DateTimeOffset CreatedAt);
public sealed record SubmissionDto(Guid Id, Guid CarId, string Status, string ReviewNotes, DateTimeOffset? SubmittedAt);
