using ADS_Simulation.NS_State;
using Priority_Queue;

namespace ADS_Simulation.Events
{
    class PassengerArrival : Event
    {
        public readonly int stationIdx;

        public PassengerArrival(int stationIdx)
        {
            this.stationIdx = stationIdx;
        }

        public override void Execute(State state, FastPriorityQueue<Event> eventQueue)
        {
            // Add passenger to station
            state.stations[stationIdx].waitingPassengers.Enqueue(state.time);
        }
    }
}
