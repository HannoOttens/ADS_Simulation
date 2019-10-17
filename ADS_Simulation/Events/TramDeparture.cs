using ADS_Simulation.Configuration;
using ADS_Simulation.NS_State;
using Priority_Queue;
using System;
using System.Diagnostics;

namespace ADS_Simulation.Events
{
    class TramDeparture : Event
    {
        private readonly Tram tram;
        private readonly int stationIndex;

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
                eventQueue.Enqueue(new TramArrival(newOccupant, stationIndex), state.time + Sampling.tramSafetyDistance());
            }

            // Schedule arrival at next station
            int newStationIndex = stationIndex + 1 == state.stations.Count ? 0 : stationIndex + 1;
            int drivingTime = Sampling.drivingTime(Config.c.transferTimes[stationIndex].averageTime);
            Debug.WriteLine($"TramDeparture: tram {tram.id}: {station.name}, dir: {station.direction}, time: {state.time}");

            // Make sure trams do not take over eachother
            int arrivalTime = station.SignalNextArrival(state.time + drivingTime);
            if (state.stations[newStationIndex] is Endstation endstation)
                eventQueue.Enqueue(new ExpectedArrivalEndstation(tram, endstation), arrivalTime);
            else
                eventQueue.Enqueue(new ExpectedTramArrival(tram, newStationIndex), arrivalTime);
        }
    }
}
