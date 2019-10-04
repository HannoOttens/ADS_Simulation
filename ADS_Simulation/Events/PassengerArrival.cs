using System;
using System.Collections.Generic;
using System.Text;
using ADS_Simulation.NS_State;
using Priority_Queue;

namespace ADS_Simulation.Events
{
    class PassengerArrival : Event
    {
        private int _stationIdx;

        public PassengerArrival(int stationIdx)
        {
            _stationIdx = stationIdx;
        }

        public override void Execute(State state, StablePriorityQueue<Event> eventQueue)
        {
            // Add passenger to station
            state.stations[_stationIdx].waitingPassengers.Enqueue(state.time);

            //Enqueue next passenger arrival
            int timeUntilNext = Sampling.timeUntilNextPassenger(state.time);
            eventQueue.Enqueue(new PassengerArrival(_stationIdx), state.time + timeUntilNext);
        }
    }
}
