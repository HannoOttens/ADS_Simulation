using System;
using System.Collections.Generic;
using System.Text;

namespace ADS_Simulation.NS_State
{
    public enum Direction
    {
        /// <summary>
        /// P+R to Centraal Station
        /// </summary>
        A,
        /// <summary>
        /// Centraal Station to P+R
        /// </summary>
        B,
        /// <summary>
        /// Endstation (P+R or Centraal Station)
        /// </summary>
        END
    }

    public class Station
    {
        public Queue<Tram> incomingTrams;
        public string name;
        public Direction direction;
        public Queue<int> waitingPassengers;

        public Tram? occupant;

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
        public Tram OccupyFromQueue()
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
        public void Occupy(Tram tram)
        {
            if (occupant is null)
                occupant = tram;
            else
                throw new Exception($"{tram.id} tried to occupy {name} in direction {direction}, but the station was full.");
            tram.IsDriving = false;
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
            occupant.IsDriving = true;
            occupant = null;
        }

        /// <summary>
        /// Remove passengers from station
        /// </summary>
        /// <param name="maxPassengers">Maximum amount that will still fit in the tram</param>
        /// <returns>The amount of boarded passengers</returns>
        public int BoardPassengers(Tram tram)
        {
            int maxPassengers = tram.PassengerSpace();
            int count = 0;

            while (count < maxPassengers && waitingPassengers.Count > 0)
            {
                waitingPassengers.Dequeue();
                count++;
            }

            tram.AddPassengers(count);
            return count;
        }

        /// <summary>
        /// Check if the station has waiting passengers
        /// </summary>
        /// <returns>True when there are passengers waiting</returns>
        public bool HasPassengers()
        {
            return waitingPassengers.Count > 0;
        }

        /// <summary>
        /// First unboard, then board passengers
        /// </summary>
        /// <param name="tram"></param>
        /// <returns></returns>
        public (int pOut, int pIn) UnboardAndBoard(Tram tram)
        {
            // First empty the tram
            int pOut = tram.EmptyPassengers(); //TODO: Don;t empty, but stochastic number

            // Then board the passengers
            int pIn = BoardPassengers(tram);

            return (pOut, pIn);
        }

    }
}
