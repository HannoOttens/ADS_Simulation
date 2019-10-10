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

        public PassengerWaitStatistic(int stationCount)
        {
            this.stationCount = stationCount;
            lastEventTime = Configuration.Config.c.startTime;

            currentTotalWaitingTime = new (int, int)[stationCount];
            totalWaitingTime = new (int, int)[stationCount];
            longestWaitTime = new int[stationCount];
        }

        public override void measure(State state)
        {
            for (int stationIdx = 0; stationIdx < stationCount; stationIdx++)
            {
                var station = state.stations[stationIdx];

                // Check if tram has just departed from station
                if (station.HasPassengers())
                {
                    var current = currentTotalWaitingTime[stationIdx];

                    // Calculate total waiting time of passengers arrived in de mean time
                    int newPassengers = station.waitingPassengers.Count - current.passengers;
                    current.totalTime += station.waitingPassengers.TakeLast(newPassengers).Aggregate(0, (sum, queueTime) => sum + state.time - queueTime)
                        + (state.time - lastEventTime) * current.passengers;
                    current.passengers += newPassengers;
                    currentTotalWaitingTime[stationIdx] = current;

                    // Check if longest wait time has changed
                    int waitTime = state.time - station.waitingPassengers.Peek();
                    if (waitTime > longestWaitTime[stationIdx])
                        longestWaitTime[stationIdx] = waitTime;
                }
                else
                {
                    // Add intermediate waiting times to total waiting time on station
                    totalWaitingTime[stationIdx].passengers += currentTotalWaitingTime[stationIdx].passengers;
                    totalWaitingTime[stationIdx].totalTime += currentTotalWaitingTime[stationIdx].totalTime;
                    currentTotalWaitingTime[stationIdx] = (0, 0);
                }
            }
            lastEventTime = state.time;
        }

        public int[] AverageWaitingTime()
        {
            return totalWaitingTime.Select((record) => record.passengers == 0 ? 0 : record.totalTime / record.passengers).ToArray();
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
