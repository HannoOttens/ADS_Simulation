using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ADS_Simulation.Events;
using ADS_Simulation.NS_State;

namespace ADS_Simulation.Statistics
{
    class TotalPassengerStatistic : Statistic
    {
        int[] totalPassengers;

        public TotalPassengerStatistic(int startTime, int endTime, int stationCount) : base(startTime, endTime)
        {
            totalPassengers = new int[stationCount];
        }

        public override void measure(State state, Event currentEvent)
        {
            if(currentEvent is PassengerArrival pa)
                totalPassengers[pa.stationIdx] += 1;
        }

        public override void Print(State state)
        {
            Console.WriteLine("=========Total Passengers=========");
            for(int i = 0; i < state.stations.Count; i++)
            {
                string name = state.stations[i].name;
                Direction direction = state.stations[i].direction;
                Console.WriteLine($"{direction} - {name}: {totalPassengers[i]}");
            }
            Console.WriteLine($"Total: {totalPassengers.Sum()}");
        }

        public override string[] GetHeaders()
        {
            return new string[] { "total_passengers" };
        }

        public override string[] GetValues(State state)
        {
            return new string[] { totalPassengers.Sum().ToString() };
        }
    }
}
