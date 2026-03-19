using Casino.Application;
using Casino.Domain;

namespace Casino.ConsoleApp
{
    public class PlayerDepositCommand : PlayerCommandBase
    {
        public PlayerDepositCommand(IConsoleOutput consoleOutput) : base(consoleOutput, "deposit") { }

        protected override void ExecuteCommand(Player player, decimal amount)
        {
            player.Deposit(amount);
            _consoleOutput.WriteSystemMessage($"Your deposit of {amount:C} was successful.");
        }
    }

    public class PlayerWithdrawCommand : PlayerCommandBase
    {
        public PlayerWithdrawCommand(IConsoleOutput consoleOutput) : base(consoleOutput, "withdraw") { }

        protected override void ExecuteCommand(Player player, decimal amount)
        {
            player.Withdraw(amount);
            _consoleOutput.WriteSystemMessage($"Your withdrawal of {amount:C} was successful.");
        }
    }

    public class PlayerBetCommand : PlayerCommandBase
    {
        private readonly IBettingService _bettingService;

        public PlayerBetCommand(IConsoleOutput consoleOutput, IBettingService bettingService) : base(consoleOutput, "bet")
        {
            _bettingService = bettingService;
        }

        protected override void ExecuteCommand(Player player, decimal amount)
        {
            BetResult result = _bettingService.PlaceBet(player, amount);
            if (result.IsSuccess)
            {
                if (result.Payout > 0)
                {
                    _consoleOutput.WriteWin(result.Payout);
                }
                else
                {
                    _consoleOutput.WriteLoss();
                }
            }
            else
            {
                string failReason = "unknown";
                switch (result.Status)
                {
                    case BetStatus.InsufficientFunds:
                        failReason = "insufficient balance";
                        break;
                    case BetStatus.InvalidBetAmount:
                        failReason = "invalid bet amount";
                        break;
                }

                _consoleOutput.WriteRejection(failReason);
            }
        }
    }

    public class PlayerExitCommand : PlayerCommandBase
    {
        protected override bool RequiresAmount => false;

        public PlayerExitCommand(IConsoleOutput consoleOutput) : base(consoleOutput, "exit") { }
        protected override void ExecuteCommand(Player player, decimal amount)
        {
            // Validation for no amount is performed in the base class
            _consoleOutput.WriteSystemMessage("Thank you for playing! Hope to see you again soon.");
            Environment.Exit(0);
        }
    }
}