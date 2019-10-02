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
        public string name;
        public Direction direction;
        public Queue<int> waitingPassengers;

        private Tram? occupant;
        private Queue<Tram> incomingTrams;

        public Station(string name, Direction direction)
        {
            this.name = name;
            this.direction = direction;
            waitingPassengers = new Queue<int>();
            incomingTrams = new Queue<Tram>();
        }

        /// <summary>
        /// Let train enter from queue
        /// </summary>
        /// <returns>The new occupant</returns>
        internal Tram OccupyFromQueue()
        {
            occupant = incomingTrams.Dequeue();
            return occupant;
        }

        /// <summary>
        /// Check if station has a queue of trams
        /// </summary>
        /// <returns>True when station has queue</returns>
        public bool HasQueue()
        {
            return incomingTrams.Count > 0;
        }

        /// <summary>
        /// Try enter the station
        /// </summary>
        /// <param name="tram"></param>
        /// <returns>True if arrived at station, false when placed in queue</returns>
        public bool TryOccupy(Tram tram)
        {
            if(occupant is null)
            {
                occupant = tram;
                return true;
            }
            else
            {
                incomingTrams.Enqueue(tram);
                return false;
            }
        }

        /// <summary>
        /// Free the station
        /// </summary>
        /// <returns>True when station has no more trains in queue</returns>
        public void Free()
        {
            occupant = null;
        }
    }
}
