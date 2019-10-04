using System;
using System.Collections.Generic;
using System.Text;

namespace ADS_Simulation.NS_State
{
    class Switch
    {
        private (bool cross, bool arrival, bool departure) state;

        public Switch()
        {
            state = (false, false, false);
        }

        public void FreeSwitch(SwitchLane lane)
        {
            switch (lane)
            {
                case SwitchLane.ArrivalLane:
                    state.arrival = false;
                    break;
                case SwitchLane.DepartureLane:
                    state.departure = false;
                    break;
                case SwitchLane.Cross:
                    state.cross = false;
                    break;
            }
        }

        public bool UseSwitchIfFree(SwitchLane lane)
        {
            bool isFree = false;
            switch (lane)
            {
                case SwitchLane.ArrivalLane:
                    isFree = !state.cross && !state.arrival;
                    if (isFree) state.arrival = true;
                    break;
                case SwitchLane.DepartureLane:
                    isFree = !state.cross && !state.departure;
                    if (isFree) state.departure = true;
                    break;
                case SwitchLane.Cross:
                    isFree = !state.cross && !state.arrival && !state.departure;
                    if (isFree) state.cross = true;
                    break;
            }
            return isFree;
        }
    }

    enum SwitchLane
    {
        ArrivalLane = 0,
        DepartureLane = 1,
        Cross = 2
    }
}
