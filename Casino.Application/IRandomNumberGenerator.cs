namespace Casino.Application
{
    public interface IRandomNumberGenerator
    {
        double NextDouble();
    }

    public static class RngExtensions
    {
        public static double NextDouble(this IRandomNumberGenerator rng, double min, double max)
        {
            return (rng.NextDouble() * (max - min)) + min;
        }
    }
}