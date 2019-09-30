using ADS_Simulation.Events;
using ADS_Simulation.NS_State;
using ADS_Simulation.Statistics;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADS_Simulation
{
    class Simulation
    {
        private const int MAX_EVENTS = 10000; // NOG GEEN IDEE OF DIT PAST
        private const int MAX_TIME = 36000; //KLOPT NOG NIET

        State state;
        StablePriorityQueue<Event> eventQueue;
        List<Statistic> statistics;
        ulong simulationClock;

        public Simulation()
        {
            state = new State();
            simulationClock = 0;
            eventQueue = new StablePriorityQueue<Event>(MAX_EVENTS);
            statistics = new List<Statistic>()
            {
                new PassengerWaitStatistic(),
                new TramLoadStatistic(),
                new EmptyStationStatistic()
            };
        }

        internal void Step()
        {
            // Get next event and execute
            Event _event = eventQueue.Dequeue();
            _event.Execute(state, eventQueue);

            // Advance clock
            simulationClock = _event.time;

            // Measure statistics
            foreach (var statistic in statistics)
                statistic.measure(state);
        }

        /// <summary>
        /// Check if the stopping condition of simulation has been met
        /// </summary>
        /// <returns></returns>
        public bool StoppingConditionMet()
        {
            return eventQueue.Count == 0
                || simulationClock >= MAX_TIME;
        }
    }
}
