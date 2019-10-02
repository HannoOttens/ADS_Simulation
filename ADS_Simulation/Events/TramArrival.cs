using System;
using System.Collections.Generic;
using System.Text;
using ADS_Simulation.NS_State;
using Priority_Queue;

namespace ADS_Simulation.Events
{
    class TramArrival : Event
    {
        Tram tram;
        int stationIndex;

        public TramArrival(Tram tram, int stationIndex)
        {
            this.tram = tram;
            this.stationIndex = stationIndex;
        }

        public override void Execute(State state, StablePriorityQueue<Event> eventQueue)
        {
            Console.WriteLine($"{tram.id} arrived at {state.stations[stationIndex].name}");

            // Try occupy the station (otherwise queue is joined)
            bool occupied = state.stations[stationIndex].TryOccupy(tram);
            if(occupied)
                eventQueue.Enqueue(new TramDeparture(tram, stationIndex), state.time + Sampling.passengerExchangeTime(0,0));
        }
    }
}
