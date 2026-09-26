using System.Net.Http.Json;
using System.Text.Json;
using AutoMarket.Application;

namespace AutoMarket.Infrastructure;

public sealed class AiClient(HttpClient client, AiSettings settings) : IAiService
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public Task<JsonElement> Models(CancellationToken ct) => Send(HttpMethod.Get, "models", null, ct);
    public Task<JsonElement> Valuation(AiValuationRequest request, CancellationToken ct) => Send(HttpMethod.Post, "valuation", request, ct);
    public Task<JsonElement> Recommendations(AiRecommendationPayload request, CancellationToken ct) => Send(HttpMethod.Post, "recommendations", request, ct);

    private async Task<JsonElement> Send(HttpMethod method, string path, object? body, CancellationToken ct)
    {
        if (client.BaseAddress is null || string.IsNullOrWhiteSpace(settings.ApiKey))
            return Element(new { status = "NOT_CONFIGURED", message = "The optional intelligence service is not configured." });

        try
        {
            using var request = new HttpRequestMessage(method, path);
            request.Headers.Add("X-API-Key", settings.ApiKey);
            if (body is not null) request.Content = JsonContent.Create(body, options: Json);
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
            if (!response.IsSuccessStatusCode)
                return Element(new { status = "UNAVAILABLE", message = "The optional intelligence service could not complete this request." });
            return await response.Content.ReadFromJsonAsync<JsonElement>(Json, ct);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            return Element(new { status = "UNAVAILABLE", message = "The optional intelligence service timed out." });
        }
        catch (HttpRequestException)
        {
            return Element(new { status = "UNAVAILABLE", message = "The optional intelligence service is temporarily unavailable." });
        }
    }

    private static JsonElement Element(object value) => JsonSerializer.SerializeToElement(value, Json);
}

public sealed record AiSettings(string ApiKey);
