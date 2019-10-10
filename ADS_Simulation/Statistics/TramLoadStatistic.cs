﻿using System;
using System.Diagnostics;
using System.Linq;
using ADS_Simulation.NS_State;

namespace ADS_Simulation.Statistics
{
    class TramLoadStatistic : Statistic
    {
        int tramCount;
        int lastEventTime;

        int[] totalPassengerCount;
        int[] lowestPassengerCount;
        int[] highestPassengerCount;

        public TramLoadStatistic(int startTime, int endTime, int tramCount) : base(startTime, endTime)
        {
            this.tramCount = tramCount;
            totalPassengerCount = new int[tramCount];
            lowestPassengerCount = Enumerable.Repeat(int.MaxValue, tramCount).ToArray();
            highestPassengerCount = Enumerable.Repeat(int.MinValue, tramCount).ToArray();
        }

        public override void measure(State state)
        {
            Debug.Assert(state.time >= startTime && state.time <= endTime, "Measured statistic outside of time bounds");

            int timeDelta = state.time - lastEventTime;
            if (timeDelta == 0) return; // No time has passed - not interesting

            for(int tramIndex = 0; tramIndex < tramCount; tramIndex++)
            {
                Tram tram = state.trams[tramIndex];
                
                totalPassengerCount[tramIndex] += tram.passengerCount * timeDelta;
                
                if (tram.passengerCount < lowestPassengerCount[tramIndex])
                    lowestPassengerCount[tramIndex] = tram.passengerCount;
                if (tram.passengerCount > highestPassengerCount[tramIndex])
                    highestPassengerCount[tramIndex] = tram.passengerCount;
            }

            lastEventTime = state.time;
        }

        public float TotalAverageTramLoad(int stateEndtime, int tramCount)
        {
            int runtime = Math.Min(stateEndtime, endTime) - startTime;
            return totalPassengerCount.Sum() / (runtime * tramCount);
        }

        public int LowestTramLoad()
        {
            return lowestPassengerCount.Min();
        }

        public int HigestTramLoad()
        {
            return highestPassengerCount.Max();
        }

        public override void Print(State state)
        {
            Console.WriteLine($"Average tram load: {TotalAverageTramLoad(state.time, state.trams.Count)}");
            Console.WriteLine($"Lowest tram load: {LowestTramLoad()}");
            Console.WriteLine($"Highest tram load: {HigestTramLoad()}");
        }
    }
}
