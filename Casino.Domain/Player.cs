namespace Casino.Domain
{
    public class Player
    {
        private readonly Wallet _wallet;
        public string Name { get; }

        public Player(string name, decimal initialBalance)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required.");
            if (initialBalance < 0) throw new ArgumentException("Initial balance cannot be negative.");

            Name = name;
            _wallet = new Wallet(initialBalance);
        }

        public decimal Balance => _wallet.Balance;

        public void Deposit(decimal amount) => _wallet.Deposit(amount);
        public void Withdraw(decimal amount) => _wallet.Withdraw(amount);
        public bool TryDebitBet(decimal amount) => _wallet.TryDebitBet(amount);
        public void CreditWin(decimal amount) => _wallet.CreditWin(amount);
    }
}