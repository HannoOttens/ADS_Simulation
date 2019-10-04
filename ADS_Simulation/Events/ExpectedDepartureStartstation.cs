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

        public ExpectedDepartureStartstation(Tram tram, Endstation station, Platform platform)
        {
            this.tram = tram;
            this.station = station;
            this.platform = platform;
        }

        public override void Execute(State state, FastPriorityQueue<Event> eventQueue)
        {
            throw new NotImplementedException();
        }
    }
}
