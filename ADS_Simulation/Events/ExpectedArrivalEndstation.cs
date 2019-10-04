using System;
using System.Collections.Generic;
using System.Text;
using ADS_Simulation.NS_State;
using Priority_Queue;

namespace ADS_Simulation.Events
{
    class ExpectedArrivalEndstation : Event
    {
        Tram tram;
        Endstation station;

        public ExpectedArrivalEndstation(Tram tram, Endstation station)
        {
            this.tram = tram;
            this.station = station;
        }

        public override void Execute(State state, FastPriorityQueue<Event> eventQueue)
        {
            // Check if there is a free platform available and if switch lane is free
            // Cross is used for platform 1
            if (station.IsFree(1) && station.Switch.UseSwitchIfFree(SwitchLane.Cross))
            {
                station.Occupy(tram);
                eventQueue.Enqueue(new ArrivalEndstation(tram, station, 1), Configuration.Config.c.switchClearanceTime);
            }
            // Arrival lane is used for platform 2
            else if (station.IsFree(2) && station.Switch.UseSwitchIfFree(SwitchLane.ArrivalLane))
            {
                station.occupant2 = tram;
                eventQueue.Enqueue(new ArrivalEndstation(tram, station, 2), Configuration.Config.c.switchClearanceTime);
            }
            else
                station.incomingTrams.Enqueue(tram);
        }
    }
}
