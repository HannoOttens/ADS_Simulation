﻿using System;
using System.Collections.Generic;
using System.Text;
using ADS_Simulation.NS_State;
using Priority_Queue;

namespace ADS_Simulation.Events
{
    class DepartureStartstation : Event
    {
        private Tram tram;
        private Endstation station;
        private Platform platform;

        public DepartureStartstation(Tram tram, Endstation station, Platform platform)
        {
            this.tram = tram;
            this.station = station;
            this.platform = platform;
        }

        public override void Execute(State state, FastPriorityQueue<Event> eventQueue)
        {
            // Free the platform
            station.Free(platform);

            // Reset ready for departure
            tram.ResetReadyForDeparture();
            
            // Claim lane
            SwitchLane lane = Switch.ExitLaneFor(platform);
            System.Diagnostics.Debug.WriteLine($"DepartureStartstation: tram {tram.id}, station: {station.name}, {platform}, {lane}");
            station.Switch.UseSwitchLane(lane);

            // Clear the lane it's leaving over in 60s
            eventQueue.Enqueue(new ClearSwitchLane(station, lane), state.time + Sampling.switchClearanceTime());

            // Queue next arrival
            int stationIndex = state.stations.IndexOf(station);
            int newStationIndex = stationIndex + 1;
            eventQueue.Enqueue(new TramArrival(tram, newStationIndex), state.time + Sampling.drivingTime(100)); //TODO Fix avarage for part
        }
    }
}
