using System;
using System.Collections.Generic;
using System.Text;

namespace ADS_Simulation.NS_State
{
    class Endstation : Station
    {
        private bool hasDepot;
        public Switch Switch;
        public Queue<Tram> departingTrams;

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

        public override void Free(int platform = 1)
        {
            if (platform == 1)
                base.Free(platform);
            else
                occupant2 = null;
        }

        public override bool IsFree(int platform = 1)
        {
            if (platform == 1)
                return base.IsFree(platform);
            else return occupant2 == null;
        }
    }
}
