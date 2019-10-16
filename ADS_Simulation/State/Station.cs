using System;
using System.Collections.Generic;
using System.Diagnostics;

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
        public readonly Queue<Tram> incomingTrams;
        public readonly string name;
        public readonly Direction direction;
        public readonly Queue<int> waitingPassengers;

        public Tram? occupant;
        private int lastOccupantId = -1;

        /// <summary>
        /// The time the previous tram would arrive at the next station.
        /// When departing a tram, it cannot arrive earlier at the next station than this timestamp.
        /// </summary>
        public int lastSignaledArrivalTime = 0;

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
                Occupy(incomingTrams.Dequeue());
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
            {
                occupant = tram;
                Trace.Assert(lastOccupantId < 0 || occupant.id - 1 == lastOccupantId || occupant.id == 6001, 
                    $"Tram arrived in wrong order, {lastOccupantId} arrived before {occupant.id}");
                lastOccupantId = occupant.id;
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
        public (int pOut, int pIn) UnboardAndBoard(Tram tram, int maxUnboard)
        {
            // First empty the tram
            int pOut = tram.EmptyPassengers(maxUnboard); 

            // Then board the passengers
            int pIn = BoardPassengers(tram);

            return (pOut, pIn);
        }
    }
}
