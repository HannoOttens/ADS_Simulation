using System;
using System.Collections.Generic;
using System.Text;

namespace ADS_Simulation.NS_State
{
    public class Tram
    {
        public int id;
        public int passengerCount;

        public Tram(int id) {
            this.id = id;
            passengerCount = 0;
        }
    }
}
