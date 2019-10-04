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
        private Queue<Tram> incomingTrams;
        public string name;
        public Direction direction;
        Tram? occupant;

        // TODO split queue depending on direction
        public Queue<int> waitingPassengers; // Passengers waiting for trip to Central Station
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
            if (occupant is null)
            {
                occupant = incomingTrams.Dequeue();
                return occupant;
            }
            else throw new Exception($"Tried to occupy {name} in direction {direction} from the incomingTrams queue, but the station was full.");
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
        /// <returns>True if tram could arrive at station</returns>
        public bool Occupy(Tram tram)
        {
            if (occupant is null)
            {
                occupant = tram;
                return true;
            }
            else
                throw new Exception($"{tram.id} tried to occupy {name} in direction {direction}, but the station was full.");
        }

        // Put tram in station queue
        public void Enqueue(Tram tram)
        {
            incomingTrams.Enqueue(tram);
        }

        /// <summary>
        /// Check if station is free
        /// </summary>
        /// <returns></returns>
        public bool IsFree()
        {
            return occupant == null;
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
