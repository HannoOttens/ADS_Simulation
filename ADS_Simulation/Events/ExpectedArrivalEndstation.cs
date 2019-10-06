using System;
using System.Collections.Generic;
using System.Text;
using ADS_Simulation.NS_State;
using Priority_Queue;

namespace ADS_Simulation.Events
{
    class ExpectedArrivalEndstation : Event
    {
        public Tram tram;
        public Endstation station;

        public ExpectedArrivalEndstation(Tram tram, Endstation station)
        {
            this.tram = tram;
            this.station = station;
        }

        public override void Execute(State state, FastPriorityQueue<Event> eventQueue)
        {
            // Check if there is a free platform available and if switch lane is free
            Platform bestPlatform = station.BestFreePlatform();
            if(bestPlatform != Platform.None)
            {
                station.Occupy(tram, bestPlatform);
                station.Switch.UseSwitchLane(Switch.ArrivalLaneFor(bestPlatform));
                eventQueue.Enqueue(new ArrivalEndstation(tram, station, bestPlatform), state.time + Sampling.switchClearanceTime());
            }
            else
                station.incomingTrams.Enqueue(tram);
        }
    }
}
