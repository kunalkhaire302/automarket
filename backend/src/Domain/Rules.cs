namespace AutoMarket.Domain;

public sealed class BusinessException(string code, string message, int status = 400) : Exception(message)
{
    public string Code { get; } = code;
    public int Status { get; } = status;
}
public static class Rules
{
    public static void Require(bool condition, string message, string code = "VALIDATION_ERROR", int status = 400)
    {
        if (!condition) throw new BusinessException(code, message, status);
    }
    public static void Transition(string current, string next, IReadOnlyDictionary<string, string[]> transitions)
    {
        Require(transitions.TryGetValue(current, out var allowed) && allowed.Contains(next),
            $"Cannot change {current} to {next}.", "INVALID_TRANSITION", 409);
    }
    public static readonly IReadOnlyDictionary<string, string[]> BookingTransitions = new Dictionary<string, string[]>
    {
        ["PENDING"] = ["CONFIRMED", "CANCELLED", "EXPIRED"],
        ["CONFIRMED"] = ["COMPLETED", "CANCELLED"],
    };
    public static readonly IReadOnlyDictionary<string, string[]> AppointmentTransitions = new Dictionary<string, string[]>
    {
        ["PENDING"] = ["CONFIRMED", "RESCHEDULED", "CANCELLED"],
        ["CONFIRMED"] = ["COMPLETED", "RESCHEDULED", "CANCELLED", "NO_SHOW"],
        ["RESCHEDULED"] = ["CONFIRMED", "CANCELLED"]
    };
    public static readonly IReadOnlyDictionary<string, string[]> SubmissionTransitions = new Dictionary<string, string[]>
    {
        ["DRAFT"] = ["SUBMITTED"], ["SUBMITTED"] = ["UNDER_REVIEW"],
        ["UNDER_REVIEW"] = ["APPROVED", "REJECTED", "NEEDS_INFORMATION"],
        ["NEEDS_INFORMATION"] = ["SUBMITTED"]
    };
    public static decimal RecommendedPrice(decimal basis, IEnumerable<decimal> adjustments)
    {
        Require(basis > 0, "A positive base price is required.");
        var result = basis;
        foreach (var multiplier in adjustments)
        {
            Require(multiplier is >= 0.5m and <= 1.5m, "Pricing multiplier is outside the allowed range.");
            result *= multiplier;
        }
        return decimal.Round(result, 2, MidpointRounding.AwayFromZero);
    }
}
