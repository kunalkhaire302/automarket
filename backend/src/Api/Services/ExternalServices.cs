using System.Net.Http.Headers;
using System.Net.Http.Json;
using AutoMarket.Application;
using AutoMarket.Domain;
using Microsoft.AspNetCore.Http;

namespace AutoMarket.Api.Services;

public sealed record StoredImage(string StorageKey, string PublicUrl);

public sealed class SupabaseStorageClient(HttpClient http, IConfiguration config)
{
    private const int MaxBytes = 6 * 1024 * 1024;

    public async Task<StoredImage> UploadCarImage(Guid carId, IFormFile file, CancellationToken ct)
    {
        var url = config["SupabaseStorage:Url"]?.TrimEnd('/');
        var bucket = config["SupabaseStorage:Bucket"];
        var key = config["SupabaseStorage:ServiceRoleKey"];
        if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(bucket) || string.IsNullOrWhiteSpace(key))
            throw new BusinessException("IMAGE_UPLOAD_UNAVAILABLE", "Image uploads are not configured yet.", 503);
        Rules.Require(file.Length is > 0 and <= MaxBytes, "Each image must be smaller than 6 MB.");

        await using var input = file.OpenReadStream();
        using var buffer = new MemoryStream();
        await input.CopyToAsync(buffer, ct);
        var bytes = buffer.ToArray();
        var extension = DetectExtension(bytes) ?? throw new BusinessException("INVALID_IMAGE", "Upload a JPEG, PNG, or WebP image.");
        var storageKey = $"cars/{carId:N}/{Guid.NewGuid():N}.{extension}";
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{url}/storage/v1/object/{Uri.EscapeDataString(bucket)}/{storageKey}")
        {
            Content = new ByteArrayContent(bytes)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", key);
        request.Headers.Add("apikey", key);
        request.Headers.Add("x-upsert", "false");
        request.Content.Headers.ContentType = new MediaTypeHeaderValue(ContentType(extension));
        using var response = await http.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
            throw new BusinessException("IMAGE_UPLOAD_FAILED", "The image could not be uploaded. Please try again.", 503);
        return new(storageKey, $"{url}/storage/v1/object/public/{Uri.EscapeDataString(bucket)}/{storageKey}");
    }

    public static string? DetectExtension(ReadOnlySpan<byte> bytes) =>
        bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF ? "jpg" :
        bytes.Length >= 8 && bytes[..8].SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }) ? "png" :
        bytes.Length >= 12 && bytes[..4].SequenceEqual("RIFF"u8) && bytes.Slice(8, 4).SequenceEqual("WEBP"u8) ? "webp" : null;

    private static string ContentType(string extension) => extension switch { "jpg" => "image/jpeg", "png" => "image/png", _ => "image/webp" };
}

public sealed class ResendEmailClient(HttpClient http, IConfiguration config, ILogger<ResendEmailClient> logger)
{
    public async Task SendWelcome(UserDto user, CancellationToken ct)
    {
        var apiKey = config["Resend:ApiKey"];
        var from = config["Resend:From"];
        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(from)) return;
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        request.Content = JsonContent.Create(new { from, to = new[] { user.Email }, subject = "Welcome to AutoMarket", html = $"<p>Hi {System.Net.WebUtility.HtmlEncode(user.Name)},</p><p>Your AutoMarket account is ready.</p>" });
        using var response = await http.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode) logger.LogWarning("Resend welcome email failed with {StatusCode}", (int)response.StatusCode);
    }
}
