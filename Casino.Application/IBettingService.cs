using Casino.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casino.Application
{
    public interface IBettingService
    {
        public BetResult PlaceBet(Player player, decimal amount);
    }
}
