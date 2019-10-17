using System;
using System.Collections.Generic;
using System.Text;
using ADS_Simulation.Configuration;
using ADS_Simulation.Events;
using ADS_Simulation.NS_State;

namespace ADS_Simulation.Statistics
{
    class RoundTripStatistics : Statistic
    {
        Dictionary<int, int> lastDepartureTime;
        int maxRtt;
        float averageRtt;
        int rttTramCount;

        public RoundTripStatistics(int startTime, int endTime, List<Tram> trams) : base(startTime, endTime)
        {
            lastDepartureTime = new Dictionary<int, int>();
            foreach (var tram in trams)
                lastDepartureTime.Add(tram.id, -1);
        }

        public override void measure(State state, Event currentEvent)
        {
            if (currentEvent is DepartureStartstation ds
                && ds.station.name == Config.c.startStation)
            {
                int tramId = ds.tram.id;

                if (lastDepartureTime[tramId] >= 0)
                {
                    int rtt = state.time - lastDepartureTime[tramId];
                    maxRtt = Math.Max(maxRtt, rtt);

                    float t = rttTramCount * averageRtt + rtt;
                    rttTramCount++;
                    averageRtt = t / rttTramCount;
                }

                lastDepartureTime[tramId] = state.time;
            }
        }

        public override void Print(State state)
        {
            Console.WriteLine("=========Round trip times=============");
            Console.WriteLine($"Average RTT: {(int)averageRtt / 60} minutes");
            Console.WriteLine($"Max RTT: {(int)maxRtt / 60} minutes");
        }

        public override string[] GetHeaders()
        {
            return new string[] { "average_rtt", "max_rtt" };
        }

        public override string[] GetValues(State state)
        {
            return new string[] {
                averageRtt.ToString(),
                maxRtt.ToString()
            };
        }
    }
}
