using Casino.Application;

namespace Casino.Infrastructure
{
    public class RandomNumberGenerator : IRandomNumberGenerator
    {
        public double NextDouble()
        {
            return Random.Shared.NextDouble();
        }
    }
}