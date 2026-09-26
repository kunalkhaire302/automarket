using AutoMarket.Application;
using AutoMarket.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoMarket.Api.Controllers;

[Route("api/v1")]
public sealed class CatalogController(CatalogService catalog, IStore store) : BaseController
{
    [HttpGet("cars")]
    public async Task<object> Cars([FromQuery] SearchRequest r, CancellationToken ct)
    {
        var p = await catalog.Search(r, ct);
        return Envelope(p.Items, new { p.Page, p.PageSize, p.TotalItems, p.TotalPages, p.HasNext });
    }
    [HttpGet("cars/{id:guid}")]
    public async Task<object> Car(Guid id, CancellationToken ct) => Envelope(await catalog.Detail(id, ct));
    [HttpGet("search/suggestions")]
    public async Task<object> Suggestions([FromQuery] string q, [FromQuery] Guid? cityId, CancellationToken ct)
    {
        var result = await catalog.Search(new() { Q = q, CityId = cityId, PageSize = 8 }, ct);
        return Envelope(result.Items.Select(c => new { c.Id, text = $"{c.Brand} {c.Model}", c.City }).DistinctBy(x => x.text));
    }
    [HttpGet("locations/cities")]
    public async Task<object> Cities(CancellationToken ct) => Envelope(await store.List(store.Query<City>().Where(c => c.IsActive).OrderBy(c => c.Name)
        .Select(c => new { c.Id, c.Name, c.State, c.Latitude, c.Longitude, c.ServiceRadiusKm }), ct));
    [HttpGet("locations/centers")]
    public async Task<object> Centers([FromQuery] Guid cityId, CancellationToken ct) => Envelope(await store.List(store.Query<ServiceCenter>().Where(c => c.CityId == cityId)
        .OrderBy(c => c.Name).Select(c => new { c.Id, c.Name, c.Address, c.Kind, c.Latitude, c.Longitude }), ct));
    [HttpGet("cars/{id:guid}/pricing")]
    public async Task<object> Pricing(Guid id, CancellationToken ct)
    {
        var car = await catalog.Detail(id, ct);
        var now = DateTimeOffset.UtcNow;
        var rules = await store.List(store.Query<PricingRule>().Where(r => r.CityId == car.CityId && (r.BodyType == "" || r.BodyType == car.BodyType) && r.StartsAt <= now && r.EndsAt > now).OrderBy(r => r.Id), ct);
        return Envelope(new { listingPrice = car.Price, recommendedPrice = Rules.RecommendedPrice(car.Price, rules.Select(r => r.Multiplier)),
            basis = "Listing price with configured regional and seasonal adjustments", method = "deterministic-rules-v1", estimated = true,
            marketDataAvailable = rules.Count > 0, calculatedAt = now, drivers = rules.Select(r => new { r.Name, r.Multiplier, r.Kind }) });
    }
    [HttpGet("cars/{id:guid}/maintenance")]
    public async Task<object> Maintenance(Guid id, CancellationToken ct)
    {
        var car = await catalog.Detail(id, ct);
        var p = await store.First(store.Query<MaintenanceProfile>().Where(p => p.Brand == car.Brand && p.Model == car.Model).OrderByDescending(p => p.CreatedAt), ct);
        if (p is null) return Envelope(new { status = "INSUFFICIENT_DATA", message = "No approved maintenance reference is available for this model." });
        var high = DateTime.UtcNow.Year - car.ManufacturingYear >= p.HighMaintenanceAge && car.Kilometers >= p.HighMaintenanceKm;
        var annual = (p.ServiceCost * decimal.Ceiling(10000m / p.ServiceIntervalKm) + p.AnnualConsumablesCost) * (high ? p.OlderVehicleMultiplier : 1);
        return Envelope(new { status = "ESTIMATE", annual, monthly = decimal.Round(annual / 12, 2), category = high ? "High maintenance expected" : "Routine maintenance expected",
            nextServiceInKm = p.ServiceIntervalKm - car.Kilometers % p.ServiceIntervalKm, assumedAnnualKm = 10000,
            p.Version, p.Source, disclaimer = "Planning estimate, not a mechanical diagnosis. Actual service history may change the next service date." });
    }
    [Authorize, HttpGet("favourites")]
    public Task<object> Favourites(CancellationToken ct) => Saved("FAVOURITE", ct);
    [Authorize, HttpGet("compare")]
    public Task<object> Compare(CancellationToken ct) => Saved("COMPARE", ct);
    private async Task<object> Saved(string kind, CancellationToken ct)
    {
        var ids = store.Query<SavedCar>().Where(s => s.UserId == Actor && s.Kind == kind).Select(s => s.CarId);
        return Envelope(await store.List(catalog.Project(store.Query<Car>().Where(c => ids.Contains(c.Id) && (c.Status == "LISTED" || c.Status == "BOOKED"))), ct));
    }
    [Authorize, HttpPost("favourites/{id:guid}")]
    public Task<object> SaveFavourite(Guid id, CancellationToken ct) => Save(id, "FAVOURITE", ct);
    [Authorize, HttpPost("compare/{id:guid}")]
    public Task<object> SaveCompare(Guid id, CancellationToken ct) => Save(id, "COMPARE", ct);
    private Task<object> Save(Guid id, string kind, CancellationToken ct) => store.Transaction(async () =>
    {
        await catalog.Detail(id, ct);
        var q = store.Query<SavedCar>().Where(s => s.UserId == Actor && s.Kind == kind);
        if (await store.First(q.Where(s => s.CarId == id), ct) is not null) return Envelope(new { saved = true });
        Rules.Require(kind != "COMPARE" || await store.Count(q, ct) < 4, "Compare up to four cars. Remove one before adding another.");
        store.Add(new SavedCar { UserId = Actor, CarId = id, Kind = kind });
        await store.Save(ct);
        return Envelope(new { saved = true });
    }, ct);
    [Authorize, HttpDelete("favourites/{id:guid}")]
    public Task<object> DeleteFavourite(Guid id, CancellationToken ct) => DeleteSaved(id, "FAVOURITE", ct);
    [Authorize, HttpDelete("compare/{id:guid}")]
    public Task<object> DeleteCompare(Guid id, CancellationToken ct) => DeleteSaved(id, "COMPARE", ct);
    private async Task<object> DeleteSaved(Guid id, string kind, CancellationToken ct)
    {
        var row = await store.First(store.Query<SavedCar>().Where(s => s.UserId == Actor && s.CarId == id && s.Kind == kind), ct);
        if (row is not null) { store.Remove(row); await store.Save(ct); }
        return Envelope(new { saved = false });
    }
}
