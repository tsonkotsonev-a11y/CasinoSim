using Casino.Application;
using Casino.ConsoleApp;
using Casino.Domain;
using Casino.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// The Registry
services.AddSingleton<IConsoleTerminal, ConsoleTerminal>();
services.AddSingleton<IConsoleOutput>(provider => provider.GetRequiredService<IConsoleTerminal>());
services.AddSingleton<IConsoleInput>(provider => provider.GetRequiredService<IConsoleTerminal>());

services.AddSingleton<IBettingService, BettingService>();
services.AddTransient<IPlayerCommand, PlayerDepositCommand>();
services.AddTransient<IPlayerCommand, PlayerWithdrawCommand>();
services.AddTransient<IPlayerCommand, PlayerBetCommand>();
services.AddTransient<IPlayerCommand, PlayerExitCommand>();
services.AddTransient<IRandomNumberGenerator, RandomNumberGenerator>();
services.AddTransient<IGameMathProvider, GameMathProvider>();
services.AddSingleton<GameLoop>();

var provider = services.BuildServiceProvider();

// Resolve and Run
var loop = provider.GetRequiredService<GameLoop>();

Player player = new("You", 0m);
loop.Run(player);

/*Console.WriteLine($"Player: {player.Name}, Placing 10$ bet");


for(int i = 0; i < 10000; i++)
    bettingService.PlaceBet(player, 1m);
Console.WriteLine($"After 100000 bets: Player: {player.Name}, Wallet Balance: {player.Balance:C}");

Console.ReadLine();
*/