using ADS_Simulation.NS_State;
using Priority_Queue;

namespace ADS_Simulation.Events
{
    abstract class Event : FastPriorityQueueNode
    {
        /// <summary>
        /// Execute the event and add new events to the queue
        /// </summary>
        /// <param name="state">Current state</param>
        /// <param name="eventQueue">Current event queue</param>
        public abstract void Execute(State state, FastPriorityQueue<Event> eventQueue);
    }
}
