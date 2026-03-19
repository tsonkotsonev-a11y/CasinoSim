using Casino.Application;
using Casino.ConsoleApp;
using Casino.Domain;

namespace Casino.ConsoleApp.Tests;

public class PlayerBetCommandTests
{
    private readonly IConsoleOutput _output;
    private readonly IBettingService _bettingService;
    private readonly PlayerBetCommand _sut;
    private readonly Player _player;

    public PlayerBetCommandTests()
    {
        _output = Substitute.For<IConsoleOutput>();
        _bettingService = Substitute.For<IBettingService>();
        _sut = new PlayerBetCommand(_output, _bettingService);
        _player = new Player("Alice", 100m);
    }

    // ── Win ───────────────────────────────────────────────────────────────────

    [Fact]
    public void Execute_WinBet_CallsWriteWin()
    {
        _bettingService.PlaceBet(_player, 5m).Returns(BetResult.CreateSuccess(5m, 10m));

        _sut.Execute(_player, 5m);

        _output.Received(1).WriteWin(10m);
    }

    [Fact]
    public void Execute_WinBet_DoesNotCallWriteLoss()
    {
        _bettingService.PlaceBet(_player, 5m).Returns(BetResult.CreateSuccess(5m, 10m));

        _sut.Execute(_player, 5m);

        _output.DidNotReceive().WriteLoss();
    }

    [Fact]
    public void Execute_WinBet_CallsWriteBalance()
    {
        _bettingService.PlaceBet(_player, 5m).Returns(BetResult.CreateSuccess(5m, 10m));

        _sut.Execute(_player, 5m);

        _output.Received(1).WriteBalance(_player.Balance);
    }

    // ── Lose ──────────────────────────────────────────────────────────────────

    [Fact]
    public void Execute_LoseBet_CallsWriteLoss()
    {
        _bettingService.PlaceBet(_player, 5m).Returns(BetResult.CreateSuccess(5m, 0m));

        _sut.Execute(_player, 5m);

        _output.Received(1).WriteLoss();
    }

    [Fact]
    public void Execute_LoseBet_DoesNotCallWriteWin()
    {
        _bettingService.PlaceBet(_player, 5m).Returns(BetResult.CreateSuccess(5m, 0m));

        _sut.Execute(_player, 5m);

        _output.DidNotReceive().WriteWin(Arg.Any<decimal>());
    }

    [Fact]
    public void Execute_LoseBet_CallsWriteBalance()
    {
        _bettingService.PlaceBet(_player, 5m).Returns(BetResult.CreateSuccess(5m, 0m));

        _sut.Execute(_player, 5m);

        _output.Received(1).WriteBalance(_player.Balance);
    }

    // ── Failure — InsufficientFunds ───────────────────────────────────────────

    [Fact]
    public void Execute_InsufficientFunds_CallsWriteRejection()
    {
        _bettingService.PlaceBet(_player, 5m).Returns(BetResult.CreateFailure(BetStatus.InsufficientFunds));

        _sut.Execute(_player, 5m);

        _output.Received(1).WriteRejection("insufficient balance");
    }

    [Fact]
    public void Execute_InsufficientFunds_DoesNotCallWriteWinOrLoss()
    {
        _bettingService.PlaceBet(_player, 5m).Returns(BetResult.CreateFailure(BetStatus.InsufficientFunds));

        _sut.Execute(_player, 5m);

        _output.DidNotReceive().WriteWin(Arg.Any<decimal>());
        _output.DidNotReceive().WriteLoss();
    }

    // ── Failure — InvalidBetAmount ────────────────────────────────────────────

    [Fact]
    public void Execute_InvalidBetAmount_CallsWriteRejection()
    {
        _bettingService.PlaceBet(_player, 5m).Returns(BetResult.CreateFailure(BetStatus.InvalidBetAmount));

        _sut.Execute(_player, 5m);

        _output.Received(1).WriteRejection("invalid bet amount");
    }

    // ── Amount validation (base class) ────────────────────────────────────────

    [Fact]
    public void Execute_NullAmount_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _sut.Execute(_player, null));
    }

    [Fact]
    public void Execute_ZeroAmount_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _sut.Execute(_player, 0m));
    }

    [Fact]
    public void Execute_NegativeAmount_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _sut.Execute(_player, -1m));
    }
}
