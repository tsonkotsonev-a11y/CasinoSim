namespace Casino.ConsoleApp
{
    public record PlayerInput(string Command, decimal? Amount);

    public interface IConsoleInput
    {
        public PlayerInput ReadInput();
    }
}
