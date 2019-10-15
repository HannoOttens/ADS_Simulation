using System;
using System.Collections.Generic;
using ADS_Simulation.Configuration;
using ADS_Simulation.Events;
using ADS_Simulation.NS_State;

namespace ADS_Simulation.Statistics
{
    class TramDelayStatistic : Statistic
    {
        Dictionary<(Endstation, Platform), int> nextDeparture;
        int[] totalDelay;
        Endstation startStation;
        int totalDelayedTrams = 0;
        int totalOnTimeTrams = 0;

        public TramDelayStatistic(int startTime, int endTime, Endstation startStation, Endstation endStation) : base(startTime, endTime)
        {
            this.startStation = startStation;
            nextDeparture = new Dictionary<(Endstation, Platform), int>() {
                { (startStation, Platform.A), 0 },
                { (startStation, Platform.B), 0 },
                { (endStation, Platform.A), 0 },
                { (endStation, Platform.B), 0 },
            };
            totalDelay = new int[2];
        }

        public override void measure(State state, Event currentEvent)
        {
            // Check if tram has arrived on platform
            if (currentEvent is ExpectedDepartureStartstation eds)
                nextDeparture[(eds.station, eds.platform)] = eds.timeTableTime;
            else if (currentEvent is DepartureStartstation ds)
            {
                int delay = state.time - nextDeparture[(ds.station, ds.platform)];
                if (delay >= Config.c.maximumDelayBeforeDelayed)
                {
                    totalDelay[startStation == ds.station ? 0 : 1] += delay;
                    totalDelayedTrams++;
                }
                else
                    totalOnTimeTrams++;
            }
        }

        public override void Print(State state)
        {
            Console.WriteLine($"Total delay P+R: {totalDelay[0]}; UC: {totalDelay[1]}");
            Console.WriteLine($"Delayed %: {100 * (float)totalDelayedTrams / (totalDelayedTrams + totalOnTimeTrams)}");
        }
    }
}
