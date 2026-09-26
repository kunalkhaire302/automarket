using AutoMarket.Application;
using AutoMarket.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoMarket.Api.Controllers;

public sealed record CityRequest(string Name, string State, double Latitude, double Longitude, double ServiceRadiusKm);
public sealed record CenterRequest(Guid CityId, string Name, string Address, string Kind, double Latitude, double Longitude);

[Authorize(Roles = "ADMIN"), Route("api/v1/admin")]
public sealed class AdminController(IStore store, WorkflowService workflow, CatalogService catalog) : BaseController
{
    [HttpGet("cars")]
    public async Task<object> Cars(CancellationToken ct) => Envelope(await store.List(catalog.Project(store.Query<Car>().OrderByDescending(x => x.CreatedAt).Take(100)), ct));
    [HttpGet("sellers")]
    public async Task<object> Sellers(CancellationToken ct) => Envelope((await store.List(store.Query<SellerSubmission>().OrderByDescending(x => x.CreatedAt).Take(100), ct)).Select(WorkflowService.Map));
    [HttpGet("appointments")]
    public async Task<object> Appointments(CancellationToken ct) => Envelope((await store.List(store.Query<Appointment>().OrderByDescending(x => x.StartsAt).Take(100), ct)).Select(WorkflowService.Map));
    [HttpGet("bookings")]
    public async Task<object> Bookings(CancellationToken ct) => Envelope((await store.List(store.Query<Booking>().OrderByDescending(x => x.CreatedAt).Take(100), ct)).Select(WorkflowService.Map));
    [HttpGet("audit-logs")]
    public async Task<object> Audit(CancellationToken ct) => Envelope(await store.List(store.Query<AuditLog>().OrderByDescending(x => x.CreatedAt).Take(100).Select(x => new { x.Id, x.ActorUserId, x.Action, x.EntityId, x.EntityType, x.CreatedAt }), ct));
    [HttpPost("sellers/{id:guid}/review")]
    public async Task<object> Review(Guid id, StatusRequest r, CancellationToken ct) => Envelope(await workflow.ChangeSubmission(Actor, true, id, r, ct));
    [HttpPatch("appointments/{id:guid}")]
    public async Task<object> Appointment(Guid id, StatusRequest r, CancellationToken ct) => Envelope(await workflow.ChangeAppointment(Actor, true, id, r.Status, ct));
    [HttpPatch("bookings/{id:guid}")]
    public async Task<object> Booking(Guid id, StatusRequest r, CancellationToken ct) => Envelope(await workflow.ChangeBooking(Actor, true, id, r.Status, ct));
    [HttpPost("cities")]
    public async Task<object> City(CityRequest r, CancellationToken ct)
    {
        Rules.Require(!string.IsNullOrWhiteSpace(r.Name) && r.Name.Length <= 100 && r.State.Length <= 100, "Enter a valid city and state.");
        Rules.Require(double.IsFinite(r.Latitude) && double.IsFinite(r.Longitude) && r.Latitude is >= -90 and <= 90 && r.Longitude is >= -180 and <= 180 && r.ServiceRadiusKm is > 0 and <= 500, "Invalid service-area coordinates.");
        var city = new City { Name = r.Name.Trim(), State = r.State.Trim(), Latitude = r.Latitude, Longitude = r.Longitude, ServiceRadiusKm = r.ServiceRadiusKm };
        store.Add(city);
        store.Add(new AuditLog { ActorUserId = Actor, Action = "CITY_CREATED", EntityType = "City", EntityId = city.Id });
        await store.Save(ct);
        return Envelope(new { city.Id });
    }
    [HttpPost("centers")]
    public async Task<object> Center(CenterRequest r, CancellationToken ct)
    {
        Rules.Require(!string.IsNullOrWhiteSpace(r.Name) && r.Name.Length <= 100 && r.Address.Length <= 500, "Invalid center name or address.");
        Rules.Require(double.IsFinite(r.Latitude) && double.IsFinite(r.Longitude) && r.Latitude is >= -90 and <= 90 && r.Longitude is >= -180 and <= 180, "Invalid coordinates.");
        Rules.Require(new[] { "INSPECTION", "PICKUP", "HUB", "SERVICE" }.Contains(r.Kind), "Invalid center type.");
        Rules.Require(await store.First(store.Query<City>().Where(x => x.Id == r.CityId && x.IsActive), ct) is not null, "Select an active city.");
        var center = new ServiceCenter { CityId = r.CityId, Name = r.Name.Trim(), Address = r.Address.Trim(), Kind = r.Kind, Latitude = r.Latitude, Longitude = r.Longitude };
        store.Add(center);
        store.Add(new AuditLog { ActorUserId = Actor, Action = "CENTER_CREATED", EntityType = "ServiceCenter", EntityId = center.Id });
        await store.Save(ct);
        return Envelope(new { center.Id });
    }
}
