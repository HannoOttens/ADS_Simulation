using ADS_Simulation.Events;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADS_Simulation
{
    class Simulation
    {
        StablePriorityQueue<Event> eventQueue = new StablePriorityQueue<Event>(1000);
    }
}
