﻿using System;
using System.Collections.Generic;
using ADS_Simulation.Configuration;
using ADS_Simulation.NS_State;
using Priority_Queue;

namespace ADS_Simulation.Events
{
    class ArrivalEndstation : Event
    {
        readonly Tram tram;
        public Endstation station;
        readonly Platform platform;
        readonly bool fromDepot;
        public List<int> entrances = new List<int>();

        public ArrivalEndstation(Tram tram, Endstation station, Platform platform, bool fromDepot = false)
        {
            this.tram = tram;
            this.station = station;
            this.platform = platform;
            this.fromDepot = fromDepot;
        }

        public override void Execute(State state, FastPriorityQueue<Event> eventQueue)
        {

            // Clear switch (no lane cleared if coming from depot)
            SwitchLane lane = fromDepot ? SwitchLane.None : Switch.ArrivalLaneFor(platform);
            eventQueue.Enqueue(new ClearSwitchLane(station, lane), state.time + Config.c.switchClearanceTime);
            
            // Log
            System.Diagnostics.Debug.WriteLine($"ArrivalEndstation: tram {tram.id}, station: {station.name}, {platform}, {lane}, time: {state.time}"); 

            // Check if tram can do another round trip if it is at endstation with depot
            if (!station.TramToDepot(state.time))
            {
                // Board and unboard
                (int pOut, int pIn, List<int> e) = station.UnboardAndBoard(tram, tram.passengerCount);
                entrances = e;

                // Calculate when to schedule departure
                // If boarding/unboarding takes shorter than the turnaround, take the turnaround time
                int passengerTransferTime = state.time + Math.Max(Sampling.passengerExchangeTime(pOut, pIn), Config.c.turnAroundTimeFor(station));
                int nextDepartureTime = station.NextDeparture();
                int nextEventTime = Math.Max(passengerTransferTime, nextDepartureTime);

                // Queue event
                eventQueue.Enqueue(new ExpectedDepartureStartstation(tram, station, platform, nextDepartureTime), nextEventTime);
            }
            // Transfer tram to depot
            else
                station.Free(platform);
        }
    }
}
