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

        // tram at platform 2
        public Tram? occupant2;

        public Endstation(string name, bool hasDepot) : base(name, Direction.END)
        {
            this.hasDepot = hasDepot;
            Switch = new Switch();
            departingTrams = new Queue<Tram>();
        }

        public bool TramToDepot(int currentTime)
        {
            // Trip time is one-way driving time times two plus the turn-around time
            int roundTripTime = Configuration.Config.c.oneWayTripTimeMinutes * 60 * 2 
                + Configuration.Config.c.turnAroundTimeMinutes * 60;
            return hasDepot && Configuration.Config.c.endTime - currentTime >= roundTripTime;
        }

        public int NextDeparture(int currentTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Free an endstation platform
        /// </summary>
        /// <param name="platform"></param>
        public void Free(int platform)
        {
            if (platform == 1)
                Free(platform);
            else
                occupant2 = null;
        }

        public bool IsFree(int platform)
        {
            if (platform == 1)
                return IsFree();
            else return occupant2 == null;
        }
    }
}
