using ADS_Simulation.Events;
using ADS_Simulation.NS_State;

namespace ADS_Simulation.Statistics
{
    abstract class Statistic
    {
        public int endTime;
        public int startTime;

        public Statistic(int startTime, int endTime)
        {
            this.startTime = startTime;
            this.endTime = endTime;
        }

        public abstract void measure(State state, Event currentEvent);
        public abstract void Print(State state);
        public abstract string[] GetHeaders();
        public abstract string[] GetValues(State state);
    }
}
