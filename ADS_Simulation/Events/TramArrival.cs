﻿using System.Collections.Generic;
using ADS_Simulation.NS_State;
using Priority_Queue;

namespace ADS_Simulation.Events
{
    class TramArrival : Event
    {
        private readonly Tram tram;
        public int stationIndex;
        public List<int> entrances = new List<int>();

        public TramArrival(Tram tram, int stationIndex)
        {
            this.tram = tram;
            this.stationIndex = stationIndex;
        }

        public override void Execute(State state, FastPriorityQueue<Event> eventQueue)
        {
            var station = state.stations[stationIndex];
            System.Diagnostics.Debug.WriteLine($"TramArrival: tram {tram.id}, station: {station.name}, dir: {station.direction}, time: {state.time}");

            // Board and unboard passengers
            (int pOut, int pIn, List<int> ent) = station.UnboardAndBoard(tram, Sampling.unboardingPassengerCount(state.time, stationIndex, tram.passengerCount));
            entrances = ent;

            // Queue the expected tram departure
            int scheduledDepartTime = state.time + Sampling.passengerExchangeTime(pOut, pIn);
            eventQueue.Enqueue(new ExpectedTramDeparture(tram, stationIndex, scheduledDepartTime), scheduledDepartTime);
        }
    }
}
