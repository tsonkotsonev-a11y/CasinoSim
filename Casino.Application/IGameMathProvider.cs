namespace Casino.Application
{
    public interface IGameMathProvider
    {
        IReadOnlyList<BetOutcomeTier> GetBetOutcomeTiers();
    }
}