using System;
using System.Collections.Generic;
using System.Text;
using ADS_Simulation.NS_State;
using Priority_Queue;

namespace ADS_Simulation.Events
{
    class ExpectedTramArrival : Event
    {
        // Public wegens GUI
        public Tram tram;
        public int stationIndex;

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
            {
                // Occupy station with the tram
                station.Occupy(tram);

                eventQueue.Enqueue(new TramArrival(tram, stationIndex), state.time);
        }
            }
    }
}
