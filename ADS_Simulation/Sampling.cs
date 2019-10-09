using System;
using ADS_Simulation.Configuration;
using ADS_Simulation.NS_State;
using MathNet.Numerics.Distributions;

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
        /// The estimated driving time between stations.
        /// This is a lognormal distribution with the natural logarithm of the average driving time as mu and 0.1 as sigma
        /// </summary>
        /// <param name="averageForPart">Average driving time for this section</param>
        /// <returns></returns>
        public static int drivingTime(int averageForPart)
        {
            return (int)LogNormal.Sample(Math.Log(averageForPart), Config.c.sdDrivingTimes);
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
            var g = Gamma.Sample(2, 2/d);
            return (int)Math.Max(0.8 * d, g);
        }

        /// <summary>
        /// Time until next passenger arrives
        /// </summary>
        /// <param name="time">Needed to get the correct poisson time range</param>
        /// <param name="station">Station where the passenger arrives</param>
        /// <returns></returns>
        public static int timeUntilNextPassenger(int time, int stationIndex)
        {
            int idxT = Math.Min(time / 15, Config.c.transferTimes[stationIndex].arivalRate.Length - 1);
            double mean = Config.c.transferTimes[stationIndex].arivalRate[idxT];
            return Poisson.Sample(Math.Max(0.1, mean));
        }

        /// <summary>
        /// The time a station needs to be clear for before the next tram can arrive
        /// </summary>
        /// <returns></returns>
        public static int tramSafetyDistance()
        {
            return 40;
        }

        internal static int unboardingPassengerCount(int time, int stationIndex)
        {
            int idxT = Math.Min(time / 15, Config.c.transferTimes[stationIndex].arivalRate.Length - 1);
            double mean = Config.c.transferTimes[stationIndex].averageExit[idxT];
            double sd = Config.c.transferTimes[stationIndex].standardDeviationExit[idxT];

            double log_mean = Math.Max(0.000001, Math.Log(mean));
            double log_sd = Math.Max(0.000001, Math.Log(sd));

            return (int)LogNormal.Sample(log_mean, log_sd);
        }
    }
}
