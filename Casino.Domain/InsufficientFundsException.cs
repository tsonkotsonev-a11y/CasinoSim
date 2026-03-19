namespace Casino.Domain
{
    public class InsufficientFundsException(decimal requestedAmount, decimal balance) : Exception($"Insufficient funds. Requested: {requestedAmount}, Available: {balance}")
    {
        public decimal RequestedAmount { get; } = requestedAmount;
        public decimal Balance { get; } = balance;
    }
}