using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMarket.Application;
using AutoMarket.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AutoMarket.Infrastructure;

public sealed record JwtSettings(string Key, string Issuer, string Audience);
public sealed class Identity(JwtSettings settings) : IIdentity
{
    private readonly PasswordHasher<User> hasher = new();
    public string HashPassword(User user, string password) => hasher.HashPassword(user, password);
    public bool VerifyPassword(User user, string password) => hasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Failed;
    public string AccessToken(User user)
    {
        var jwt = new JwtSecurityToken(settings.Issuer, settings.Audience,
            [new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim(ClaimTypes.Role, user.Role)],
            expires: DateTime.UtcNow.AddMinutes(15), signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)), SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
    public string NewToken() => Convert.ToHexString(RandomNumberGenerator.GetBytes(48));
    public string HashToken(string token) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
