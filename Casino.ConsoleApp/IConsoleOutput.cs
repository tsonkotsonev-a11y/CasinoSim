using Casino.Domain;

namespace Casino.ConsoleApp
{
    public interface IConsoleOutput
    {
        public void WriteHeader(string title);
        public void WriteSystemMessage(string message);
        public void WriteWin(decimal payout);
        public void WriteLoss();
        public void WriteRejection(string reason);
        public void WriteBalance(decimal amount);
    }

    public interface IConsoleTerminal : IConsoleInput, IConsoleOutput;
}