using AutoMarket.Domain;

namespace AutoMarket.Application;

public sealed class WorkflowService(IStore store, CatalogService catalog)
{
    public static AppointmentDto Map(Appointment a) => new(a.Id, a.CarId, a.ServiceCenterId, a.StartsAt, a.EndsAt, a.AppointmentType, a.Status, a.Notes);
    public static BookingDto Map(Booking b) => new(b.Id, b.CarId, b.AppointmentId, b.BookingReference, b.Status, b.Amount, b.CreatedAt);
    public static SubmissionDto Map(SellerSubmission s) => new(s.Id, s.CarId, s.Status, s.ReviewNotes, s.SubmittedAt);
    public void Event(Guid user, string type, Guid entity, string message, string link)
    {
        store.Add(new AuditLog { ActorUserId = user, Action = type, EntityId = entity, EntityType = type.Split('_')[0] });
        store.Add(new Notification { UserId = user, Type = type, Title = type.Replace('_', ' '), Message = message,
            DeepLink = link, EventKey = $"{type}:{entity}:{Guid.NewGuid()}" });
    }
    public Task<AppointmentDto> CreateAppointment(Guid user, AppointmentRequest r, CancellationToken ct) => store.Transaction(async () =>
    {
        var now = DateTimeOffset.UtcNow;
        Rules.Require(r.StartsAt.Offset == TimeSpan.Zero && r.StartsAt > now.AddHours(1) && r.StartsAt < now.AddDays(60), "Choose a UTC appointment between one hour and 60 days from now.");
        Rules.Require(r.StartsAt.Minute is 0 or 30 && r.StartsAt.Second == 0, "Appointments start on a half-hour boundary.");
        Rules.Require(r.Notes.Length <= 1000, "Notes are limited to 1,000 characters.");
        Rules.Require(new[] { "VIEWING", "TEST_DRIVE", "INSPECTION" }.Contains(r.AppointmentType), "Invalid appointment type.");
        var car = await store.First(store.Query<Car>().Where(c => c.Id == r.CarId), ct);
        Rules.Require(car is not null && (car.Status == "LISTED" || (r.AppointmentType == "INSPECTION" && car.SellerId == user && car.Status == "PENDING_REVIEW")), "This vehicle is unavailable for an appointment.", "VEHICLE_UNAVAILABLE", 409);
        var center = await store.First(store.Query<ServiceCenter>().Where(c => c.Id == r.ServiceCenterId && c.CityId == car!.CityId), ct);
        Rules.Require(center is not null, "Select a service center in the vehicle's city.");
        var end = r.StartsAt.AddMinutes(30);
        var conflict = await store.Count(store.Query<Appointment>().Where(a => a.CarId == r.CarId &&
            a.Status != "CANCELLED" && a.Status != "NO_SHOW" && a.StartsAt < end && a.EndsAt > r.StartsAt), ct);
        Rules.Require(conflict == 0, "That appointment slot has been taken.", "SLOT_UNAVAILABLE", 409);
        var appointment = new Appointment { UserId = user, CarId = r.CarId, ServiceCenterId = r.ServiceCenterId,
            StartsAt = r.StartsAt, EndsAt = end, Notes = r.Notes, AppointmentType = r.AppointmentType };
        store.Add(appointment);
        Event(user, "APPOINTMENT_CREATED", appointment.Id, "Your appointment request has been recorded.", "/profile/appointments");
        await store.Save(ct);
        return Map(appointment);
    }, ct);
    public Task<BookingDto> CreateBooking(Guid user, BookingRequest r, string key, CancellationToken ct) => store.Transaction(async () =>
    {
        Rules.Require(key.Length is >= 8 and <= 100, "Supply an Idempotency-Key of 8–100 characters.");
        var prior = await store.First(store.Query<Booking>().Where(b => b.UserId == user && b.IdempotencyKey == key), ct);
        if (prior is not null)
        {
            Rules.Require(prior.CarId == r.CarId && prior.AppointmentId == r.AppointmentId, "Idempotency key was used for a different request.", "IDEMPOTENCY_CONFLICT", 409);
            return Map(prior);
        }
        var car = await store.First(store.Query<Car>().Where(c => c.Id == r.CarId), ct);
        Rules.Require(car is { Status: "LISTED" } && car.SellerId != user, "This vehicle cannot be booked.", "VEHICLE_UNAVAILABLE", 409);
        var appointment = await store.First(store.Query<Appointment>().Where(a => a.Id == r.AppointmentId && a.UserId == user && a.CarId == r.CarId), ct);
        Rules.Require(appointment is not null && (appointment.Status == "CONFIRMED" || appointment.Status == "COMPLETED"), "A confirmed or completed appointment for this vehicle is required.", "APPOINTMENT_REQUIRED", 409);
        var booking = new Booking { UserId = user, CarId = r.CarId, AppointmentId = r.AppointmentId, Amount = car!.Price, IdempotencyKey = key };
        car.Status = "BOOKED";
        car.UpdatedAt = DateTimeOffset.UtcNow;
        store.Add(booking);
        Event(user, "BOOKING_CREATED", booking.Id, $"Purchase intent {booking.BookingReference} recorded. No payment has been taken.", "/profile/bookings");
        await store.Save(ct);
        return Map(booking);
    }, ct);
    public Task<BookingDto> ChangeBooking(Guid actor, bool admin, Guid id, string next, CancellationToken ct) => store.Transaction(async () =>
    {
        var b = await store.First(store.Query<Booking>().Where(x => x.Id == id && (admin || x.UserId == actor)), ct)
            ?? throw new BusinessException("NOT_FOUND", "Booking not found.", 404);
        Rules.Require(admin || next == "CANCELLED", "Only an administrator can change booking status.", "FORBIDDEN", 403);
        Rules.Transition(b.Status, next, Rules.BookingTransitions);
        var car = await store.First(store.Query<Car>().Where(x => x.Id == b.CarId), ct)
            ?? throw new BusinessException("NOT_FOUND", "Vehicle not found.", 404);
        b.Status = next;
        if (next is "CANCELLED" or "EXPIRED") car.Status = "LISTED";
        if (next == "COMPLETED")
        {
            car.Status = "SOLD";
            await QualifyReferral(b, ct);
        }
        Event(b.UserId, "BOOKING_" + next, b.Id, $"Your booking is now {next.ToLowerInvariant()}.", "/profile/bookings");
        if (actor != b.UserId) store.Add(new AuditLog { ActorUserId = actor, Action = "ADMIN_BOOKING_" + next, EntityId = b.Id, EntityType = "Booking" });
        await store.Save(ct);
        return Map(b);
    }, ct);
    private async Task QualifyReferral(Booking booking, CancellationToken ct)
    {
        var referral = await store.First(store.Query<Referral>().Where(r => r.ReferredUserId == booking.UserId), ct);
        if (referral is null || referral.Status != "ATTRIBUTED") return;
        referral.Status = "QUALIFIED";
        referral.QualifyingBookingId = booking.Id;
        referral.QualifiedAt = DateTimeOffset.UtcNow;
        const int reward = 500;
        store.Add(new WalletTransaction { UserId = referral.ReferrerId, Points = reward, Type = "REFERRAL_REWARD",
            EventKey = $"referral:{referral.Id}:referrer", ReferenceId = booking.Id });
        store.Add(new WalletTransaction { UserId = referral.ReferredUserId, Points = reward, Type = "WELCOME_REWARD",
            EventKey = $"referral:{referral.Id}:referred", ReferenceId = booking.Id });
        Event(referral.ReferrerId, "REFERRAL_REWARD", referral.Id, $"You earned {reward} points from a completed referral.", "/profile/rewards");
        Event(referral.ReferredUserId, "REFERRAL_REWARD", referral.Id, $"You earned {reward} welcome points.", "/profile/rewards");
    }

    public async Task<WalletDto> Wallet(Guid user, CancellationToken ct)
    {
        var rows = await store.List(store.Query<WalletTransaction>().Where(x => x.UserId == user)
            .OrderByDescending(x => x.CreatedAt).Take(200), ct);
        var now = DateTimeOffset.UtcNow;
        var balance = rows.Where(x => x.Status == "POSTED" && (x.ExpiresAt == null || x.ExpiresAt > now)).Sum(x => x.Points);
        return new(balance, rows.Select(x => new WalletTransactionDto(x.Id, x.Points, x.Type, x.Status, x.EventKey,
            x.ReferenceId, x.CreatedAt, x.ExpiresAt)).ToList());
    }
    public Task<WalletDto> Redeem(Guid user, RedemptionRequest request, CancellationToken ct) => store.Transaction(async () =>
    {
        Rules.Require(request.Points is >= 100 and <= 10000 && request.Points % 100 == 0, "Redeem 100–10,000 points in increments of 100.");
        Rules.Require(request.IdempotencyKey is { Length: >= 8 and <= 100 }, "A valid idempotency key is required.");
        var eventKey = $"redemption:{user}:{request.IdempotencyKey}";
        if (await store.First(store.Query<WalletTransaction>().Where(x => x.EventKey == eventKey), ct) is not null)
            return await Wallet(user, ct);
        var wallet = await Wallet(user, ct);
        Rules.Require(wallet.Balance >= request.Points, "Your points balance is too low.", "INSUFFICIENT_BALANCE", 409);
        var transaction = new WalletTransaction { UserId = user, Points = -request.Points, Type = "REDEMPTION",
            EventKey = eventKey, Status = "POSTED" };
        store.Add(transaction);
        Event(user, "REWARD_REDEEMED", transaction.Id, $"{request.Points} points were redeemed.", "/profile/rewards");
        await store.Save(ct);
        return await Wallet(user, ct);
    }, ct);
    public Task<AppointmentDto> ChangeAppointment(Guid actor, bool admin, Guid id, string next, CancellationToken ct) => store.Transaction(async () =>
    {
        var a = await store.First(store.Query<Appointment>().Where(x => x.Id == id && (admin || x.UserId == actor)), ct)
            ?? throw new BusinessException("NOT_FOUND", "Appointment not found.", 404);
        Rules.Require(admin || next == "CANCELLED", "Only an administrator can change this status.", "FORBIDDEN", 403);
        Rules.Transition(a.Status, next, Rules.AppointmentTransitions);
        Rules.Require(await store.Count(store.Query<Booking>().Where(b => b.AppointmentId == id && b.Status != "CANCELLED" && b.Status != "EXPIRED"), ct) == 0,
            "Cancel the associated booking before changing its appointment.", "BOOKING_EXISTS", 409);
        a.Status = next;
        Event(a.UserId, "APPOINTMENT_" + next, a.Id, $"Your appointment is now {next.ToLowerInvariant()}.", "/profile/appointments");
        if (actor != a.UserId) store.Add(new AuditLog { ActorUserId = actor, Action = "ADMIN_APPOINTMENT_" + next, EntityId = a.Id, EntityType = "Appointment" });
        await store.Save(ct);
        return Map(a);
    }, ct);
    public Task<SubmissionDto> CreateSubmission(Guid seller, CarRequest r, CancellationToken ct) => store.Transaction(async () =>
    {
        var car = await catalog.Create(seller, r, ct);
        var submission = new SellerSubmission { SellerId = seller, CarId = car.Id };
        store.Add(submission);
        await store.Save(ct);
        return Map(submission);
    }, ct);
    public Task<SubmissionDto> ChangeSubmission(Guid actor, bool admin, Guid id, StatusRequest r, CancellationToken ct) => store.Transaction(async () =>
    {
        var s = await store.First(store.Query<SellerSubmission>().Where(x => x.Id == id && (admin || x.SellerId == actor)), ct)
            ?? throw new BusinessException("NOT_FOUND", "Submission not found.", 404);
        Rules.Require(admin || r.Status == "SUBMITTED", "Review requires administrator access.", "FORBIDDEN", 403);
        Rules.Require(r.Notes.Length <= 2000, "Review notes are too long.");
        Rules.Transition(s.Status, r.Status, Rules.SubmissionTransitions);
        if (r.Status is "REJECTED" or "NEEDS_INFORMATION") Rules.Require(!string.IsNullOrWhiteSpace(r.Notes), "Explain the review decision.");
        var car = await store.First(store.Query<Car>().Where(x => x.Id == s.CarId), ct)
            ?? throw new BusinessException("NOT_FOUND", "Vehicle not found.", 404);
        s.Status = r.Status;
        if (r.Status == "SUBMITTED") { s.SubmittedAt = DateTimeOffset.UtcNow; car.Status = "PENDING_REVIEW"; }
        if (admin) { s.ReviewNotes = r.Notes; s.ReviewedBy = actor; }
        if (r.Status == "APPROVED") car.Status = "LISTED";
        if (r.Status == "REJECTED") car.Status = "REJECTED";
        Event(s.SellerId, "SUBMISSION_" + r.Status, s.Id, "Your vehicle submission status has changed.", "/profile/seller-submissions");
        store.Add(new AuditLog { ActorUserId = actor, Action = "REVIEW_" + r.Status, EntityId = s.Id, EntityType = "SellerSubmission" });
        await store.Save(ct);
        return Map(s);
    }, ct);
}
