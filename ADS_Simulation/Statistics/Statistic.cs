using ADS_Simulation.NS_State;

namespace ADS_Simulation.Statistics
{
    abstract class Statistic
    {
        public abstract void measure(State state);
        public abstract void Print(State state);
    }
}
