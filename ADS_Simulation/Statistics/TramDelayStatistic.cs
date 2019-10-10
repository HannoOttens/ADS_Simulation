using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ADS_Simulation.NS_State;

namespace ADS_Simulation.Statistics
{
    class TramDelayStatistic : Statistic
    {
        int[] endstations;

        (int a, int b)[] nextDeparture;
        int[] totalDelay;
        (bool a, bool b)[] platformFree;

        public TramDelayStatistic(int startTime, int endTime, int stationCount) : base(startTime, endTime)
        {
            endstations = new int[] { 0, 8 };

            nextDeparture = new (int, int)[2];
            totalDelay = new int[2];
            platformFree = new (bool, bool)[] { (true, true), (true, true) };
        }

        public override void measure(State state)
        {
            for (int i = 0; i < endstations.Length; i++)
            {
                if (state.stations[endstations[i]] is Endstation endstation)
                {
                    // Check if tram has arrived on platform
                    if (!endstation.IsFree(Platform.A) && platformFree[i].a)
                    {
                        platformFree[i].a = false;
                        nextDeparture[i].a = endstation.PeekNextDeparture();
                    }
                    else if (!endstation.IsFree(Platform.B) && platformFree[i].b)
                    {
                        platformFree[i].b = false;
                        nextDeparture[i].b = endstation.PeekNextDeparture();
                    }
                    else if (endstation.IsFree(Platform.A) && !platformFree[i].a)
                    {
                        platformFree[i].a = true;
                        if (state.time - nextDeparture[i].a is int delay && delay >= 60)
                            totalDelay[i] += delay;
                    }
                    else if (endstation.IsFree(Platform.B) && !platformFree[i].b)
                    {
                        platformFree[i].b = true;
                        if (state.time - nextDeparture[i].b is int delay && delay >= 60)
                            totalDelay[i] += delay;
                    }
                }
            }
        }

        public override void Print(State state)
        {
            Console.WriteLine($"Total delay P+R: {totalDelay[0]}; UC: {totalDelay[1]}");
        }
    }
}
