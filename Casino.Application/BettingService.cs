using Casino.Domain;

namespace Casino.Application
{
    public class BettingService : IBettingService
    {
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly IGameMathProvider _gameMathProvider;

        private const decimal MinBetAmount = 1m;
        private const decimal MaxBetAmount = 10m;

        public BettingService(IRandomNumberGenerator randomNumberGenerator, IGameMathProvider gameMathProvider)
        {
            _randomNumberGenerator = randomNumberGenerator;
            _gameMathProvider = gameMathProvider;
        }

        public BetResult PlaceBet(Player player, decimal amount)
        {
            if (amount < MinBetAmount || amount > MaxBetAmount)
            {
                return BetResult.CreateFailure(BetStatus.InvalidBetAmount);
            }
            if (!player.TryDebitBet(amount))
            {
                return BetResult.CreateFailure(BetStatus.InsufficientFunds);
            }
            decimal payout = SimulateBetOutcome(amount);
            if (payout > 0)
            {
                player.CreditWin(payout);
            }
            return BetResult.CreateSuccess(amount, payout);
        }

        private decimal SimulateBetOutcome(decimal amount)
        {
            IReadOnlyList<BetOutcomeTier> tiers = _gameMathProvider.GetBetOutcomeTiers();
            double randomValue = _randomNumberGenerator.NextDouble(0, 100);
            double currentProbability = 0.0;
            foreach (var tier in tiers)
            {
                currentProbability += tier.Probability;
                if (randomValue <= currentProbability)
                {
                    decimal multiplier = CalculateMultiplier(tier);
                    return amount * multiplier;
                }
            }

            return 0;
        }

        private decimal CalculateMultiplier(BetOutcomeTier tier)
        {
            if (tier.MinMultiplier == tier.MaxMultiplier) return tier.MinMultiplier; // Fast-fail for Lose tier

            double scaledRoll = _randomNumberGenerator.NextDouble((double)tier.MinMultiplier, (double)tier.MaxMultiplier);
            return Math.Round((decimal)scaledRoll, 2, MidpointRounding.ToEven);
        }
    }
}