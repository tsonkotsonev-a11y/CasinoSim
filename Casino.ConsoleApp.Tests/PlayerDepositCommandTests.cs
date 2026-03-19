using Casino.ConsoleApp;
using Casino.Domain;

namespace Casino.ConsoleApp.Tests;

public class PlayerDepositCommandTests
{
    private readonly IConsoleOutput _output;
    private readonly PlayerDepositCommand _sut;
    private readonly Player _player;

    public PlayerDepositCommandTests()
    {
        _output = Substitute.For<IConsoleOutput>();
        _sut = new PlayerDepositCommand(_output);
        _player = new Player("Alice", 100m);
    }

    [Fact]
    public void Execute_ValidDeposit_CallsWriteSystemMessage()
    {
        _sut.Execute(_player, 50m);

        _output.Received(1).WriteSystemMessage(Arg.Is<string>(s => s.Contains("50")));
    }

    [Fact]
    public void Execute_ValidDeposit_CallsWriteBalance()
    {
        _sut.Execute(_player, 50m);

        _output.Received(1).WriteBalance(150m); // 100 + 50
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
