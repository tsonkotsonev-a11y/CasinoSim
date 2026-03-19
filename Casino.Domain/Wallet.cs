namespace Casino.Domain
{
    internal class Wallet
    {
        private readonly object _lock = new();
        public decimal Balance { get; private set; }

        public Wallet(decimal initialBalance)
        {
            if (initialBalance < 0) throw new ArgumentException("Initial balance cannot be negative.");
            Balance = initialBalance;
        }

        public void Deposit(decimal amount)
        {
            ValidateAmount(amount);
            lock (_lock)
            {
                Balance += amount;
            }
        }

        public void Withdraw(decimal amount)
        {
            ValidateAmount(amount);
            lock (_lock)
            {
                if (amount > Balance) throw new InvalidOperationException("Insufficient funds.");
                Balance -= amount;
            }
        }

        public bool TryDebitBet(decimal amount)
        {
            ValidateAmount(amount);
            lock (_lock)
            {
                if (amount > Balance) return false;
                Balance -= amount;
                // Logic for Gaming Audit / Bonus interaction...
            }
            return true;
        }

        public void CreditWin(decimal amount)
        {
            if (amount == 0) return;
            ValidateAmount(amount);
            lock (_lock)
            {
                Balance += amount;
            }
        }

        private void ValidateAmount(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be positive.");
        }
    }
}