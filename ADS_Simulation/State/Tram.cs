using ADS_Simulation.Configuration;
using System;
using System.Diagnostics;

namespace ADS_Simulation.NS_State
{
    public class Tram
    {
        public int id;
        public int passengerCount;
        private bool readyForDeparture;

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

        internal bool IsReadyForDeparture()
        {
            return readyForDeparture;
        }

        /// <summary>
        /// Empty the tram
        /// </summary>
        /// <returns>The amount of passengers that left the tram</returns>
        public int EmptyPassengers(int maxUnboard)
        {
            int unboarding = Math.Min(maxUnboard, passengerCount);
            passengerCount -= unboarding;
            return unboarding;
        }

        /// <summary>
        /// Mark a tram as ready to depart
        /// </summary>
        public void ReadyForDeparture()
        {
            Trace.Assert(!readyForDeparture, $"Tried to mark tram {id} as 'ready for departure', but tram was already marked ready.");
            readyForDeparture = true;
        }

        /// <summary>
        /// Reset the ready for departure flag
        /// </summary>
        public void ResetReadyForDeparture()
        {
            Trace.Assert(readyForDeparture, $"Tried to mark tram {id} as 'unready for departure', but tram wasn't marked ready.");
            readyForDeparture = false;
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
                throw new Exception($"Tram capacity exceeded in tram {id} by {passengerCount - Config.c.tramCapacity}");
        }
    }
}
