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

        public override void Execute(State state, StablePriorityQueue<Event> eventQueue)
        {
            var station = state.stations[stationIndex];
            station.Free();
            
            // Enqueue current train
            if (station.HasQueue())
            {
                Tram newOccupant = station.OccupyFromQueue();
                Console.WriteLine($"{newOccupant.id} arrived at {station.name}");
                eventQueue.Enqueue(new TramDeparture(newOccupant, stationIndex), state.time + Sampling.tramSafetyDistance() + Sampling.passengerExchangeTime(0,0));
            }

            Console.WriteLine($"Tram {tram.id} departed from {station.name}");

            // Let current train arrive at next station
            int newStationIndex = stationIndex + 1 >= state.stations.Count ? 0 : stationIndex + 1;

            //TODO:REMOVE
            if (newStationIndex == 0) return;

            eventQueue.Enqueue(new TramArrival(tram, newStationIndex), state.time + Sampling.drivingTime(100));
        }
    }
}
