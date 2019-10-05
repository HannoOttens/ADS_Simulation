using ADS_Simulation.Configuration;
using ADS_Simulation.Events;
using ADS_Simulation.NS_State;
using ADS_Simulation.Statistics;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ADS_Simulation
{
    class Simulation
    {
        private const int MAX_EVENTS = 10000; // NOG GEEN IDEE OF DIT PAST

        public State state;
        public FastPriorityQueue<Event> eventQueue;
        public List<Statistic> statistics;

        public Simulation()
        {
            List<Station> stations = CreateStations();
            List<Tram> trams = CreateTrams();
            state = new State(0, trams, stations);
            eventQueue = InitializeEventQueue();
            statistics = new List<Statistic>()
            {
                //new PassengerWaitStatistic(),
                //new TramLoadStatistic(),
                new EmptyStationStatistic(stations.Count)
            };
        }

        private FastPriorityQueue<Event> InitializeEventQueue()
        {
            var queue = new FastPriorityQueue<Event>(MAX_EVENTS);
            int interval = 60 / Config.c.frequency;

            // Make an arrival event for every tram
            for (int i = 0; i < state.trams.Count; i++)
                queue.Enqueue(new ExpectedTramArrival(state.trams[i], 0), i * interval * 60);

            return queue;
        }

        /// <summary>
        /// Calculates the number of trams necessary based on the frequency
        /// </summary>
        /// <returns>List of trams</returns>
        public List<Tram> CreateTrams()
        {
            // Interval at which trains leave the station
            float interval = 60 / Config.c.frequency;

            // Calculate the amount needed to fill a 44 minute cycle 
            // and the first tram is scheduled to leave P+R again
            int roundTripTime = 2 * (Config.c.oneWayTripTimeMinutes + Config.c.turnAroundTimeMinutes);
            int numberOfTrains = (int)(roundTripTime / interval);

            var trams = new List<Tram>();
            for (int i = 1; i <= numberOfTrains; i++)
                trams.Add(new Tram(6000 + i));
            return trams;
        }

        /// <summary>
        /// Create the list of stations (P+R to UC and back) with direction A for P+R to UC
        /// </summary>
        /// <returns>The list of stations</returns>
        private List<Station> CreateStations()
        {
            // Sort stations on index
            Config.c.transferTimes.Sort((a, b) => a.index.CompareTo(b.index));

            var stations = new List<Station>();

            // Start in direction A
            var direction = Direction.A;
            foreach (StationData stationData in Config.c.transferTimes)
            {
                bool isEndStation = stationData.from == Config.c.startStation
                    || stationData.from == Config.c.endStation;

                if (isEndStation)
                {
                    bool hasDepot = direction == Direction.A;
                    stations.Add(new Endstation(stationData.from, hasDepot));
                }
                else
                    stations.Add(new Station(stationData.from, direction));

                // Change direction at endstation
                if (stationData.from == Config.c.endStation)
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
            Debug.Assert(state.time <= (int)_event.Priority, "We traveled back in time - that's not that great - time travel really isn't great - please stop");
            state.time = (int)_event.Priority;
            _event.Execute(state, eventQueue);

            // Measure statistics
            foreach (var statistic in statistics)
                statistic.measure(state);

            return !StoppingConditionMet();
        }

        /// <summary>
        /// Check if the stopping condition of simulation has been met
        /// </summary>
        private bool StoppingConditionMet()
        {
            return eventQueue.Count == 0
                && state.time >= Config.c.endTime;
        }
    }
}
