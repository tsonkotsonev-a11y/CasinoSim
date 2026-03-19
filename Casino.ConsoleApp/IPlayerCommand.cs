using Casino.Domain;

namespace Casino.ConsoleApp
{
    public interface IPlayerCommand
    {
        public string Command { get; }
        public void Execute(Player player, decimal? amount);
    }

    public abstract class PlayerCommandBase : IPlayerCommand
    {
        protected readonly IConsoleOutput _consoleOutput;
        protected virtual bool RequiresAmount => true;

        public string Command { get; }

        public PlayerCommandBase(IConsoleOutput consoleOutput, string command)
        {
            _consoleOutput = consoleOutput;
            Command = command;
        }

        public void Execute(Player player, decimal? amount)
        {
            ValidateAmount(amount);
            ExecuteCommand(player, amount ?? 0);
            _consoleOutput.WriteBalance(player.Balance);
        }

        protected abstract void ExecuteCommand(Player player, decimal amount);
        protected void ValidateAmount(decimal? amount)
        {
            if (!RequiresAmount && amount.HasValue)
            {
                throw new ArgumentException($"{Command} command does not take an amount.");
            }
            if (RequiresAmount && (amount == null || amount <= 0))
            {
                throw new ArgumentException("Amount must be greater than zero.");
            }
        }
    }
}