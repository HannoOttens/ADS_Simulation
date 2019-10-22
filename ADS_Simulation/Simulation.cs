using ADS_Simulation.Configuration;
using ADS_Simulation.Events;
using ADS_Simulation.NS_State;
using ADS_Simulation.Statistics;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ADS_Simulation
{
    class Simulation
    {
        private const int MAX_EVENTS = 1000000;

        public State state;
        public FastPriorityQueue<Event> eventQueue;
        public StatisticsManager statisticsManager;

        public Simulation()
        {
            List<Station> stations = CreateStations();
            List<Tram> trams = CreateTrams();
            state = new State(trams, stations);
            eventQueue = InitializeEventQueue();

            // Set the start time to the first event
            state.time = (int)eventQueue.First.Priority;

            statisticsManager = new StatisticsManager(state, new (int, int)[] { 
                // Whole simulation
                (Config.c.startTime,  int.MaxValue),
                // 7:30 - 9:30
                (75*6*60,  95*6*60),
                // 9:30 - 16:00
                (95*6*60,  16*60*60),
                // 16:00 - 18:00
                (16*60*60,  18*60*60),
                // 18:00 - End
                (18*60*60,  int.MaxValue),
                // 07:00 - 19:00
                (07*60*60,  19*60*60)
            });
        }

        private FastPriorityQueue<Event> InitializeEventQueue()
        {
            var queue = new FastPriorityQueue<Event>(MAX_EVENTS);

            // Make an arrival event for every tram
            var startStation = (Endstation)state.stations[0];

            for (int i = 0; i < state.trams.Count; i++)
                startStation.depotQueue.Enqueue(state.trams[i]);
            startStation.OccupyFromDepotQueue(Platform.A);
            startStation.OccupyFromDepotQueue(Platform.B);
            queue.Enqueue(new ArrivalEndstation(state.trams[0], startStation, Platform.A, true), state.time);
            queue.Enqueue(new ArrivalEndstation(state.trams[1], startStation, Platform.B, true), state.time);

            // Create passenger arrivals
            for (int i = 0; i < state.stations.Count; i++)
                for (int j = 0; j < Config.c.transferTimes[i].arrivalRate.Length; j++)
                {
                    if (Config.c.transferTimes[i].arrivalRate[j] == 0)
                        continue; // No arrival in this window

                    var times = Sampling.arrivingPassengers(Config.c.transferTimes[i].arrivalRate[j] * Config.c.passengerMultiplier);
                    foreach (var time in times)
                        queue.Enqueue(new PassengerArrival(i), j * 900 + time);
                }

            return queue;
        }

        /// <summary>
        /// Calculates the number of trams necessary based on the frequency
        /// </summary>
        /// <returns>List of trams</returns>
        public List<Tram> CreateTrams()
        {
            var trams = new List<Tram>();
            for (int i = 1; i <= Config.c.numberOfTrams; i++)
                trams.Add(new Tram(6000 + i));
            return trams;
        }

        /// <summary>
        /// Create the list of stations (P+R to UC and back) with direction A for P+R to UC
        /// </summary>
        /// <returns>The list of stations</returns>
        private List<Station> CreateStations()
        {
            var stations = new List<Station>();

            // Start in direction A
            var direction = Direction.A;
            for (var i = 0; i < Config.c.transferTimes.Length; i++)
            {
                StationData stationData = Config.c.transferTimes[i];
                bool isEndStation = stationData.@from == Config.c.startStation
                                    || stationData.@from == Config.c.endStation;

                if (isEndStation)
                {
                    bool hasDepot = stationData.@from == Config.c.startStation;
                    stations.Add(new Endstation(stationData.@from, hasDepot, i));
                }
                else
                    stations.Add(new Station(stationData.@from, direction));

                // Change direction at endstation
                if (stationData.@from == Config.c.endStation)
                    direction = Direction.B;
            }

            return stations;
        }

        /// <summary>
        /// Make one step in the simulation
        /// </summary>
        /// <returns>False when simulation has ended</returns>
        public bool Step()
        {
            // Get next event and execute
            Event _event = eventQueue.Dequeue();

            // Advance clock and execute
            Trace.Assert(state.time <= (int)_event.Priority, "We traveled back in time - that's not that great - time travel really isn't great - please stop");
            state.time = (int)_event.Priority;
            _event.Execute(state, eventQueue);

            // Measure statistics
            statisticsManager.measureStatistics(state, _event);

            return !StoppingConditionMet();
        }

        /// <summary>
        /// Check if the stopping condition of simulation has been met
        /// </summary>
        private bool StoppingConditionMet()
        {
            return eventQueue.Count == 0;
        }
    }
}
