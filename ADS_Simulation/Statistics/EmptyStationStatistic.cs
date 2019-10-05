using System;
using System.Collections.Generic;
using System.Text;
using ADS_Simulation.NS_State;

namespace ADS_Simulation.Statistics
{
    class EmptyStationStatistic : Statistic
    {
        int lastEventTime;

        int totalEmptyTime;
        int[] longestEmptyStationTime;

        public EmptyStationStatistic(int startTime, int stationCount)
        {
            totalEmptyTime = 0;
            lastEventTime = startTime;
            longestEmptyStationTime = new int[stationCount];
        }

        public override void measure(State state)
        {
            for (int stationIndex = 0; stationIndex < state.stations.Count; i++)
            {
                Station station = state.stations[stationIndex];

                bool hasOccupant = false;
                hasOccupant |= station.IsFree();
                if (station is Endstation endstation)
                    hasOccupant |= endstation.IsFree(Platform.B);

                if (!hasOccupant)
                {
                    int emptyTime = state.time - lastEventTime;
                    totalEmptyTime += emptyTime;
                    longestEmptyStationTime[stationIndex] += emptyTime;
                }
                else
                    longestEmptyStationTime[stationIndex] = 0;
            }
        }
    }
}
