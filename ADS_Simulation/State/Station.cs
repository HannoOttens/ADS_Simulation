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
        public Queue<Tram> incomingTrams;
        public string name;
        public Direction direction;
        public Tram? occupant;

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

        public virtual bool IsFree(int platform = 1)
        {
            return occupant == null;
        }

        /// <summary>
        /// Free the station
        /// </summary>
        /// <returns>True when station has no more trains in queue</returns>
        public virtual void Free(int platform = 1)
        {
            occupant = null;
        }

        public int DequePassengers(int currentTime, int capacity)
        {
            int i = 0;
            while (waitingPassengers.Count > 0 && i <= capacity)
            {
                int arrivalTime = waitingPassengers.Dequeue();
                int waitingTime = currentTime - arrivalTime;
                //TODO pass waiting time to statistics
                i++;
            }
            return i;
        }

        public int ExitingPassengers(int currentTime)
        {
            throw new NotImplementedException();
            //TODO determine amount of passangers that will exit the tram
        }
    }
}
