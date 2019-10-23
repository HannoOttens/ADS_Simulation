using System;
using ADS_Simulation.Configuration;
using MathNet.Numerics.Distributions;

namespace ADS_Simulation
{
    /// <summary>
    /// Class for all time sampling
    /// </summary>
    static class Sampling
    {
        /// <summary>
        /// The estimated driving time between stations.
        /// This could either be a simplified version for validation, or a lognormal distribution.
        /// </summary>
        /// <param name="averageForPart">Average driving time for this section</param>
        /// <returns></returns>
        public static int drivingTime(int averageForPart)
        {
            if (Config.c.simplifiedDrivingTimes)
            {
                var x = DiscreteUniform.Sample(0, 100);
                if (x <= 40)
                    return (int) (0.8 * averageForPart);
                if (x <= 70)
                    return averageForPart;
                if (x <= 90)
                    return (int) (1.2 * averageForPart);
                return (int) (1.4 * averageForPart);
            }
            return (int) LogNormal.Sample(Math.Log(averageForPart), Config.c.sdDrivingTimes);
        }

        /// <summary>
        /// Time it takes for passengers to get in/out
        /// </summary>
        /// <param name="pOut">Passengers getting out</param>
        /// <param name="pIn">Passengers getting in</param>
        /// <returns></returns>
        public static int passengerExchangeTime(int pOut, int pIn)
        {
            var d = 12.5 + 0.22 * pIn + 0.13 * pOut;
            var g = Gamma.Sample(2, 2 / d);
            return (int) Math.Max(0.8 * d, g);
        }

        /// <summary>
        /// Determine the number of arrivals in time window (15 minutes) and their arrival times
        /// </summary>
        /// <param name="mean">Average number of arrivals</param>
        /// <returns></returns>
        public static int[] arrivingPassengers(double mean)
        {
            var arrivals = Poisson.Sample(mean);
            var times = new int[arrivals];
            for (int i = 0; i < arrivals; i++)
                times[i] = DiscreteUniform.Sample(0, 899);
            return times;
        }

        /// <summary>
        /// Calculate number of unboarding passengers
        /// </summary>
        /// <param name="time">Current time</param>
        /// <param name="stationIndex">Index of station where passengers unboard</param>
        /// <param name="passengers">Current passengers in tram</param>
        /// <returns></returns>
        internal static int unboardingPassengerCount(int time, int stationIndex, int passengers)
        {
            if (passengers == 0)
                return 0; // Nothing to unboard

            int idxT = Math.Min(time / 900, Config.c.transferTimes[stationIndex].arrivalRate.Length - 1);
            var averageExit = Config.c.transferTimes[stationIndex].averageExit[idxT];
            if (averageExit.Equals(0))
                return 0; //No passengers expecting to leave

            // Calculate expected passengers based on data from previous stations on the route
            double expectedPassengers = 0;
            for (var i = stationIndex > 8 ? 8 : 0; i < stationIndex; i++)
            {
                expectedPassengers += Config.c.transferTimes[i].arrivalRate[idxT];
                if(i != 8) // Passengers exiting on CS have left on the previous trip
                    expectedPassengers -= Config.c.transferTimes[i].averageExit[idxT];
            }
            if (expectedPassengers <= 0)
                return 0; // No passengers expected in tram

            var mean = passengers * (averageExit / expectedPassengers);
            var sd = Config.c.transferTimes[stationIndex].standardDeviationExit[idxT];
            if (sd.Equals(0))
                return (int)mean; //Avoid division by zero 
            return (int) Gamma.Sample((mean * mean) / (sd * sd), mean / (sd * sd));
        }
    }
}
