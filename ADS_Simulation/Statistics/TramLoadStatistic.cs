using System;
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

        public TramLoadStatistic(int tramCount)
        {
            this.tramCount = tramCount;
            totalPassengerCount = new int[tramCount];
            lowestPassengerCount = Enumerable.Repeat(int.MaxValue, tramCount).ToArray();
            highestPassengerCount = Enumerable.Repeat(int.MinValue, tramCount).ToArray();
        }

        public override void measure(State state)
        {
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

        public float TotalAverageTramLoad(int endtime, int tramCount)
        {
            return totalPassengerCount.Sum() / (endtime * tramCount);
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
