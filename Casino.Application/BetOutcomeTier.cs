namespace Casino.Application
{
    public record BetOutcomeTier(string Name, double Probability, decimal MinMultiplier, decimal MaxMultiplier);
}