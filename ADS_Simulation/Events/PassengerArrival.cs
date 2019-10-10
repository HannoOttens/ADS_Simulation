using ADS_Simulation.Configuration;
using ADS_Simulation.NS_State;
using Priority_Queue;

namespace ADS_Simulation.Events
{
    class PassengerArrival : Event
    {
        private readonly int _stationIdx;

        public PassengerArrival(int stationIdx)
        {
            _stationIdx = stationIdx;
        }

        public override void Execute(State state, FastPriorityQueue<Event> eventQueue)
        {
            // Add passenger to station
            state.stations[_stationIdx].waitingPassengers.Enqueue(state.time);

            // Don't queue new passengers after endtime
            if (state.time > Config.c.endTime) return;

            //Enqueue next passenger arrival
            int timeUntilNext = Sampling.timeUntilNextPassenger(state.time, _stationIdx);
            eventQueue.Enqueue(new PassengerArrival(_stationIdx), state.time + timeUntilNext);
        }
    }
}
