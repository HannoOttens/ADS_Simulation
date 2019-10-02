using System;
using System.Collections.Generic;
using System.Text;

namespace ADS_Simulation.NS_State
{
    public class Tram
    {
        int id;
        int passengerCount;
        Direction direction;

        public Tram(int id) {
            this.id = id;
            passengerCount = 0;
            direction = Direction.WestBound;
        }
    }

    public enum Direction
    {
        WestBound, // Towards Central Station
        EastBound // Towards P+R
    }
}
