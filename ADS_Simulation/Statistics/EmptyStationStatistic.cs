using System;
using System.Collections.Generic;
using System.Text;
using ADS_Simulation.NS_State;

namespace ADS_Simulation.Statistics
{
    class EmptyStationStatistic : Statistic
    {
        public override void measure(State state)
        {
            throw new NotImplementedException();
        }
    }
}
