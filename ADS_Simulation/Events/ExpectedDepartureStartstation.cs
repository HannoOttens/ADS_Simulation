using System.Collections.Generic;
using ADS_Simulation.NS_State;
using Priority_Queue;

namespace ADS_Simulation.Events
{
    class ExpectedDepartureStartstation : Event
    {
        public readonly Tram tram;
        public readonly Endstation station;
        public readonly Platform platform;
        public readonly int timeTableTime;
        public List<int> entrances = new List<int>();

        public ExpectedDepartureStartstation(Tram tram, Endstation station, Platform platform, int timeTableTime)
        {
            this.tram = tram;
            this.station = station;
            this.platform = platform;
            this.timeTableTime = timeTableTime;
        }

        public override void Execute(State state, FastPriorityQueue<Event> eventQueue)
        {
            System.Diagnostics.Debug.WriteLine($"ExpectedDepartureStartstation: tram {tram.id}, station: {station.name}, {platform}, Timetable: {timeTableTime}, time: {state.time}");

            // Close doors if it is time to leave
            if (state.time >= timeTableTime)
            {
                // Mark tram as ready for departure
                tram.ReadyForDeparture();

                // Start leaving directly when possible
                SwitchLane lane = Switch.ExitLaneFor(platform);
                bool isFirst = station.first == platform;
                if (isFirst && station._switch.SwitchLaneFree(lane))
                {
                    station._switch.UseSwitchLane(lane);
                    eventQueue.Enqueue(new DepartureStartstation(tram, station, platform), state.time);
                }
            }
            // Otherwise let extra passengers enter
            else
            {
                (int pInExtra, List<int> e) = station.BoardPassengers(tram);
                entrances = e;
                int extraTime = Sampling.passengerExchangeTime(0, pInExtra);
                eventQueue.Enqueue(new ExpectedDepartureStartstation(tram, station, platform, timeTableTime), state.time + extraTime);
            }
        }
    }
}
