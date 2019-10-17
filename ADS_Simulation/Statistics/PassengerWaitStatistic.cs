using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ADS_Simulation.Events;
using ADS_Simulation.NS_State;

namespace ADS_Simulation.Statistics
{
    class PassengerWaitStatistic : Statistic
    {
        int lastEventTime;
        int stationCount;

        int[] totalWaitingTime;
        int[] longestWaitTime;
        int[] previousQueueLength;

        int[] totalPassengers;
        int[] totalPassengersLeftWaiting;
        int[] mostPassengersLeftWaiting;
        int longestQueue;

        public PassengerWaitStatistic(int startTime, int endTime, int stationCount) : base(startTime, endTime)
        {
            this.stationCount = stationCount;
            this.lastEventTime = startTime;

            totalWaitingTime = new int[stationCount];
            longestWaitTime = new int[stationCount];
            previousQueueLength = new int[stationCount];
            totalPassengers = new int[stationCount];

            totalPassengersLeftWaiting = new int[stationCount];
            mostPassengersLeftWaiting = new int[stationCount];
            longestQueue = 0;
        }

        public override void measure(State state, Event currentEvent)
        {
            int timeDelta = state.time - lastEventTime;

            for (int stationIdx = 0; stationIdx < stationCount; stationIdx++)
            {
                var station = state.stations[stationIdx];

                int currentQueue = station.waitingPassengers.Count;
                int queueDelta = currentQueue - previousQueueLength[stationIdx];
                previousQueueLength[stationIdx] = currentQueue;

                if (longestQueue < currentQueue)
                    longestQueue = currentQueue;

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

                // Check if passengers are left waiting for the next tram
                if (queueDelta < 0 && currentQueue != 0)
                {
                    totalPassengersLeftWaiting[stationIdx] += currentQueue;
                    if (mostPassengersLeftWaiting[stationIdx] < currentQueue)
                        mostPassengersLeftWaiting[stationIdx] = currentQueue;
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

        public int[] AverageWaitingTimePerStation()
        {
            return totalWaitingTime.Zip(totalPassengers).Select(tuple => tuple.Second == 0 ? 0 : tuple.First / tuple.Second).ToArray();
        } 

        public int AverageWaitingTime()
        {
            return totalWaitingTime.Sum() / totalPassengers.Sum();
        }

        public float AverageLeftWaiting()
        {
            return (float)totalPassengersLeftWaiting.Sum() / totalPassengers.Sum() * 100;
        }

        public override void Print(State state)
        {
            var average = AverageWaitingTime();
            var maxWaitTime = longestWaitTime.Max();
            var mostLeftWaiting = mostPassengersLeftWaiting.Max();
            var averageLeftWaiting = AverageLeftWaiting();

            Console.WriteLine($"Average waiting time: {average}");
            Console.WriteLine($"Longest waiting time: {maxWaitTime}");
            Console.WriteLine($"Longest queue length: {longestQueue}");

            Console.WriteLine($"Most passengers left waiting: {mostLeftWaiting}");
            Console.WriteLine($"Percentage of passengers left waiting: {averageLeftWaiting}");
        }

        public override string[] GetHeaders()
        {
            return new string[] { "average_waiting_time", "longest_waiting_time", "longest_queue", "most_left_waiting", "percentage_left_waiting" };
        }

        public override string[] GetValues(State state)
        {
            var average = AverageWaitingTime();
            var maxWaitTime = longestWaitTime.Max();
            var mostLeftWaiting = mostPassengersLeftWaiting.Max();
            var averageLeftWaiting = AverageLeftWaiting();

            return new string[] {
                average.ToString(),
                maxWaitTime.ToString(),
                longestQueue.ToString(),
                mostLeftWaiting.ToString(),
                averageLeftWaiting.ToString()
            };
        }
    }
}
