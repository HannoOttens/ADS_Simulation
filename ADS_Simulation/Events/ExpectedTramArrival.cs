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
            var station = state.stations[stationIndex];

            System.Diagnostics.Debug.WriteLine($"ExpectedTramArrival: tram {tram.id}, station: {station.name}, dir: {station.direction}");

            if (station.IsFree())
                eventQueue.Enqueue(new TramArrival(tram, stationIndex), state.time);
        }
    }
}
