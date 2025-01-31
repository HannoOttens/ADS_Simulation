﻿using System;
using System.Linq;
using ADS_Simulation.Configuration;
using ADS_Simulation.Events;
using ADS_Simulation.NS_State;

namespace ADS_Simulation.Statistics
{
    class EmptyStationStatistic : Statistic
    {
        int lastEventTime;
        readonly int stationCount;

        int totalEmptyTime;
        readonly int[] currentEmptyStationTime;
        readonly int[] longestEmptyStationTime;

        public EmptyStationStatistic(int startTime, int endTime, int stationCount) : base(startTime, endTime)
        {
            totalEmptyTime = 0;
            lastEventTime = Config.c.startTime;
            this.stationCount = stationCount;
            currentEmptyStationTime = new int[stationCount];
            longestEmptyStationTime = new int[stationCount];
        }

        public override void measure(State state, Event currentEvent)
        {
            for (int stationIndex = 0; stationIndex < state.stations.Count; stationIndex++)
            {
                Station station = state.stations[stationIndex];

                bool hasOccupant = false;
                hasOccupant |= !station.IsFree();
                if (station is Endstation endstation)
                    hasOccupant |= !endstation.IsFree(Platform.B);

                if (!hasOccupant)
                {
                    int emptyTime = state.time - lastEventTime;
                    totalEmptyTime += emptyTime;
                    currentEmptyStationTime[stationIndex] += emptyTime;
                }
                else
                {
                    if (longestEmptyStationTime[stationIndex] < currentEmptyStationTime[stationIndex])
                        longestEmptyStationTime[stationIndex] = currentEmptyStationTime[stationIndex];
                    currentEmptyStationTime[stationIndex] = 0;
                }
            }

            // Move last event time forward
            lastEventTime = state.time;
        }

        public (int stationIndex, int longestEmptyTime) LongestEmptyTime()
        {
            int longest = longestEmptyStationTime.Max();
            int index = longestEmptyStationTime.ToList().IndexOf(longest);
            return (index, longest);
        }

        public int AverageEmptyTime()
        {
            return totalEmptyTime / stationCount;
        }

        public override void Print(State state)
        {
            Console.WriteLine($"Average station empty time: {AverageEmptyTime()}");

            (int stationIndex, int longestTime) = LongestEmptyTime();
            Station station = state.stations[stationIndex];
            Console.WriteLine($"Longest empty: {station.name} for {longestTime}s");
        }

        public override string[] GetHeaders()
        {
            return new string[] { "average_empty_time", "longest_empty_time", "longest_empty_time_station" };
        }

        public override string[] GetValues(State state)
        {
            var averageEmpty = AverageEmptyTime();
            (var stationIndex, var emptyTime) = LongestEmptyTime();
            var station = state.stations[stationIndex].name;

            return new string[] {
                averageEmpty.ToString(),
                emptyTime.ToString(),
                station
            };
        }
    }
}
