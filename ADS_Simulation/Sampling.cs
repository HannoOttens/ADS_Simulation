using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;

namespace ADS_Simulation
{
    /// <summary>
    /// Class for all time sampling
    /// </summary>
    static class Sampling
    {
        /// <summary>
        /// The time it takes to clear a switch
        /// </summary>
        /// <returns></returns>
        public static int switchClearanceTime()
        {
            return 60;
        }

        /// <summary>
        /// The estimated driving time between stations
        /// </summary>
        /// <returns></returns>
        public static int drivingTime() {
            return -1;
        }

        /// <summary>
        /// Time it takes for passengers to get in/out
        /// </summary>
        /// <param name="pOut">Passengers getting out</param>
        /// <param name="pIn">Passengers getting in</param>
        /// <returns></returns>
        public static int passengerExchangeTime(int pOut, int pIn)
        {
            return -1;
        }

        /// <summary>
        /// Time until next passenger arrives
        /// </summary>
        /// <returns></returns>
        public static int timeUntilNextPassenger()
        {
            return -1;
        }

        /// <summary>
        /// The time a station needs to be clear for before the next tram can arrive
        /// </summary>
        /// <returns></returns>
        public static int tramSafetyDistance()
        {
            return 40;
        }
    }
}
