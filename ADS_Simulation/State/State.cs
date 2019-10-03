using System;
using System.Collections.Generic;
using System.Text;

// NS_ prefix wegens namespace conflict
namespace ADS_Simulation.NS_State
{
    public class State
    {
        public int time;
        public List<Tram> trams;
        public List<Station> stations;

        public State(int startTime, List<Tram> ts, List<Station> ss)
        {
            time = startTime;
            trams = ts;
            stations = ss;
        }
    }
}
