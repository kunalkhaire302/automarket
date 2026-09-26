using AutoMarket.Application;
using AutoMarket.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoMarket.Api.Controllers;

[Authorize, Route("api/v1/ai")]
public sealed class AiController(IAiService ai, IStore store) : BaseController
{
    [HttpGet("models/status")]
    public async Task<object> Models(CancellationToken ct) => Envelope(await ai.Models(ct));

    [HttpPost("valuation")]
    public async Task<object> Valuation(AiValuationRequest request, CancellationToken ct)
    {
        Rules.Require(request.RegistrationYear is >= 1950 && request.RegistrationYear <= 2100, "Registration year is invalid.");
        Rules.Require(request.Kilometers is >= 0 and <= 2_000_000, "Kilometres must be between 0 and 2,000,000.");
        Rules.Require(request.OwnershipCount is >= 1 and <= 20, "Ownership count must be between 1 and 20.");
        return Envelope(await ai.Valuation(request, ct));
    }

    [HttpPost("recommendations")]
    public async Task<object> Recommendations(AiRecommendationRequest request, CancellationToken ct)
    {
        Rules.Require(request.Limit is >= 1 and <= 30, "Recommendation limit must be between 1 and 30.");
        var cars = await store.List(store.Query<Car>().Where(c => c.Status == "LISTED").OrderByDescending(c => c.UpdatedAt).Take(200), ct);
        var preferences = new[] { request.FuelType, request.Transmission, request.BodyType }.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        var candidates = cars.Select(c =>
        {
            var matched = preferences.Count(p => string.Equals(p, c.FuelType, StringComparison.OrdinalIgnoreCase)
                || string.Equals(p, c.Transmission, StringComparison.OrdinalIgnoreCase)
                || string.Equals(p, c.BodyType, StringComparison.OrdinalIgnoreCase));
            var age = DateTimeOffset.UtcNow - c.UpdatedAt;
            return new AiRecommendationCandidate(c.Id.ToString(), true,
                request.MaxPrice is null ? 1 : c.Price <= request.MaxPrice ? 1 : Math.Max(0, 1 - (double)((c.Price - request.MaxPrice.Value) / request.MaxPrice.Value)),
                preferences.Length == 0 ? 1 : (double)matched / preferences.Length,
                .5, request.CityId is null ? 1 : c.CityId == request.CityId ? 1 : 0,
                age.TotalDays <= 7 ? 1 : age.TotalDays <= 90 ? .5 : .2, .5);
        }).ToArray();
        return Envelope(await ai.Recommendations(new(candidates, request.Limit), ct), new { candidateCount = candidates.Length, advisory = true });
    }

    [HttpPost("semantic-search")]
    public object SemanticSearch(AiSemanticSearchRequest request)
    {
        Rules.Require(!string.IsNullOrWhiteSpace(request.Query) && request.Query.Length <= 200, "Search text must be between 1 and 200 characters.");
        return Envelope(new { status = "NOT_CONFIGURED", message = "Semantic search is not configured. Use the standard catalogue search.",
            fallback = new { path = "/api/v1/cars", q = request.Query, request.CityId } });
    }
}
