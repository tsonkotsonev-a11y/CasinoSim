using Casino.Application;

namespace Casino.Application.Tests;

public class GameMathProviderTests
{
    private readonly GameMathProvider _sut = new();

    [Fact]
    public void GetBetOutcomeTiers_ReturnsThreeTiers()
    {
        var tiers = _sut.GetBetOutcomeTiers();
        Assert.Equal(3, tiers.Count);
    }

    [Fact]
    public void GetBetOutcomeTiers_TotalProbabilityIs100()
    {
        var tiers = _sut.GetBetOutcomeTiers();
        double total = tiers.Sum(t => t.Probability);
        Assert.Equal(100.0, total);
    }

    [Fact]
    public void GetBetOutcomeTiers_LoseTierHasZeroMultiplier()
    {
        var tiers = _sut.GetBetOutcomeTiers();
        var lose = tiers.Single(t => t.Name == "Lose");
        Assert.Equal(0m, lose.MinMultiplier);
        Assert.Equal(0m, lose.MaxMultiplier);
    }

    [Fact]
    public void GetBetOutcomeTiers_WinTierHasCorrectRange()
    {
        var tiers = _sut.GetBetOutcomeTiers();
        var win = tiers.Single(t => t.Name == "Win");
        Assert.Equal(0.01m, win.MinMultiplier);
        Assert.Equal(2m, win.MaxMultiplier);
    }

    [Fact]
    public void GetBetOutcomeTiers_JackpotTierHasCorrectRange()
    {
        var tiers = _sut.GetBetOutcomeTiers();
        var jackpot = tiers.Single(t => t.Name == "Jackpot");
        Assert.Equal(2m, jackpot.MinMultiplier);
        Assert.Equal(10m, jackpot.MaxMultiplier);
    }

    [Fact]
    public void GetBetOutcomeTiers_ReturnsReadOnlyList()
    {
        var result = _sut.GetBetOutcomeTiers();
        Assert.IsAssignableFrom<IReadOnlyList<BetOutcomeTier>>(result);
    }
}