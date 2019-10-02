using System;
using System.Collections.Generic;
using System.Text;

namespace ADS_Simulation.NS_State
{
    public enum Direction
    {
        /// <summary>
        /// P+R naar Centraal Station
        /// </summary>
        A,
        /// <summary>
        /// Centraal Station naar P+R
        /// </summary>
        B,
        /// <summary>
        /// Eindstation (P+R of Centraal Station)
        /// </summary>
        END
    }

    public class Station
    {
        string name;
        public Direction direction;
        public Queue<int> waitingPassengers;

        public Station(string name, Direction direction)
        {
            this.name = name;
            this.direction = direction;
            waitingPassengers = new Queue<int>();
        }
    }
}
