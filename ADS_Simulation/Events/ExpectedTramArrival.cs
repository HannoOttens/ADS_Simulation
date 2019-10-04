using System;
using System.Collections.Generic;
using System.Text;
using ADS_Simulation.NS_State;
using Priority_Queue;

namespace ADS_Simulation.Events
{
    class ExpectedTramArrival : Event
    {
        private Tram tram;
        private int stationIndex;

        public ExpectedTramArrival(Tram tram, int stationIndex)
        {
            this.tram = tram;
            this.stationIndex = stationIndex;
        }

        public override void Execute(State state, FastPriorityQueue<Event> eventQueue)
        {
            if (state.stations[stationIndex].IsFree())
                eventQueue.Enqueue(new TramArrival(tram, stationIndex), state.time);
        }
    }
}
