using System;
using System.Collections.Generic;
using System.Text;
using ADS_Simulation.NS_State;
using Priority_Queue;

namespace ADS_Simulation.Events
{
    class ExpectedDepartureStartstation : Event
    {
        private Tram tram;
        private Endstation station;
        private Platform platform;
        private int timeTableTime;

        public ExpectedDepartureStartstation(Tram tram, Endstation station, Platform platform, int timeTableTime)
        {
            this.tram = tram;
            this.station = station;
            this.platform = platform;
            this.timeTableTime = timeTableTime;
        }

        public override void Execute(State state, FastPriorityQueue<Event> eventQueue)
        {
            // Close doors if it is time to leave
            if (state.time >= timeTableTime)
            {
                // Mark tram as ready for departure
                tram.ReadyForDeparture();

                // Start leaving directly when possible
                SwitchLane lane = Switch.ExitLaneFor(Platform.A);
                if (station.Switch.SwitchLaneFree(lane))
                    eventQueue.Enqueue(new DepartureStartstation(tram, station, platform), state.time);
            }
            // Otherwise let extra passengers enter
            else
            {
                int pInExtra = station.BoardPassengers(tram);
                int extraTime = Sampling.passengerExchangeTime(0, pInExtra);
                eventQueue.Enqueue(new ExpectedDepartureStartstation(tram, station, platform, timeTableTime), state.time + extraTime);
            }
        }
    }
}
