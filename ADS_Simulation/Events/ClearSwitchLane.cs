using ADS_Simulation.NS_State;
using Priority_Queue;

namespace ADS_Simulation.Events
{
    class ClearSwitchLane : Event
    {
        readonly Endstation station;
        readonly SwitchLane lane;

        public ClearSwitchLane(Endstation station, SwitchLane lane)
        {
            this.station = station;
            this.lane = lane;
        }

        public override void Execute(State state, FastPriorityQueue<Event> eventQueue)
        {

            System.Diagnostics.Debug.WriteLine($"ClearSwitchLane: {station.name}, {lane}");

            /* Kind of a critical point here:
             * - We have a possible departing tram
             * - We have a possible incoming tram
             * - The incoming tram can never go to the departing tram platform (not free yet)
             * This means that if the departing tram is on platform B, the arriving tram cannot enter because the cross will be used.
             * If the departing tram is on platform A, they can drive past each other, so it's okay to queue the arrival. */

            // Clear the switch
            station._switch.FreeSwitch(lane);

            // Check if tram is in queue at switch, give priority to departing trams
            var (departingTram, departingPlatform) = station.GetFirstDepartingTram();
            if (departingTram != null)
            {
                // Queue the departure
                SwitchLane departureLane = Switch.ExitLaneFor(departingPlatform);
                station._switch.UseSwitchLane(departureLane);

                Event e = new DepartureStartstation(departingTram, station, departingPlatform);
                eventQueue.Enqueue(e, state.time);
            }

            // Get the best free platform
            Platform arrivalPlatform = station.BestFreePlatform();
            if (arrivalPlatform != Platform.None)
            {
                // Check if there are trams arriving from the depot, if so prefer those.
                if (station.depotQueue.Count > 0)
                {
                    Tram arrivingTram = station.OccupyFromDepotQueue(arrivalPlatform);
                    Event e = new ArrivalEndstation(arrivingTram, station, arrivalPlatform, true);
                    eventQueue.Enqueue(e, state.time);
                }
                // Check if we can enqueue an arrival as well
                else if (station.HasQueue() && station._switch.SwitchLaneFree(Switch.ArrivalLaneFor(arrivalPlatform)))
                {
                    // Get best available platform && queue
                    Tram arrivingTram = station.OccupyFromQueue(arrivalPlatform);
                    SwitchLane lane = Switch.ArrivalLaneFor(arrivalPlatform);
                    station._switch.UseSwitchLane(lane);

                    // Queue the arrival
                    Event e = new ArrivalEndstation(arrivingTram, station, arrivalPlatform);
                    eventQueue.Enqueue(e, state.time + Sampling.switchClearanceTime());
                }
            }
        }
    }
}
