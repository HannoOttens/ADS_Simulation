using ADS_Simulation.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADS_Simulation.NS_State
{
    public class Tram
    {
        public int id;
        public int passengerCount;

        public Tram(int id) {
            this.id = id;
            passengerCount = 0;
        }

        /// <summary>
        /// Check how much space there is left in the tram
        /// </summary>
        /// <returns>The amount of passengers that can still fit in the tram</returns>
        public int PassengerSpace()
        {
            return Config.c.tramCapacity - passengerCount;
        }

        /// <summary>
        /// Empty the tram
        /// </summary>
        /// <returns>The amount of passengers that left the tram</returns>
        public int EmptyPassengers()
        {
            int t = passengerCount;
            passengerCount = 0;
            return t;
        }

        /// <summary>
        /// Check if tram is full
        /// </summary>
        /// <returns>True if tram is full</returns>
        public bool IsFull()
        {
            return passengerCount >= Config.c.tramCapacity;
        }

        public void AddPassengers(int count)
        {
            passengerCount += count;
            if (passengerCount > Config.c.tramCapacity)
                throw new Exception($"Tram capacity exceeded in tram {id} by {Config.c.tramCapacity - passengerCount}");
        }
    }
}
