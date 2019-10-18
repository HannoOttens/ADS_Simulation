using ADS_Simulation.NS_State;
using Priority_Queue;

namespace ADS_Simulation.Events
{
    class PassengerArrival : Event
    {
        public readonly int stationIndex;

        public PassengerArrival(int stationIdx)
        {
            this.stationIndex = stationIdx;
        }

        public override void Execute(State state, FastPriorityQueue<Event> eventQueue)
        {
            // Add passenger to station
            state.stations[stationIndex].waitingPassengers.Enqueue(state.time);
        }
    }
}
