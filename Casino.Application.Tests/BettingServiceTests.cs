using Casino.Domain;
using NSubstitute;

namespace Casino.Application.Tests;

public class BettingServiceTests
{
    private readonly IRandomNumberGenerator _rng;
    private readonly IGameMathProvider _mathProvider;
    private readonly BettingService _sut;

    public BettingServiceTests()
    {
        _rng = Substitute.For<IRandomNumberGenerator>();
        _mathProvider = Substitute.For<IGameMathProvider>();
        _sut = new BettingService(_rng, _mathProvider);
    }

    // Fixed-multiplier tier helpers — Min == Max triggers the fast-fail path in CalculateMultiplier
    private static IReadOnlyList<BetOutcomeTier> LoseTiers() =>
        [new BetOutcomeTier("Lose", 100, 0m, 0m)];

    private static IReadOnlyList<BetOutcomeTier> FixedWinTiers(decimal multiplier) =>
        [new BetOutcomeTier("Win", 100, multiplier, multiplier)];

    // ── Bet amount validation ──────────────────────────────────────────────────

    [Fact]
    public void PlaceBet_BelowMinBet_ReturnsInvalidBetAmount()
    {
        var player = new Player("Alice", 100m);
        var result = _sut.PlaceBet(player, 0.99m);
        Assert.Equal(BetStatus.InvalidBetAmount, result.Status);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void PlaceBet_AboveMaxBet_ReturnsInvalidBetAmount()
    {
        var player = new Player("Alice", 100m);
        var result = _sut.PlaceBet(player, 10.01m);
        Assert.Equal(BetStatus.InvalidBetAmount, result.Status);
    }

    [Fact]
    public void PlaceBet_ZeroAmount_ReturnsInvalidBetAmount()
    {
        var player = new Player("Alice", 100m);
        var result = _sut.PlaceBet(player, 0m);
        Assert.Equal(BetStatus.InvalidBetAmount, result.Status);
    }

    [Fact]
    public void PlaceBet_NegativeAmount_ReturnsInvalidBetAmount()
    {
        var player = new Player("Alice", 100m);
        var result = _sut.PlaceBet(player, -5m);
        Assert.Equal(BetStatus.InvalidBetAmount, result.Status);
    }

    [Fact]
    public void PlaceBet_ExactlyAtMinBet_IsNotRejected()
    {
        _mathProvider.GetBetOutcomeTiers().Returns(LoseTiers());
        _rng.NextDouble().Returns(0.5);
        var result = _sut.PlaceBet(new Player("Alice", 100m), 1m);
        Assert.NotEqual(BetStatus.InvalidBetAmount, result.Status);
    }

    [Fact]
    public void PlaceBet_ExactlyAtMaxBet_IsNotRejected()
    {
        _mathProvider.GetBetOutcomeTiers().Returns(LoseTiers());
        _rng.NextDouble().Returns(0.5);
        var result = _sut.PlaceBet(new Player("Alice", 100m), 10m);
        Assert.NotEqual(BetStatus.InvalidBetAmount, result.Status);
    }

    // ── Insufficient funds ────────────────────────────────────────────────────

    [Fact]
    public void PlaceBet_InsufficientFunds_ReturnsInsufficientFunds()
    {
        var player = new Player("Alice", 0.50m);
        var result = _sut.PlaceBet(player, 1m);
        Assert.Equal(BetStatus.InsufficientFunds, result.Status);
    }

    [Fact]
    public void PlaceBet_InsufficientFunds_DoesNotDebitPlayer()
    {
        var player = new Player("Alice", 0.50m);
        _sut.PlaceBet(player, 1m);
        Assert.Equal(0.50m, player.Balance);
    }

    // ── Lose tier ─────────────────────────────────────────────────────────────

    [Fact]
    public void PlaceBet_LoseTier_ReturnsSuccessWithZeroPayout()
    {
        _mathProvider.GetBetOutcomeTiers().Returns(LoseTiers());
        _rng.NextDouble().Returns(0.5); // → NextDouble(0,100) = 50 ≤ 100 → Lose tier
        var result = _sut.PlaceBet(new Player("Alice", 100m), 5m);
        Assert.Equal(BetStatus.Success, result.Status);
        Assert.Equal(0m, result.Payout);
    }

    [Fact]
    public void PlaceBet_LoseTier_DebitsPlayerExactBetAmount()
    {
        _mathProvider.GetBetOutcomeTiers().Returns(LoseTiers());
        _rng.NextDouble().Returns(0.5);
        var player = new Player("Alice", 100m);
        _sut.PlaceBet(player, 5m);
        Assert.Equal(95m, player.Balance);
    }

    // ── Win tier ──────────────────────────────────────────────────────────────

    [Fact]
    public void PlaceBet_WinTier_ReturnsSuccessWithPositivePayout()
    {
        _mathProvider.GetBetOutcomeTiers().Returns(FixedWinTiers(2m));
        _rng.NextDouble().Returns(0.5);
        var result = _sut.PlaceBet(new Player("Alice", 100m), 5m);
        Assert.Equal(BetStatus.Success, result.Status);
        Assert.Equal(10m, result.Payout); // 5 × 2
    }

    [Fact]
    public void PlaceBet_WinTier_CreditsPlayerWithPayout()
    {
        _mathProvider.GetBetOutcomeTiers().Returns(FixedWinTiers(2m));
        _rng.NextDouble().Returns(0.5);
        var player = new Player("Alice", 100m);
        _sut.PlaceBet(player, 5m);
        Assert.Equal(105m, player.Balance); // −5 bet, +10 win
    }

    // ── Jackpot tier ──────────────────────────────────────────────────────────

    [Fact]
    public void PlaceBet_JackpotTier_ReturnsSuccessWithHighPayout()
    {
        _mathProvider.GetBetOutcomeTiers().Returns(FixedWinTiers(10m));
        _rng.NextDouble().Returns(0.5);
        var result = _sut.PlaceBet(new Player("Alice", 100m), 10m);
        Assert.Equal(BetStatus.Success, result.Status);
        Assert.Equal(100m, result.Payout); // 10 × 10
    }

    // ── Fixed multiplier (Min == Max fast-fail) ───────────────────────────────

    [Fact]
    public void PlaceBet_FixedMultiplierTier_ReturnsExactPayout()
    {
        _mathProvider.GetBetOutcomeTiers().Returns(FixedWinTiers(3m));
        _rng.NextDouble().Returns(0.5);
        var result = _sut.PlaceBet(new Player("Alice", 100m), 5m);
        Assert.Equal(15m, result.Payout); // 5 × 3
    }

    // ── Variable multiplier (Min != Max, uses second RNG call) ───────────────

    [Fact]
    public void PlaceBet_VariableMultiplierTier_ReturnsRoundedScaledPayout()
    {
        // Tier: Min=1, Max=3; both RNG calls return 0.5
        // Tier-select:    NextDouble(0,100)  = 0.5*100     = 50 ≤ 100 → hits tier
        // Multiplier:     NextDouble(1.0,3.0)= 0.5*(3-1)+1 = 2.0 → rounded = 2.0m
        var tiers = new List<BetOutcomeTier> { new("Win", 100, 1m, 3m) };
        _mathProvider.GetBetOutcomeTiers().Returns(tiers);
        _rng.NextDouble().Returns(0.5, 1);

        var result = _sut.PlaceBet(new Player("Alice", 100m), 5m);

        Assert.Equal(15m, result.Payout); // 5 × 3.0
    }

    // ── Probability boundary ──────────────────────────────────────────────────

    [Fact]
    public void PlaceBet_TierProbabilityBoundary_FirstTierHit()
    {
        // First tier covers exactly [0, 50]; raw 0.5 → 50.0 == boundary → hits first tier
        var tiers = new List<BetOutcomeTier>
        {
            new("Win",  50, 2m, 2m),
            new("Lose", 50, 0m, 0m),
        };
        _mathProvider.GetBetOutcomeTiers().Returns(tiers);
        _rng.NextDouble().Returns(0.5); // → 50.0 ≤ 50.0 cumulative → Win

        var result = _sut.PlaceBet(new Player("Alice", 100m), 5m);

        Assert.Equal(10m, result.Payout); // confirms Win tier was hit, not Lose
    }

    // ── No tier matched ───────────────────────────────────────────────────────

    [Fact]
    public void PlaceBet_NoTierMatched_ReturnsSuccessWithZeroPayout()
    {
        // Tiers only sum to 50%; rng produces 60 → no tier matched → SimulateBetOutcome returns 0
        var tiers = new List<BetOutcomeTier> { new("Win", 50, 2m, 2m) };
        _mathProvider.GetBetOutcomeTiers().Returns(tiers);
        _rng.NextDouble().Returns(0.6); // → 60.0 > 50.0 cumulative → no match

        var result = _sut.PlaceBet(new Player("Alice", 100m), 5m);

        Assert.Equal(BetStatus.Success, result.Status);
        Assert.Equal(0m, result.Payout);
    }
}