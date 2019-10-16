using ADS_Simulation.Configuration;
using ADS_Simulation.NS_State;
using Priority_Queue;
using System;

namespace ADS_Simulation.Events
{
    class DepartureStartstation : Event
    {
        public readonly Tram tram;
        public readonly Endstation station;
        public readonly Platform platform;

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

            // Clear the lane it's leaving over in 60s
            eventQueue.Enqueue(new ClearSwitchLane(station, lane), state.time + Sampling.switchClearanceTime());

            // Queue next arrival
            int stationIndex = state.stations.IndexOf(station);
            int newStationIndex = stationIndex + 1;

            // Make sure trams do not take over eachother
            int drivingTime = state.time + Sampling.drivingTime(Config.c.transferTimes[stationIndex].averageTime);
            int arrivalTime = Math.Max(state.time + drivingTime, station.lastSignaledArrivalTime + 1);
            station.lastSignaledArrivalTime = arrivalTime;
            eventQueue.Enqueue(new ExpectedTramArrival(tram, newStationIndex), arrivalTime);
        }
    }
}
