namespace Casino.Domain.Tests;

public class PlayerTests
{
    // ── Constructor ───────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_ValidArgs_SetsNameAndBalance()
    {
        var player = new Player("Alice", 100m);
        Assert.Equal("Alice", player.Name);
        Assert.Equal(100m, player.Balance);
    }

    [Fact]
    public void Constructor_ZeroBalance_IsValid()
    {
        var player = new Player("Alice", 0m);
        Assert.Equal(0m, player.Balance);
    }

    [Fact]
    public void Constructor_EmptyName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Player("", 100m));
    }

    [Fact]
    public void Constructor_WhitespaceName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Player("   ", 100m));
    }

    [Fact]
    public void Constructor_NegativeInitialBalance_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Player("Alice", -1m));
    }

    // ── Deposit ───────────────────────────────────────────────────────────────

    [Fact]
    public void Deposit_PositiveAmount_IncreasesBalance()
    {
        var player = new Player("Alice", 100m);
        player.Deposit(50m);
        Assert.Equal(150m, player.Balance);
    }

    // ── Withdraw ──────────────────────────────────────────────────────────────

    [Fact]
    public void Withdraw_ValidAmount_ReducesBalance()
    {
        var player = new Player("Alice", 100m);
        player.Withdraw(40m);
        Assert.Equal(60m, player.Balance);
    }

    [Fact]
    public void Withdraw_MoreThanBalance_ThrowsInvalidOperationException()
    {
        var player = new Player("Alice", 50m);
        Assert.Throws<InvalidOperationException>(() => player.Withdraw(100m));
    }

    // ── TryDebitBet ───────────────────────────────────────────────────────────

    [Fact]
    public void TryDebitBet_SufficientFunds_ReturnsTrueAndDebits()
    {
        var player = new Player("Alice", 100m);
        bool result = player.TryDebitBet(30m);
        Assert.True(result);
        Assert.Equal(70m, player.Balance);
    }

    [Fact]
    public void TryDebitBet_InsufficientFunds_ReturnsFalseAndDoesNotDebit()
    {
        var player = new Player("Alice", 20m);
        bool result = player.TryDebitBet(50m);
        Assert.False(result);
        Assert.Equal(20m, player.Balance);
    }

    // ── CreditWin ─────────────────────────────────────────────────────────────

    [Fact]
    public void CreditWin_PositiveAmount_IncreasesBalance()
    {
        var player = new Player("Alice", 100m);
        player.CreditWin(25m);
        Assert.Equal(125m, player.Balance);
    }
}