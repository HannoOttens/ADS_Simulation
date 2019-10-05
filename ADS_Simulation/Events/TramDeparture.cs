﻿using System;
using System.Collections.Generic;
using System.Text;
using ADS_Simulation.NS_State;
using Priority_Queue;

namespace ADS_Simulation.Events
{
    class TramDeparture : Event
    {
        Tram tram;
        int stationIndex;

        public TramDeparture(Tram tram, int stationIndex)
        {
            this.tram = tram;
            this.stationIndex = stationIndex;
        }

        public override void Execute(State state, FastPriorityQueue<Event> eventQueue)
        {
            var station = state.stations[stationIndex];
            station.Free();

            // Enqueue current train
            if (station.HasQueue())
            {
                Tram newOccupant = station.OccupyFromQueue();
                eventQueue.Enqueue(new ExpectedTramDeparture(newOccupant, stationIndex), state.time + Sampling.tramSafetyDistance() + Sampling.passengerExchangeTime(0, 0));
            }

            int newStationIndex = stationIndex + 1 == state.stations.Count ? 0 : stationIndex + 1;
            if (state.stations[newStationIndex] is Endstation endstation)
                eventQueue.Enqueue(new ExpectedArrivalEndstation(tram, endstation), state.time + Sampling.drivingTime(100)); //TODO: Fix driving time
            else
                eventQueue.Enqueue(new ExpectedTramArrival(tram, newStationIndex), state.time + Sampling.drivingTime(100)); //TODO: Fix driving time
        }
    }
}
