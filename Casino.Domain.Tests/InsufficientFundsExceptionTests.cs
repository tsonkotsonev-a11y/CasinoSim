namespace Casino.Domain.Tests;

public class InsufficientFundsExceptionTests
{
    [Fact]
    public void Constructor_SetsRequestedAmount()
    {
        var ex = new InsufficientFundsException(100m, 50m);
        Assert.Equal(100m, ex.RequestedAmount);
    }

    [Fact]
    public void Constructor_SetsBalance()
    {
        var ex = new InsufficientFundsException(100m, 50m);
        Assert.Equal(50m, ex.Balance);
    }

    [Fact]
    public void Constructor_MessageContainsAmountAndBalance()
    {
        var ex = new InsufficientFundsException(100m, 50m);
        Assert.Contains("100", ex.Message);
        Assert.Contains("50", ex.Message);
    }

    [Fact]
    public void IsSubclassOf_Exception()
    {
        Assert.IsAssignableFrom<Exception>(new InsufficientFundsException(1m, 0m));
    }
}
