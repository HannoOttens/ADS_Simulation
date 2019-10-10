using System;
using System.Collections.Generic;
using System.Text;
using ADS_Simulation.NS_State;

namespace ADS_Simulation.Statistics
{
    class StatisticsManager
    {
        List<Statistic> statistics;
        (int, int)[] timeRanges;

        public StatisticsManager(State startState, (int, int)[] timeRanges)
        {
            this.timeRanges = timeRanges;
            
            // Create statistics for every range
            statistics = new List<Statistic>();
            foreach ((int startTime, int endTime) in timeRanges)
                statistics.AddRange(new List<Statistic>(){
                    new PassengerWaitStatistic(startTime, endTime, startState.stations.Count),
                    new TramLoadStatistic(startTime, endTime, startState.trams.Count),
                    new EmptyStationStatistic(startTime, endTime, startState.stations.Count)
                });
        }

        public void measureStatistics(State state)
        {
            foreach (Statistic statistic in statistics)
                if (state.time >= statistic.startTime 
                    && state.time <= statistic.endTime)
                {
                    statistic.measure(state);
                }
        }

        public void printStatistics(State state)
        {
            foreach((int startTime, int endTime) in timeRanges)
            {
                Console.WriteLine("-----------------------------------------");
                Console.WriteLine($"\t{stateTimeToString(startTime)} - {stateTimeToString(endTime)}");
                Console.WriteLine("-----------------------------------------");
                foreach (Statistic statistic in statistics)
                    if (statistic.startTime == startTime && statistic.endTime == endTime)
                        statistic.Print(state);
            }
        }

        private string stateTimeToString(int stateTime)
        {
            stateTime /= 60;
            return $"{(stateTime / 60).ToString("00")}:{(stateTime % 60).ToString("00")}";
        }
    }
}
