namespace Casino.Application
{
    public class GameMathProvider : IGameMathProvider
    {
        public IReadOnlyList<BetOutcomeTier> GetBetOutcomeTiers()
        {
            List<BetOutcomeTier> tiers =
            [
                new("Lose", 50, 0, 0m), // 50% chance to lose
                new("Win", 40, 0.01m, 2m), // 40% chance to win up to x2 the bet. Min 0.01m: Uses Loss Disguised as a Win to hit 40% "win" frequency without destroying the house RTP
                new("Jackpot", 10, 2m, 10m), // 10% chance to win x2 to x10 the bet
            ];
            return tiers;
        }
    }
}