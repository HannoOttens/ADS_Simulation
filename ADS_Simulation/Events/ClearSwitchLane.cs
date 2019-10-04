using System;
using System.Collections.Generic;
using System.Text;
using ADS_Simulation.NS_State;
using Priority_Queue;

namespace ADS_Simulation.Events
{
    class ClearSwitchLane : Event
    {
        Endstation station;
        SwitchLane lane;

        public ClearSwitchLane(Endstation station, SwitchLane lane)
        {
            this.station = station;
            this.lane = lane;
        }

        public override void Execute(State state, FastPriorityQueue<Event> eventQueue)
        {
            station.Switch.FreeSwitch(lane);

            // Check if tram is in queue at switch, give priority to departing trams
            if (station.departingTrams.TryDequeue(out Tram? departingTram))
            {
                Event e = new ExpectedDepartureStartstation();
                eventQueue.Enqueue(e, 0);
            }

            if (station.incomingTrams.TryDequeue(out Tram? arrivingTram))
            {
                Event e = new ExpectedArrivalEndstation(arrivingTram, station);
                eventQueue.Enqueue(e, 0);
            }
        }
    }
}
