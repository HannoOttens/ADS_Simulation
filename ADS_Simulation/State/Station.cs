using System;
using System.Collections.Generic;
using System.Text;

namespace ADS_Simulation.NS_State
{
    public class Station
    {
        public Queue<int> waitingPassengers;
        public Station()
        {
            waitingPassengers = new Queue<int>();
        }
    }
}
