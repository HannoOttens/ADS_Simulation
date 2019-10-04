﻿using ADS_Simulation.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADS_Simulation.NS_State
{
    class Endstation : Station
    {
        public Switch Switch;
        public Queue<Tram> departingTrams;
        private bool hasDepot;
        private TimeTable timeTable;

        // tram at platform 2
        public Tram? occupant2;

        public Endstation(string name, bool hasDepot) : base(name, Direction.END)
        {
            this.hasDepot = hasDepot;

            Switch = new Switch();
            departingTrams = new Queue<Tram>();
            
            //TODO: Juiste start-offset
            timeTable = new TimeTable(0, Config.c.GetIntervalSeconds());
        }

        /// <summary>
        /// Check if tram should be brought to the depot (end of service)
        /// </summary>
        /// <param name="currentTime">The current time</param>
        /// <returns>if the tram should be stopped</returns>
        public bool TramToDepot(int currentTime)
        {
            // Trip time is one-way driving time times two plus the turn-around time
            int roundTripTime = Configuration.Config.c.oneWayTripTimeMinutes * 60 * 2
                + Configuration.Config.c.turnAroundTimeMinutes * 60;
            return hasDepot && Configuration.Config.c.endTime - currentTime >= roundTripTime;
        }

        /// <summary>
        /// Returns the best platform to enter on
        /// </summary>
        /// <returns>1 or 2 for platform, -1 if unable to enter</returns>
        public int BestFreePlatform()
        {
            if (IsFree(1) && Switch.SwitchLaneFree(SwitchLane.Cross))
                return 1;
            else if (IsFree(2) && Switch.SwitchLaneFree(SwitchLane.ArrivalLane))
                return 2;
            else return -1;
        }

        /// <summary>
        /// Get the time of next departure
        /// </summary>
        /// <returns></returns>
        public int NextDeparture()
        {
            return timeTable.Next();
        }

        /// <summary>
        /// Occupy a platform of the station
        /// </summary>
        /// <param name="tram">The new occupant</param>
        /// <param name="platform">The target platform</param>
        internal void Occupy(Tram tram, int platform)
        {
            if (platform == 1)
            {
                if (occupant != null) throw new Exception($"Tram {tram.id} tried to occupy {name} with platform {platform} but that platform was already occupied by {occupant.id}");
                occupant = tram;
            }
            else
            {
                if (occupant2 != null) throw new Exception($"Tram {tram.id} tried to occupy {name} with platform {platform} but that platform was already occupied by {occupant2.id}");
                occupant2 = tram;
            }
        }

        /// <summary>
        /// Free an endstation platform
        /// </summary>
        /// <param name="platform"></param>
        public void Free(int platform)
        {
            if (platform == 1)
            {
                if (occupant == null) throw new Exception($"Tried to free platform {platform} on {name}, but platform was free already.");
                occupant = null;
            }
            else
            {
                if (occupant2 == null) throw new Exception($"Tried to free platform {platform} on {name}, but platform was free already.");
                occupant2 = null;
            }
        }

        /// <summary>
        /// Check if platform is free
        /// </summary>
        /// <param name="platform">The target platform</param>
        /// <returns>If the platform</returns>
        public bool IsFree(int platform)
        {
            if (platform == 1)
                return IsFree();
            else return occupant2 == null;
        }
    }
}
