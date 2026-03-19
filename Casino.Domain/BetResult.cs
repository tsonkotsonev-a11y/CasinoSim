namespace Casino.Domain
{
    public enum BetStatus
    {
        Success,
        InsufficientFunds,
        InvalidBetAmount
    }

    public record BetResult
    {
        public BetStatus Status { get; init; }
        public decimal BetAmount { get; init; }
        public decimal Payout { get; init; }
        public bool IsSuccess => Status == BetStatus.Success;

        private BetResult(BetStatus status, decimal betAmount, decimal payout)
        {
            Status = status;
            BetAmount = betAmount;
            Payout = payout;
        }

        public static BetResult CreateSuccess(decimal betAmount, decimal payout) => new(BetStatus.Success, betAmount, payout);

        public static BetResult CreateFailure(BetStatus status) => new(status, 0m, 0m);
    }
}