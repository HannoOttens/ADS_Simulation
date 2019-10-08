using ADS_Simulation.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ADS_Simulation.NS_State
{
    enum Platform
    {
        /// <summary>
        /// Platform connected to the departure track
        /// </summary>
        A,
        /// <summary>
        /// Platform connected to the arrival track
        /// </summary>
        B,
        None
    }

    class Endstation : Station
    {
        public Switch _switch;
        private bool hasDepot;
        private TimeTable timeTable;

        // tram at platform 2
        public Tram? occupant2;

        public Endstation(string name, bool hasDepot) : base(name, Direction.END)
        {
            this.hasDepot = hasDepot;
            _switch = new Switch();
            
            //TODO: Juiste start-offset
            timeTable = new TimeTable(0, Config.c.GetIntervalSeconds());
        }

        public (Tram? departingTram, Platform platform) GetFirstDepartingTram()
        {
            //TODO: Get the FIRST!! departing tram (A is not nesserily first)

            if (occupant != null 
                && occupant.IsReadyForDeparture() 
                && _switch.SwitchLaneFree(Switch.ExitLaneFor(Platform.A)))
            {
                return (occupant, Platform.A);
            }
            else if (occupant2 != null 
                && occupant2.IsReadyForDeparture()
                && _switch.SwitchLaneFree(Switch.ExitLaneFor(Platform.B)))
            {
                return (occupant2, Platform.B);
            }

            return (null, Platform.None);
        }

        /// <summary>
        /// Get platform corresponing to occupant
        /// </summary>
        /// <param name="tram">Tram</param>
        /// <returns>Platform at which the tram is stationed</returns>
        private Platform PlatformForOccupant(Tram tram)
        {
            if (tram == occupant)
                return Platform.A;
            else if (tram == occupant2)
                return Platform.B;

            throw new Exception($"Tried to find platform corresponding to tram {tram.id}, but none was found.");
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
            return hasDepot && Configuration.Config.c.endTime - currentTime <= roundTripTime;
        }

        /// <summary>
        /// Returns the best platform to enter on
        /// </summary>
        /// <returns>1 or 2 for platform, -1 if unable to enter</returns>
        public Platform BestFreePlatform()
        {
            if (IsFree(Platform.A) && _switch.SwitchLaneFree(SwitchLane.Cross))
                return Platform.A;
            else if (IsFree(Platform.B) && _switch.SwitchLaneFree(SwitchLane.ArrivalLane))
                return Platform.B;
            else return Platform.None;
        }

        /// <summary>
        /// Get the time of next departure
        /// </summary>
        /// <returns>The time of next departure</returns>
        public int NextDeparture()
        {
            return timeTable.Next();
        }

        /// <summary>
        /// Occupy a platform of the station
        /// </summary>
        /// <param name="tram">The new occupant</param>
        /// <param name="platform">The target platform</param>
        internal void Occupy(Tram tram, Platform platform)
        {
            if (platform == Platform.A)
            {
                Trace.Assert(occupant == null, $"Tram {tram.id} tried to occupy {name} with platform {platform} but that platform was already occupied by {occupant?.id}");
                occupant = tram;
                occupant.IsDriving = false;
            }
            else if (platform == Platform.B)
            {
                Trace.Assert(occupant2 == null, $"Tram {tram.id} tried to occupy {name} with platform {platform} but that platform was already occupied by {occupant2?.id}");
                occupant2 = tram;
                occupant2.IsDriving = false;
            }
            else throw new Exception($"Unknown platform {platform}.");
        }

        /// <summary>
        /// Free an endstation platform
        /// </summary>
        /// <param name="platform"></param>
        public void Free(Platform platform)
        {
            if (platform == Platform.A)
            {
                Trace.Assert(occupant != null, $"Tried to free platform {platform} on {name}, but platform was free already.");
                occupant.IsDriving = true;
                occupant = null;
            }
            else
            {
                Trace.Assert(occupant2 != null, $"Tried to free platform {platform} on {name}, but platform was free already.");
                occupant2.IsDriving = true;
                occupant2 = null;
            }
        }

        /// <summary>
        /// Check if platform is free
        /// </summary>
        /// <param name="platform">The target platform</param>
        /// <returns>If the platform</returns>
        public bool IsFree(Platform platform)
        {
            if (platform == Platform.A)
                return occupant == null;
            else 
                return occupant2 == null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Tram OccupyFromQueue(Platform platform)
        {
            Trace.Assert(IsFree(platform), $"Tried to occupy {platform} from queue, but platform was already taken.");

            Tram tram = incomingTrams.Dequeue();
            if (platform == Platform.A)
                occupant = tram;
            else
                occupant2 = tram;

            return tram;
        }
    }
}
