using System;
using System.Collections.Generic;
using System.Text;

namespace ADS_Simulation.NS_State
{
    public class Station
    {
        public string name;

        // TODO split queue depending on direction
        public Queue<int> waitingPassengers; // Passengers waiting for trip to Central Station
        public Station(string name)
        {
            this.name = name;
            waitingPassengers = new Queue<int>();
        }
    }
}
