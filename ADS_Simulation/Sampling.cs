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
        public static int timeUntilNextPassenger(int time, Station station)
        {
            foreach(string[] values in Config.indata)
                    if (values[0] == station.name &&
                        (values[1] == "0" && station.direction == Direction.A ||
                         values[1] == "1" && station.direction == Direction.B) &&
                        int.Parse(values[2]) * 60 <= time && time <= (int.Parse(values[2]) + 15) * 60)
                        return Poisson.Sample(Math.Max(0.1, double.Parse(values[3])));
            return 10; // TODO: in and out files don't have all data yet
        }

        /// <summary>
        /// The time a station needs to be clear for before the next tram can arrive
        /// </summary>
        /// <returns></returns>
        public static int tramSafetyDistance()
        {
            return 40;
        }

        internal static int unboardingPassengerCount(int time, Station station)
        {
            foreach (string[] values in Config.outdata)
                if (values[0] == station.name &&
                    (values[1] == "0" && station.direction == Direction.A ||
                     values[1] == "1" && station.direction == Direction.B) &&
                    int.Parse(values[2]) * 60 <= time && time < (int.Parse(values[2]) + 15) * 60)
                {
                    var mean = Math.Log(double.Parse(values[3]));
                    if (mean <= 0) mean = 0.00001;
                    var sd = Math.Log(double.Parse(values[4]));
                    if (sd <= 0) sd = 0.00001;
                    return (int)LogNormal.Sample(mean, sd);
                }
            return 10;// TODO: in and out files don't have all data yet
        }
    }
}
