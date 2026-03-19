using Casino.ConsoleApp;
using Casino.Domain;

namespace Casino.ConsoleApp.Tests;

public class PlayerWithdrawCommandTests
{
    private readonly IConsoleOutput _output;
    private readonly PlayerWithdrawCommand _sut;
    private readonly Player _player;

    public PlayerWithdrawCommandTests()
    {
        _output = Substitute.For<IConsoleOutput>();
        _sut = new PlayerWithdrawCommand(_output);
        _player = new Player("Alice", 100m);
    }

    [Fact]
    public void Execute_ValidWithdraw_CallsWriteSystemMessage()
    {
        _sut.Execute(_player, 30m);

        _output.Received(1).WriteSystemMessage(Arg.Is<string>(s => s.Contains("30")));
    }

    [Fact]
    public void Execute_ValidWithdraw_CallsWriteBalance()
    {
        _sut.Execute(_player, 30m);

        _output.Received(1).WriteBalance(70m); // 100 - 30
    }

    [Fact]
    public void Execute_MoreThanBalance_ThrowsInvalidOperationException()
    {
        // Wallet.Withdraw throws when amount > balance — propagates through the command
        Assert.Throws<InvalidOperationException>(() => _sut.Execute(_player, 200m));
    }

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
        Assert.Throws<ArgumentException>(() => _sut.Execute(_player, -10m));
    }
}
