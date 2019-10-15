using System.Collections.Generic;

// NS_ prefix due to namespace conflict
namespace ADS_Simulation.NS_State
{
    public class State
    {
        public int time;
        public List<Tram> trams;
        public List<Station> stations;

        public State(List<Tram> ts, List<Station> ss)
        {
            trams = ts;
            stations = ss;
        }
    }
}
