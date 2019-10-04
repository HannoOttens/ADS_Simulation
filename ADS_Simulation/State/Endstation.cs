using System;
using System.Collections.Generic;
using System.Text;

namespace ADS_Simulation.NS_State
{
    class Endstation : Station
    {
        Switch _switch;
        public Endstation(string name) : base(name, Direction.END)
        {
            _switch = new Switch();
        }
    }
}
