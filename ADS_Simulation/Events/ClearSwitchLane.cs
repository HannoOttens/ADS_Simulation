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

            // FIXME: Find the first occupant ready for departure
            if (false)
            {
                //Event e = new DepartureStartstation(departingTram, station);
                //eventQueue.Enqueue(e, state.time);
            }
            else if (station.incomingTrams.TryDequeue(out Tram? arrivingTram))
            {
                Platform platform = station.BestFreePlatform();
                Event e = new ArrivalEndstation(arrivingTram, station, platform);
                eventQueue.Enqueue(e, state.time + Sampling.switchClearanceTime());
            }
        }
    }
}
