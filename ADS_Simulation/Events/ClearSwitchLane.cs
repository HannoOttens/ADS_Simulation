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
            /* Kind of a critical point here:
             * - We have a possible departing tram
             * - We have a possible incoming tram
             * - The incoming tram can never go to the departing tram platform (not free yet)
             * This means that if the departing tram is on platform B, the arriving tram cannot enter because the cross will be used.
             * If the departing tram is on platform A, they can drive past eachother, so it's okay to queue the arrival. */

            // Clear the switch
            station.Switch.FreeSwitch(lane);

            // Check if tram is in queue at switch, give priority to departing trams
            (var departingTram, var departingPlatform) = station.GetFirstDepartingTram();
            if (departingTram != null)
            {
                // Queue the departure
                Event e = new DepartureStartstation(departingTram, station, departingPlatform);
                eventQueue.Enqueue(e, state.time);
            }
            
            // Check if we can enqueue an arrival as well
            Platform arrivalPlatform =  station.BestFreePlatform();
            if (station.HasQueue() && arrivalPlatform == Platform.B)
            {
                // Get best avaialbe platform && queue
                Tram arrivingTram = station.OccupyFromQueue(arrivalPlatform);

                // Queue the arrival
                Event e = new ArrivalEndstation(arrivingTram, station, arrivalPlatform);
                eventQueue.Enqueue(e, state.time + Sampling.switchClearanceTime());
            }
        }
    }
}
