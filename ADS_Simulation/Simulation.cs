using ADS_Simulation.Events;
using ADS_Simulation.NS_State;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADS_Simulation
{
    class Simulation
    {
        private const int MAX_EVENTS = 10000;

        State state;
        StablePriorityQueue<Event> eventQueue;
        ulong simulationClock;

        public Simulation()
        {
            state = new State();
            simulationClock = 0;
            eventQueue = new StablePriorityQueue<Event>(MAX_EVENTS);
        }
        public bool stoppingConditionMet()
        {
            return false;
        }
    }
}
