using System;
using System.Linq;
using ADS_Simulation.NS_State;

namespace ADS_Simulation.Statistics
{
    class PassengerWaitStatistic : Statistic
    {
        int lastEventTime;
        int stationCount;

        int[] totalWaitingTime;
        int[] longestWaitTime;
        int[] totalPassengers;
        int[] previousQueueLength;

        public PassengerWaitStatistic(int stationCount)
        {
            this.stationCount = stationCount;
            lastEventTime = Configuration.Config.c.startTime;

            totalWaitingTime = new int[stationCount];
            longestWaitTime = new int[stationCount];
            previousQueueLength = new int[stationCount];

            totalPassengers = new int[stationCount];
        }

        public override void measure(State state)
        {
            for (int stationIdx = 0; stationIdx < stationCount; stationIdx++)
            {
                var station = state.stations[stationIdx];

                int currentQueue = station.waitingPassengers.Count;
                int queueDelta = currentQueue - previousQueueLength[stationIdx];
                previousQueueLength[stationIdx] = currentQueue;

                // Passengers got in tram, calculate waiting time of remaining passengers
                if(queueDelta < 0)
                {
                    totalWaitingTime[stationIdx] += (state.time - lastEventTime) * currentQueue;
                }
                // New passengers have arrived, calculate their waiting time
                else
                {
                    totalWaitingTime[stationIdx] 
                        += station.waitingPassengers.TakeLast(queueDelta).Aggregate(0, (sum, queueTime) => sum + state.time - queueTime)
                        + (state.time - lastEventTime) * (currentQueue - queueDelta);
                    totalPassengers[stationIdx] += queueDelta;
                }

                // Update longest waiting time, person first in queue has the longest waiting time
                int waitTime = state.time - station.waitingPassengers.Peek();
                if (waitTime > longestWaitTime[stationIdx])
                    longestWaitTime[stationIdx] = waitTime;
            }
            lastEventTime = state.time;
        }

        public int[] AverageWaitingTime()
        {
            return totalWaitingTime.Zip(totalPassengers).Select(tuple => tuple.First / tuple.Second).ToArray();
        } 

        public override void Print(State state)
        {
            Console.WriteLine("Average waiting time per station (station, average, max):");

            var average = AverageWaitingTime();
            for (int i = 0; i < stationCount; i++)
            {
                Station station = state.stations[i];
                Console.WriteLine($"{station.name} - {station.direction} - {average[i]} - {longestWaitTime[i]}");
            }
        }
    }
}
