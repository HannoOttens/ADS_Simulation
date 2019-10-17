using ADS_Simulation.NS_State;
using Priority_Queue;

namespace ADS_Simulation.Events
{
    class ExpectedArrivalEndstation : Event
    {
        public readonly Tram tram;
        public readonly Endstation station;

        public ExpectedArrivalEndstation(Tram tram, Endstation station)
        {
            this.tram = tram;
            this.station = station;
        }

        public override void Execute(State state, FastPriorityQueue<Event> eventQueue)
        {
            System.Diagnostics.Debug.WriteLine($"ExpectedArrivalEndstation: tram {tram.id}, station: {station.name}, time: {state.time}");

            // Check if there is a free platform available and if switch lane is free
            Platform bestPlatform = station.BestFreePlatform();
            if(station.depotQueue.Count == 0 && bestPlatform != Platform.None)
            {
                station.Occupy(tram, bestPlatform);
                station._switch.UseSwitchLane(Switch.ArrivalLaneFor(bestPlatform));
                eventQueue.Enqueue(new ArrivalEndstation(tram, station, bestPlatform), state.time + Sampling.switchClearanceTime());
            }
            else
                station.Enqueue(tram);
        }
    }
}
