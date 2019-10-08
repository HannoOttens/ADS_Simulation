using System;
using System.Linq;
using ADS_Simulation.NS_State;

namespace ADS_Simulation.Statistics
{
    class TramLoadStatistic : Statistic
    {
        int tramCount;
        int counter;

        int[] totalPassengerCount;
        int[] lowestPassengerCount;
        bool[] isDriving;

        public TramLoadStatistic(int tramCount)
        {
            this.tramCount = tramCount;
            counter = 0;

            totalPassengerCount = new int[tramCount];
            lowestPassengerCount = new int[tramCount];
            isDriving = new bool[tramCount];
        }

        public override void measure(State state)
        {
            for(int tramIndex = 0; tramIndex < tramCount; tramIndex++)
            {
                Tram tram = state.trams[tramIndex];
                if (!tram.IsDriving || isDriving[tramIndex])
                {
                    isDriving[tramIndex] = tram.IsDriving;
                    continue;
                }

                isDriving[tramIndex] = true;
                totalPassengerCount[tramIndex] += tram.passengerCount;
                if (totalPassengerCount[tramIndex] < lowestPassengerCount[tramIndex])
                    lowestPassengerCount[tramIndex] = lowestPassengerCount[tramIndex];
                counter++;
            }
        }

        public float TotalAverageTramLoad()
        {
            return (float)totalPassengerCount.Sum() / (counter * Configuration.Config.c.tramCapacity);
        }

        public override void Print(State state)
        {
            Console.WriteLine($"Average passenger load: {TotalAverageTramLoad()}");
            
        }
    }
}
