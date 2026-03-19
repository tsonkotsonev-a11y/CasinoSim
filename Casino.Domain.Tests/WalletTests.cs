namespace Casino.Domain.Tests;

// Wallet is internal — accessible here via [assembly: InternalsVisibleTo("Casino.Domain.Tests")]
// in Casino.Domain.csproj.
public class WalletTests
{
    // Balancing performance and reliability in concurrent tests is tricky. Too few attempts may miss race conditions, too many may slow down the test suite.
    private const int TotalConcurrentAttempts = 1000;

    // ── Constructor ───────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_ZeroBalance_IsValid()
    {
        var wallet = new Wallet(0m);
        Assert.Equal(0m, wallet.Balance);
    }

    [Fact]
    public void Constructor_NegativeBalance_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Wallet(-1m));
    }

    // ── Deposit ───────────────────────────────────────────────────────────────

    [Fact]
    public void Deposit_ValidAmount_AddsToBalance()
    {
        var wallet = new Wallet(100m);
        wallet.Deposit(50m);
        Assert.Equal(150m, wallet.Balance);
    }

    [Fact]
    public void Deposit_ZeroAmount_ThrowsArgumentException()
    {
        var wallet = new Wallet(100m);
        Assert.Throws<ArgumentException>(() => wallet.Deposit(0m));
    }

    [Fact]
    public void Deposit_NegativeAmount_ThrowsArgumentException()
    {
        var wallet = new Wallet(100m);
        Assert.Throws<ArgumentException>(() => wallet.Deposit(-10m));
    }

    [Fact]
    public void Deposit_ConcurrentAccess_MaintainsExactBalance()
    {
        // Arrange: An empty wallet
        var sut = new Wallet(0m);

        // Act: Concurrent barrage of 1$ deposits
        Parallel.For(0, TotalConcurrentAttempts, _ =>
        {
            sut.Deposit(1m);
        });

        // Assert: Every single deposit must be accounted for
        Assert.Equal(TotalConcurrentAttempts, sut.Balance);
    }

    // ── Withdraw ──────────────────────────────────────────────────────────────

    [Fact]
    public void Withdraw_ValidAmount_ReducesBalance()
    {
        var wallet = new Wallet(100m);
        wallet.Withdraw(30m);
        Assert.Equal(70m, wallet.Balance);
    }

    [Fact]
    public void Withdraw_ExactBalance_ReducesToZero()
    {
        var wallet = new Wallet(100m);
        wallet.Withdraw(100m);
        Assert.Equal(0m, wallet.Balance);
    }

    [Fact]
    public void Withdraw_MoreThanBalance_ThrowsInvalidOperationException()
    {
        var wallet = new Wallet(50m);
        Assert.Throws<InvalidOperationException>(() => wallet.Withdraw(100m));
    }

    [Fact]
    public void Withdraw_ZeroAmount_ThrowsArgumentException()
    {
        var wallet = new Wallet(100m);
        Assert.Throws<ArgumentException>(() => wallet.Withdraw(0m));
    }

    [Fact]
    public void Withdraw_NegativeAmount_ThrowsArgumentException()
    {
        var wallet = new Wallet(100m);
        Assert.Throws<ArgumentException>(() => wallet.Withdraw(-5m));
    }

    [Fact]
    public void Withdraw_ConcurrentAccess_ProcessesValidAndThrowsForInvalid()
    {
        // Arrange: A wallet with 50 EUR
        var sut = new Wallet(50m);

        int successfulWithdrawals = 0;
        int expectedExceptions = 0;

        // Act: 100 concurrent threads trying to withdraw 10 EUR each
        Parallel.For(0, TotalConcurrentAttempts, _ =>
        {
            try
            {
                sut.Withdraw(10m);
                Interlocked.Increment(ref successfulWithdrawals);
            }
            catch (InvalidOperationException)
            {
                Interlocked.Increment(ref expectedExceptions);
            }
        });

        // Assert: Exactly 5 threads drain the 50 EUR (5 * 10 = 50). The others failed as balance was drained.
        Assert.Equal(0m, sut.Balance);
        Assert.Equal(5, successfulWithdrawals);
        Assert.Equal(TotalConcurrentAttempts - 5, expectedExceptions);
    }

    // ── TryDebitBet ───────────────────────────────────────────────────────────

    [Fact]
    public void TryDebitBet_SufficientFunds_ReturnsTrueAndDebits()
    {
        var wallet = new Wallet(100m);
        bool result = wallet.TryDebitBet(60m);
        Assert.True(result);
        Assert.Equal(40m, wallet.Balance);
    }

    [Fact]
    public void TryDebitBet_ExactBalance_ReturnsTrueAndReducesToZero()
    {
        var wallet = new Wallet(100m);
        bool result = wallet.TryDebitBet(100m);
        Assert.True(result);
        Assert.Equal(0m, wallet.Balance);
    }

    [Fact]
    public void TryDebitBet_InsufficientFunds_ReturnsFalseAndDoesNotDebit()
    {
        var wallet = new Wallet(50m);
        bool result = wallet.TryDebitBet(100m);
        Assert.False(result);
        Assert.Equal(50m, wallet.Balance);
    }

    [Fact]
    public void TryDebitBet_ZeroAmount_ThrowsArgumentException()
    {
        var wallet = new Wallet(100m);
        Assert.Throws<ArgumentException>(() => wallet.TryDebitBet(0m));
    }

    [Fact]
    public void TryDebitBet_ConcurrentAccess_MaintainsExactBalance()
    {
        // Arrange: A wallet with 50 EUR
        var sut = new Wallet(50m);

        int successfulDebits = 0;
        int failedDebits = 0;

        // Act parrallel barrage
        Parallel.For(0, TotalConcurrentAttempts, _ =>
        {
            if (sut.TryDebitBet(1m))
            {
                Interlocked.Increment(ref successfulDebits);
            }
            else
            {
                Interlocked.Increment(ref failedDebits);
            }
        });

        // Assert
        Assert.Equal(0m, sut.Balance);
        Assert.Equal(50, successfulDebits);
        Assert.Equal(TotalConcurrentAttempts - 50, failedDebits);
    }

    [Fact]
    public void TryDebitBet_NegativeAmount_ThrowsArgumentException()
    {
        var wallet = new Wallet(100m);
        Assert.Throws<ArgumentException>(() => wallet.TryDebitBet(-5m));
    }

    // ── CreditWin ─────────────────────────────────────────────────────────────

    [Fact]
    public void CreditWin_PositiveAmount_IncreasesBalance()
    {
        var wallet = new Wallet(100m);
        wallet.CreditWin(50m);
        Assert.Equal(150m, wallet.Balance);
    }

    [Fact]
    public void CreditWin_ZeroAmount_LeavesBalanceUnchanged()
    {
        // Crediting 0 is a no-op
        var wallet = new Wallet(100m);
        wallet.CreditWin(0m);
        Assert.Equal(100m, wallet.Balance);
    }

    [Fact]
    public void CreditWin_ConcurrentAccess_ProcessesValidWinsAndIgnoresZeroes()
    {
        var sut = new Wallet(0m);

        // Act: Parrallel barrage of concurrent CreditWins
        // Even threads win 1 EUR. Odd threads win 0 EUR.
        Parallel.For(0, TotalConcurrentAttempts, i =>
        {
            decimal winAmount = (i % 2 == 0) ? 1m : 0m;
            sut.CreditWin(winAmount);
        });

        // Assert: TotalConcurrentAttempts / 2 threads won 1 EUR. The other 50 were no-ops.
        Assert.Equal(TotalConcurrentAttempts / 2, sut.Balance);
    }
}