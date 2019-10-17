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
        /// This is a lognormal distribution with the natural logarithm of the average driving time as mu and 0.1 as sigma
        /// </summary>
        /// <param name="averageForPart">Average driving time for this section</param>
        /// <returns></returns>
        public static int drivingTime(int averageForPart)
        {
            if (Config.c.simplifiedDrivingTimes)
            {
                var x = ContinuousUniform.Sample(0, 1);
                if (x >= 0.6)
                    return (int)(0.8 * averageForPart);
                if (x >= 0.3)
                    return averageForPart;
                if (x >= 0.1)
                    return (int)(1.2 * averageForPart);
                return (int)(1.4 * averageForPart);
            }
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
        /// Determine the number of arrivals in time window (15 minutes) and their arrival times
        /// </summary>
        /// <param name="mean">Average number of arrivals</param>
        /// <returns></returns>
        public static int[] arrivingPassengers(double mean)
        {
            var arrivals = Poisson.Sample(mean);
            var times = new int[arrivals];
            for (int i = 0; i < arrivals; i++)
                times[i] = DiscreteUniform.Sample(0, 900);
            return times;
        }

        /// <summary>
        /// Calculate number of unboarding passengers
        /// </summary>
        /// <param name="time">Current time</param>
        /// <param name="stationIndex">Index of station where passengers unboard</param>
        /// <returns></returns>
        internal static int unboardingPassengerCount(int time, int stationIndex)
        {
            int idxT = Math.Min(time / 900, Config.c.transferTimes[stationIndex].arrivalRate.Length - 1);
            double mean = Config.c.transferTimes[stationIndex].averageExit[idxT];
            double sd = Config.c.transferTimes[stationIndex].standardDeviationExit[idxT];

            double log_mean = Math.Max(0.000001, Math.Log(mean));
            double log_sd = Math.Max(0.000001, Math.Log(sd));

            return (int)LogNormal.Sample(log_mean, log_sd);
        }
    }
}
