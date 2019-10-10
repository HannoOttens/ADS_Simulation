using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public PassengerWaitStatistic(int startTime, int endTime, int stationCount) : base(startTime, endTime)
        {
            this.stationCount = stationCount;
            this.lastEventTime = startTime;

            totalWaitingTime = new int[stationCount];
            longestWaitTime = new int[stationCount];
            previousQueueLength = new int[stationCount];

            totalPassengers = new int[stationCount];
        }

        public override void measure(State state)
        {
            int timeDelta = state.time - lastEventTime;
            if (timeDelta == 0) return; // No time has passed - not interesting

            for (int stationIdx = 0; stationIdx < stationCount; stationIdx++)
            {
                var station = state.stations[stationIdx];

                int currentQueue = station.waitingPassengers.Count;
                int queueDelta = currentQueue - previousQueueLength[stationIdx];
                previousQueueLength[stationIdx] = currentQueue;

                // Passengers got in tram, calculate waiting time of remaining passengers
                if(queueDelta <= 0)
                {
                    totalWaitingTime[stationIdx] += timeDelta * currentQueue;
                }
                // New passengers have arrived, calculate their waiting time
                else
                {
                    int waitingTime = WaitingTimeNewPassengers(station.waitingPassengers.TakeLast(queueDelta), state.time)
                                    + timeDelta * (currentQueue - queueDelta);
                    Debug.Assert(waitingTime >= 0, "Negative waiting time");
                    totalWaitingTime[stationIdx] += waitingTime;  
                    totalPassengers[stationIdx] += queueDelta;
                }

                // Update longest waiting time, person first in queue has the longest waiting time
                if (station.HasPassengers())
                {
                    int waitTime = state.time - station.waitingPassengers.Peek();
                    if (waitTime > longestWaitTime[stationIdx])
                        longestWaitTime[stationIdx] = waitTime;
                }
            }
            lastEventTime = state.time;
        }

        private int WaitingTimeNewPassengers(IEnumerable<int> newPassengers, int time)
        {
            int sum = 0;
            foreach (int arrival in newPassengers)
                sum += time - arrival;
            return sum;
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
