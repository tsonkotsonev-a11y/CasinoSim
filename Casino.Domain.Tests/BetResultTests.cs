namespace Casino.Domain.Tests;

public class BetResultTests
{
    // ── CreateSuccess ─────────────────────────────────────────────────────────

    [Fact]
    public void CreateSuccess_SetsStatusToSuccess()
    {
        var result = BetResult.CreateSuccess(5m, 10m);
        Assert.Equal(BetStatus.Success, result.Status);
    }

    [Fact]
    public void CreateSuccess_SetsCorrectBetAmountAndPayout()
    {
        var result = BetResult.CreateSuccess(5m, 10m);
        Assert.Equal(5m, result.BetAmount);
        Assert.Equal(10m, result.Payout);
    }

    [Fact]
    public void CreateSuccess_IsSuccessIsTrue()
    {
        var result = BetResult.CreateSuccess(5m, 10m);
        Assert.True(result.IsSuccess);
    }

    // ── CreateFailure ─────────────────────────────────────────────────────────

    [Fact]
    public void CreateFailure_InsufficientFunds_SetsCorrectStatus()
    {
        var result = BetResult.CreateFailure(BetStatus.InsufficientFunds);
        Assert.Equal(BetStatus.InsufficientFunds, result.Status);
    }

    [Fact]
    public void CreateFailure_InvalidBetAmount_SetsCorrectStatus()
    {
        var result = BetResult.CreateFailure(BetStatus.InvalidBetAmount);
        Assert.Equal(BetStatus.InvalidBetAmount, result.Status);
    }

    [Fact]
    public void CreateFailure_BetAmountAndPayoutAreZero()
    {
        var result = BetResult.CreateFailure(BetStatus.InsufficientFunds);
        Assert.Equal(0m, result.BetAmount);
        Assert.Equal(0m, result.Payout);
    }

    [Fact]
    public void CreateFailure_IsSuccessIsFalse()
    {
        var result = BetResult.CreateFailure(BetStatus.InsufficientFunds);
        Assert.False(result.IsSuccess);
    }
}