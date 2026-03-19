using Casino.Application;
using NSubstitute;

namespace Casino.Application.Tests;

public class RngExtensionsTests
{
    private readonly IRandomNumberGenerator _rng;

    public RngExtensionsTests()
    {
        _rng = Substitute.For<IRandomNumberGenerator>();
    }

    [Fact]
    public void NextDouble_RngReturnsZero_ReturnsMin()
    {
        _rng.NextDouble().Returns(0.0);
        double result = _rng.NextDouble(2.0, 8.0);
        Assert.Equal(2.0, result); // 0*(8-2)+2 = 2
    }

    [Fact]
    public void NextDouble_RngReturnsOne_ReturnsMax()
    {
        _rng.NextDouble().Returns(1.0);
        double result = _rng.NextDouble(2.0, 8.0);
        Assert.Equal(8.0, result); // 1*(8-2)+2 = 8
    }

    [Fact]
    public void NextDouble_RngReturnsMidpoint_ScalesCorrectly()
    {
        _rng.NextDouble().Returns(0.5);
        double result = _rng.NextDouble(0.0, 10.0);
        Assert.Equal(5.0, result); // 0.5*10+0 = 5
    }

    [Fact]
    public void NextDouble_NegativeRange_ScalesCorrectly()
    {
        _rng.NextDouble().Returns(0.5);
        double result = _rng.NextDouble(-10.0, 0.0);
        Assert.Equal(-5.0, result); // 0.5*(0-(-10))+(-10) = -5
    }

    [Fact]
    public void NextDouble_MinEqualsMax_ReturnsMin()
    {
        _rng.NextDouble().Returns(0.75); // raw value is irrelevant when max-min=0
        double result = _rng.NextDouble(5.0, 5.0);
        Assert.Equal(5.0, result); // any_raw * 0 + 5 = 5
    }
}