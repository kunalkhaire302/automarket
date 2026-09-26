using AutoMarket.Domain;
using Xunit;

namespace AutoMarket.Tests;

public sealed class RulesTests
{
    [Fact]
    public void RecommendedPrice_ComposesConfiguredMultipliers()
        => Assert.Equal(1_045_000m, Rules.RecommendedPrice(1_000_000m, [1.10m, .95m]));

    [Theory]
    [InlineData(0.49)]
    [InlineData(1.51)]
    public void RecommendedPrice_RejectsUnsafeMultiplier(decimal multiplier)
        => Assert.Throws<BusinessException>(() => Rules.RecommendedPrice(500_000m, [multiplier]));

    [Fact]
    public void Booking_CanMoveFromPendingToConfirmed()
        => Rules.Transition("PENDING", "CONFIRMED", Rules.BookingTransitions);

    [Fact]
    public void Booking_CannotMoveFromCancelledBackToPending()
        => Assert.Throws<BusinessException>(() => Rules.Transition("CANCELLED", "PENDING", Rules.BookingTransitions));

    [Fact]
    public void Submission_CannotSkipHumanReview()
        => Assert.Throws<BusinessException>(() => Rules.Transition("SUBMITTED", "APPROVED", Rules.SubmissionTransitions));
}
