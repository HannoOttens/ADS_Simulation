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
        static void initializeDistributions()
        {
            //TODO: Maak distributie objecten hier
        }


        /// <summary>
        /// The time it takes to clear a switch
        /// </summary>
        /// <returns></returns>
        public static int switchClearanceTime()
        {
            return 60;
        }

        /// <summary>
        /// The estimated driving time between stations.
        /// This is a lognormal distribution with the natural logarithm of the average driving time as mu and 0.1 as sigma
        /// </summary>
        /// <param name="averageForPart">Average driving time for this section</param>
        /// <returns></returns>
        public static int drivingTime(int averageForPart)
        {
            return (int)LogNormal.Sample(Math.Log(averageForPart), 0.1);
        }

        /// <summary>
        /// Time it takes for passengers to get in/out
        /// </summary>
        /// <param name="pOut">Passengers getting out</param>
        /// <param name="pIn">Passengers getting in</param>
        /// <returns></returns>
        public static int passengerExchangeTime(int pOut, int pIn)
        {
            //TODO: Implement
            return pOut + pIn;
        }

        /// <summary>
        /// Time until next passenger arrives
        /// </summary>
        /// <param name="time">Needed to get the correct poisson time range</param>
        /// <returns></returns>
        public static int timeUntilNextPassenger(int time)
        {
            throw new NotImplementedException();
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
