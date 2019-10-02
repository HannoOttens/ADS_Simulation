using System;
using System.Collections.Generic;
using System.Text;

namespace ADS_Simulation.NS_State
{
    class Switch
    {
        /// <summary>
        /// Straight track from 'int'
        /// </summary>
        Tram? inOccupant;
        /// <summary>
        /// Straight track to 'out'
        /// </summary>
        Tram? outOccupant;
        /// <summary>
        /// Crossing track
        /// </summary>
        Tram? crossOccupant;

        Queue<Tram> incomingTrams;

        public Switch()
        {
            incomingTrams = new Queue<Tram>();
        }
    }
}
