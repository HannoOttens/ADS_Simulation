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
            if (SwitchLaneFree(lane))
                throw new Exception($"Tried to free {lane} but it was already free.");

            _ = lane switch
            {
                SwitchLane.ArrivalLane => state.arrival = false,
                SwitchLane.DepartureLane => state.departure = false,
                SwitchLane.Cross => state.cross = false,
                _ => throw new Exception("Unknown switchlane")
            };
        }

        public void UseSwitchLane(SwitchLane lane)
        {
            if (!SwitchLaneFree(lane))
                throw new Exception($"Tried to use {lane} but it was not free.");

            _ = lane switch
            {
                SwitchLane.ArrivalLane => state.arrival = true,
                SwitchLane.DepartureLane => state.departure = true,
                SwitchLane.Cross => state.cross = true,
                _ => throw new Exception("Unknown switchlane")
            };
        }

        public SwitchLane ExitLaneFor(Platform platform)
         => platform switch
         {
             Platform.A => SwitchLane.Cross,
             Platform.B => SwitchLane.DepartureLane,
             _ => throw new Exception("Unknown platform")
         };

        public bool SwitchLaneFree(SwitchLane lane)
        {
            return lane switch
            {
                SwitchLane.ArrivalLane => !state.cross && !state.arrival,
                SwitchLane.DepartureLane => !state.cross && !state.departure,
                SwitchLane.Cross => !state.cross && !state.arrival && !state.departure,
                _ => throw new Exception("Unknown switchlane")
            };
        }
    }

    enum SwitchLane
    {
        ArrivalLane = 0,
        DepartureLane = 1,
        Cross = 2
    }
}

