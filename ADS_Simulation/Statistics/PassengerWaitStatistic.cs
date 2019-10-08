using System;
using System.Linq;
using ADS_Simulation.NS_State;

namespace ADS_Simulation.Statistics
{
    class PassengerWaitStatistic : Statistic
    {
        int lastEventTime;
        int stationCount;

        (int totalTime, int passengers)[] currentTotalWaitingTime;
        (int totalTime, int passengers)[] totalWaitingTime;
        int[] longestWaitTime;

        public PassengerWaitStatistic(int stationCount, int startTime)
        {
            this.stationCount = stationCount;
            lastEventTime = startTime;

            currentTotalWaitingTime = new (int, int)[stationCount];
            totalWaitingTime = new (int, int)[stationCount];
            longestWaitTime = new int[stationCount];
        }

        public override void measure(State state)
        {
            Station station;
            for (int stationIdx = 0; stationIdx < stationCount; stationIdx++)
            {
                station = state.stations[stationIdx];
                if (station.HasPassengers())
                {
                    var current = (station.waitingPassengers.Aggregate((sum, queueTime) => sum += state.time - queueTime), station.waitingPassengers.Count);
                    currentTotalWaitingTime[stationIdx] = current;
                    int waitTime = state.time - station.waitingPassengers.Peek();
                    if (waitTime > longestWaitTime[stationIdx])
                        longestWaitTime[stationIdx] = waitTime;
                }
                else
                {
                    totalWaitingTime[stationIdx].passengers += currentTotalWaitingTime[stationIdx].passengers;
                    totalWaitingTime[stationIdx].totalTime += currentTotalWaitingTime[stationIdx].totalTime;
                    currentTotalWaitingTime[stationIdx] = (0, 0);
                }
            }
            lastEventTime = state.time;
        }

        public override void Print(State state)
        {
            throw new NotImplementedException();
        }
    }
}
