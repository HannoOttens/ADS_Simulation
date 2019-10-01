using ADS_Simulation.Configuration;
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

        State state;
        StablePriorityQueue<Event> eventQueue;
        List<Statistic> statistics;

        public Simulation()
        {
            state = new State();
            eventQueue = new StablePriorityQueue<Event>(MAX_EVENTS);
            statistics = new List<Statistic>()
            {
                new PassengerWaitStatistic(),
                new TramLoadStatistic(),
                new EmptyStationStatistic()
            };
        }

        /// <summary>
        /// Make one step in the simulation
        /// </summary>
        /// <returns>If simulation has ended</returns>
        public bool Step()
        {
            // Get next event and execute
            Event _event = eventQueue.Dequeue();
            // Advance clock (mss deel maken van de event.Execute?)
            state.simulationClock = _event.time;
            _event.Execute(state, eventQueue);

            // Measure statistics
            foreach (var statistic in statistics)
                statistic.measure(state);

            // Return if should be stopped
            return StoppingConditionMet();
        }

        /// <summary>
        /// Check if the stopping condition of simulation has been met
        /// </summary>
        private bool StoppingConditionMet()
        {
            return eventQueue.Count == 0
                || state.simulationClock >= Config.c.endTime;
        }
    }
}
