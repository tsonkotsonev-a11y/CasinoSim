using System.Globalization;

namespace Casino.ConsoleApp
{
    public class ConsoleTerminal : IConsoleTerminal
    {
        public void WriteHeader(string title) => WriteColor($"= {title} =", ConsoleColor.DarkCyan);
        public void WriteSystemMessage(string message) => WriteColor($"{message}", ConsoleColor.DarkGray);
        public void WriteWin(decimal payout) => WriteColor($"Congrats, you won {payout:C}!", ConsoleColor.Green);
        public void WriteLoss() => WriteColor($"No luck this time!", ConsoleColor.DarkYellow);
        public void WriteRejection(string reason) => WriteColor($"Transaction rejected: {reason}!", ConsoleColor.Red);
        public void WriteBalance(decimal amount) => WriteColor($"Your current balance is : {amount:C}\n", ConsoleColor.Yellow);

        private void WriteColor(string msg, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public PlayerInput ReadInput()
        {
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Input cannot be empty.");
            }

            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0 || parts.Length > 2) throw new ArgumentException("Invalid command format. Expected: <Command> <Amount>");

            string command = parts[0].ToUpperInvariant();

            if (parts.Length == 1)
            {
                return new PlayerInput(command, null);
            }

            if (!decimal.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount))
            {
                throw new ArgumentException("Invalid amount. Please enter a valid number.");
            }

            return new PlayerInput(command, amount);
        }
    }
}
