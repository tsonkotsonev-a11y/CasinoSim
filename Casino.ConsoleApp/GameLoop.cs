using Casino.Domain;

namespace Casino.ConsoleApp
{
    public class GameLoop
    {
        private readonly IReadOnlyList<IPlayerCommand> _commands;
        private readonly IConsoleTerminal _terminal;

        public GameLoop(IConsoleTerminal terminal, IEnumerable<IPlayerCommand> commands)
        {
            _commands = commands.ToList();
            _terminal = terminal;
        }

        public void Run(Player player)
        {
            string allCommands = string.Join(", ", _commands.Select(c => c.Command));

            _terminal.WriteSystemMessage($"Welcome, {player.Name}!");
            while (true)
            {
                _terminal.WriteHeader($"Please submit action ({allCommands} followed by amount):");
                try
                {
                    PlayerInput input = _terminal.ReadInput();

                    IPlayerCommand? command = _commands.FirstOrDefault(c => c.Command.Equals(input.Command, StringComparison.OrdinalIgnoreCase));
                    if (command == null)
                    {
                        _terminal.WriteRejection($"Unknown command: {input.Command}");
                        continue;
                    }
                    else
                    {
                        command.Execute(player, input.Amount);
                    }
                }
                catch (Exception ex)
                {
                    _terminal.WriteRejection(ex.Message);
                }
            }
        }
    }
}
