using System.Net.Mail;
using AutoMarket.Domain;

namespace AutoMarket.Application;

public sealed class AuthService(IStore store, IIdentity identity)
{
    public static UserDto Map(User u) => new(u.Id, u.Name, u.Email, u.Phone, u.Role, u.ReferralCode);
    public async Task<SessionDto> Register(RegisterRequest input, CancellationToken ct)
    {
        Rules.Require(!string.IsNullOrWhiteSpace(input.Name) && input.Name.Length <= 100, "Enter your name (maximum 100 characters).");
        Rules.Require(MailAddress.TryCreate(input.Email, out _) && input.Email.Length <= 254, "Enter a valid email address.");
        Rules.Require(input.Password is { Length: >= 12 and <= 128 }, "Password must contain 12–128 characters.");
        Rules.Require(input.Phone is { Length: <= 30 }, "Phone number is too long.");
        var email = input.Email.Trim().ToLowerInvariant();
        return await store.Transaction(async () =>
        {
            Rules.Require(await store.First(store.Query<User>().Where(x => x.Email == email), ct) is null,
                "Unable to register with these details.", "REGISTRATION_FAILED", 409);
            Guid? referrer = null;
            if (!string.IsNullOrWhiteSpace(input.ReferralCode))
            {
                var code = input.ReferralCode.Trim().ToUpperInvariant();
                var referringUser = await store.First(store.Query<User>().Where(x => x.ReferralCode == code && x.IsActive), ct);
                Rules.Require(referringUser is not null && referringUser.Email != email, "Referral code is not eligible.");
                referrer = referringUser!.Id;
            }
            var user = new User { Name = input.Name.Trim(), Email = email, Phone = input.Phone.Trim(), ReferrerId = referrer };
            user.PasswordHash = identity.HashPassword(user, input.Password);
            store.Add(user);
            if (referrer.HasValue)
                store.Add(new Referral { ReferrerId = referrer.Value, ReferredUserId = user.Id,
                    CodeSnapshot = input.ReferralCode!.Trim().ToUpperInvariant() });
            return await CreateSession(user, ct);
        }, ct);
    }
    public async Task<SessionDto> Login(LoginRequest input, CancellationToken ct)
    {
        Rules.Require(input.Email is { Length: <= 254 } && input.Password is { Length: <= 128 }, "Invalid credentials.", "INVALID_CREDENTIALS", 401);
        var email = input.Email.Trim().ToLowerInvariant();
        var user = await store.First(store.Query<User>().Where(x => x.Email == email), ct);
        // Verify a real hash even for an unknown account to reduce account enumeration through timing.
        var candidate = user ?? new User { PasswordHash = DummyHash.Value };
        var valid = identity.VerifyPassword(candidate, input.Password);
        Rules.Require(valid && user is { IsActive: true }, "Invalid email or password.", "INVALID_CREDENTIALS", 401);
        return await CreateSession(user!, ct);
    }
    private static readonly Lazy<string> DummyHash = new(() =>
        "AQAAAAIAAYagAAAAEBIRY8LmIEzYR3jokdxjuaHY8XRhFnAxnjMQ55F18qRWBfDUGsqNyQrpcyorKgBkXw==");
    public Task<SessionDto> Refresh(string token, CancellationToken ct) => store.Transaction(async () =>
    {
        Rules.Require(token.Length is > 20 and <= 200, "Session expired.", "SESSION_EXPIRED", 401);
        var hash = identity.HashToken(token);
        var session = await store.First(store.Query<RefreshSession>().Where(x => x.TokenHash == hash), ct);
        Rules.Require(session is not null && session.RevokedAt is null && session.ExpiresAt > DateTimeOffset.UtcNow,
            "Session expired. Sign in again.", "SESSION_EXPIRED", 401);
        var user = await store.First(store.Query<User>().Where(x => x.Id == session!.UserId && x.IsActive), ct);
        Rules.Require(user is not null, "Session expired.", "SESSION_EXPIRED", 401);
        session!.RevokedAt = DateTimeOffset.UtcNow;
        return await CreateSession(user!, ct);
    }, ct);
    public async Task Logout(string token, CancellationToken ct)
    {
        var hash = identity.HashToken(token);
        var session = await store.First(store.Query<RefreshSession>().Where(x => x.TokenHash == hash), ct);
        if (session is not null) { session.RevokedAt = DateTimeOffset.UtcNow; await store.Save(ct); }
    }
    public async Task<UserDto> Me(Guid id, CancellationToken ct)
    {
        var user = await store.First(store.Query<User>().Where(x => x.Id == id && x.IsActive), ct);
        return user is null ? throw new BusinessException("UNAUTHORIZED", "Sign in again.", 401) : Map(user);
    }
    private async Task<SessionDto> CreateSession(User user, CancellationToken ct)
    {
        var token = identity.NewToken();
        store.Add(new RefreshSession { UserId = user.Id, TokenHash = identity.HashToken(token), ExpiresAt = DateTimeOffset.UtcNow.AddDays(14) });
        await store.Save(ct);
        return new(identity.AccessToken(user), token, Map(user));
    }
}
