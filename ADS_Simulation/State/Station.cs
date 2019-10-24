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
        private int lastSignaledArrivalTime = 0;

        public Station(string name, Direction direction)
        {
            this.name = name;
            this.direction = direction;
            waitingPassengers = new Queue<int>();
            incomingTrams = new Queue<Tram>();
        }

        /// <summary>
        /// Let tram enter from queue
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
                    $"Tram arrived in wrong order, {occupant.id} arrived after {lastOccupantId} at {name} ({direction})");
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
        /// <returns>True when station has no more trams in queue</returns>
        public void Free()
        {
            occupant = null;
        }

        /// <summary>
        /// Remove passengers from station
        /// </summary>
        /// <param name="maxPassengers">Maximum amount that will still fit in the tram</param>
        /// <returns>The amount of boarded passengers, and their arrival times</returns>
        public (int, List<int>) BoardPassengers(Tram tram)
        {
            List<int> entrances = new List<int>();
            int maxPassengers = tram.PassengerSpace();
            
            int count = 0;
            while (count < maxPassengers && waitingPassengers.Count > 0)
            {
                entrances.Add(waitingPassengers.Dequeue());
                count++;
            }

            tram.AddPassengers(count);
            return (count, entrances);
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
        public (int pOut, int pIn, List<int> entrances) UnboardAndBoard(Tram tram, int maxUnboard)
        {
            // First empty the tram
            int pOut = tram.EmptyPassengers(maxUnboard); 

            // Then board the passengers
            var (pIn, entrances) = BoardPassengers(tram);

            return (pOut, pIn, entrances);
        }

        /// <summary>
        /// Make sure trams do not take over each other.
        /// </summary>
        /// <param name="stochasticArrivalTime">The arrival time from the distribution</param>
        /// <returns>Actual arrival time</returns>
        public int SignalNextArrival(int stochasticArrivalTime)
        {
            int arrivalTime = Math.Max(stochasticArrivalTime, lastSignaledArrivalTime + 1);
            lastSignaledArrivalTime = arrivalTime;
            return arrivalTime;
        }
    }
}
