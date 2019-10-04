using System;
using System.Collections.Generic;
using System.Text;
using ADS_Simulation.NS_State;
using Priority_Queue;

namespace ADS_Simulation.Events
{
    class ArrivalEndstation : Event
    {
        Tram tram;
        Endstation station;
        int platform;

        public ArrivalEndstation(Tram tram, Endstation station, int platform)
        {
            this.tram = tram;
            this.station = station;
            this.platform = platform;
        }

        public override void Execute(State state, FastPriorityQueue<Event> eventQueue)
        {
            // Clear switch
            SwitchLane lane = platform == 1 ? SwitchLane.Cross : SwitchLane.ArrivalLane;
            eventQueue.Enqueue(new ClearSwitchLane(station, lane), 0);

            // Check if tram can do another round trip if it is at endstaion with depot
            if (!station.TramToDepot(state.time))
            {
                // TODO take into account the dwell time of passengers
                // TODO dequeu passengers
                int nextDeparture = station.NextDeparture();
                Event e = new ExpectedDepartureStartstation();
                eventQueue.Enqueue(e, nextDeparture);
            }
            else
            {
                // Transfer tram to depot
                station.Free(platform);
            }
        }
    }
}
