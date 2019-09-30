using System;
using System.Collections.Generic;
using System.Text;
using ADS_Simulation.NS_State;
using Priority_Queue;

namespace ADS_Simulation.Events
{
    class TramDeparture : Event
    {
        public override void Execute(State state, StablePriorityQueue<Event> eventQueue)
        {
            throw new NotImplementedException();
        }
    }
}
