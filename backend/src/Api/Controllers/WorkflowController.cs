using AutoMarket.Application;
using AutoMarket.Api.Services;
using AutoMarket.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoMarket.Api.Controllers;

[Authorize, Route("api/v1")]
public sealed class WorkflowController(WorkflowService service, IStore store, SupabaseStorageClient storage) : BaseController
{
    private static readonly string[] NotificationCategories = ["APPOINTMENTS", "BOOKINGS", "PRICE_DROPS", "MESSAGES", "SELLER", "REWARDS", "MARKETING"];
    [HttpPost("appointments")]
    public async Task<IActionResult> Appointment(AppointmentRequest r, CancellationToken ct) => StatusCode(201, Envelope(await service.CreateAppointment(Actor, r, ct)));
    [HttpGet("appointments/me")]
    public async Task<object> Appointments(CancellationToken ct) => Envelope((await store.List(store.Query<Appointment>().Where(x => x.UserId == Actor).OrderByDescending(x => x.StartsAt).Take(100), ct)).Select(WorkflowService.Map));
    [HttpPost("appointments/{id:guid}/cancel")]
    public async Task<object> CancelAppointment(Guid id, CancellationToken ct) => Envelope(await service.ChangeAppointment(Actor, false, id, "CANCELLED", ct));
    [HttpPost("bookings")]
    public async Task<IActionResult> Booking(BookingRequest r, CancellationToken ct) => StatusCode(201, Envelope(await service.CreateBooking(Actor, r, Request.Headers["Idempotency-Key"].ToString(), ct)));
    [HttpGet("bookings/me")]
    public async Task<object> Bookings(CancellationToken ct) => Envelope((await store.List(store.Query<Booking>().Where(x => x.UserId == Actor).OrderByDescending(x => x.CreatedAt).Take(100), ct)).Select(WorkflowService.Map));
    [HttpPost("bookings/{id:guid}/cancel")]
    public async Task<object> CancelBooking(Guid id, CancellationToken ct) => Envelope(await service.ChangeBooking(Actor, false, id, "CANCELLED", ct));
    [HttpPost("seller/submissions")]
    public async Task<IActionResult> Submission(CarRequest r, CancellationToken ct) => StatusCode(201, Envelope(await service.CreateSubmission(Actor, r, ct)));
    [HttpPost("seller/submissions/{id:guid}/images"), RequestSizeLimit(7 * 1024 * 1024)]
    public async Task<IActionResult> UploadImage(Guid id, IFormFile? file, CancellationToken ct)
    {
        Rules.Require(file is not null, "Choose an image to upload.");
        var submission = await store.First(store.Query<SellerSubmission>().Where(x => x.Id == id && x.SellerId == Actor), ct)
            ?? throw new BusinessException("NOT_FOUND", "Submission not found.", 404);
        Rules.Require(submission.Status == "DRAFT", "Images can only be changed before submission.", "SUBMISSION_LOCKED", 409);
        var stored = await storage.UploadCarImage(submission.CarId, file!, ct);
        var sortOrder = await store.Count(store.Query<CarImage>().Where(x => x.CarId == submission.CarId), ct);
        store.Add(new CarImage { CarId = submission.CarId, StorageKey = stored.StorageKey, PublicUrl = stored.PublicUrl, ImageType = "EXTERIOR", SortOrder = sortOrder });
        await store.Save(ct);
        return StatusCode(201, Envelope(new { url = stored.PublicUrl, sortOrder }));
    }
    [HttpGet("seller/submissions")]
    public async Task<object> Submissions(CancellationToken ct) => Envelope((await store.List(store.Query<SellerSubmission>().Where(x => x.SellerId == Actor).OrderByDescending(x => x.CreatedAt).Take(100), ct)).Select(WorkflowService.Map));
    [HttpPost("seller/submissions/{id:guid}/submit")]
    public async Task<object> Submit(Guid id, CancellationToken ct) => Envelope(await service.ChangeSubmission(Actor, false, id, new("SUBMITTED"), ct));
    [HttpGet("notifications")]
    public async Task<object> Notifications(CancellationToken ct) => Envelope(await store.List(store.Query<Notification>().Where(x => x.UserId == Actor).OrderByDescending(x => x.CreatedAt).Take(100)
        .Select(x => new { x.Id, x.Type, x.Title, x.Message, x.DeepLink, x.ReadAt, x.CreatedAt }), ct));
    [HttpPost("notifications/{id:guid}/read")]
    public async Task<object> Read(Guid id, CancellationToken ct)
    {
        var row = await store.First(store.Query<Notification>().Where(x => x.Id == id && x.UserId == Actor), ct) ?? throw new BusinessException("NOT_FOUND", "Notification not found.", 404);
        row.ReadAt ??= DateTimeOffset.UtcNow;
        await store.Save(ct);
        return Envelope(new { read = true });
    }
    [HttpPost("notifications/read-all")]
    public async Task<object> ReadAll(CancellationToken ct)
    {
        var rows = await store.List(store.Query<Notification>().Where(x => x.UserId == Actor && x.ReadAt == null), ct);
        foreach (var row in rows) row.ReadAt = DateTimeOffset.UtcNow;
        await store.Save(ct);
        return Envelope(new { read = rows.Count });
    }
    [HttpGet("notifications/preferences")]
    public async Task<object> Preferences(CancellationToken ct)
    {
        var rows = await store.List(store.Query<NotificationPreference>().Where(x => x.UserId == Actor), ct);
        return Envelope(NotificationCategories.Select(category =>
        {
            var row = rows.FirstOrDefault(x => x.Category == category);
            return new { Category = category, InApp = row?.InApp ?? true, Push = row?.Push ?? false };
        }));
    }
    [HttpPatch("notifications/preferences")]
    public async Task<object> Preferences(NotificationPreferenceRequest request, CancellationToken ct)
    {
        Rules.Require(NotificationCategories.Contains(request.Category), "Unsupported notification category.");
        var row = await store.First(store.Query<NotificationPreference>().Where(x => x.UserId == Actor && x.Category == request.Category), ct);
        if (row is null) { row = new NotificationPreference { UserId = Actor, Category = request.Category }; store.Add(row); }
        row.InApp = request.InApp;
        row.Push = request.Push;
        await store.Save(ct);
        return Envelope(new { row.Category, row.InApp, row.Push });
    }
    [HttpGet("rewards/wallet")]
    public async Task<object> Wallet(CancellationToken ct) => Envelope(await service.Wallet(Actor, ct));
    [HttpPost("rewards/redeem")]
    public async Task<object> Redeem(RedemptionRequest request, CancellationToken ct) => Envelope(await service.Redeem(Actor, request, ct));
}
