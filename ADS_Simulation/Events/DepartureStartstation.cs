using System;
using System.Collections.Generic;
using System.Text;
using ADS_Simulation.NS_State;
using Priority_Queue;

namespace ADS_Simulation.Events
{
    class DepartureStartstation : Event
    {
        public override void Execute(State state, FastPriorityQueue<Event> eventQueue)
        {
            throw new NotImplementedException();
        }
    }
}
