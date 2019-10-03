using ADS_Simulation.Configuration;
using ADS_Simulation.Events;
using ADS_Simulation.NS_State;
using ADS_Simulation.Statistics;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADS_Simulation
{
    class Simulation
    {
        private const int MAX_EVENTS = 10000; // NOG GEEN IDEE OF DIT PAST

        State state;
        StablePriorityQueue<Event> eventQueue;
        List<Statistic> statistics;

        public Simulation()
        {
            List<Tram> trams = CreateTrams(Config.c.frequency);
            List<Station> stations = CreateStations();
            List<Tram> trams = CreateTrams();
            List<Station> stations = CreateStations();
            state = new State(0, trams, stations);
            eventQueue = InitializeEventQueue();
            statistics = new List<Statistic>()
            {
                //new PassengerWaitStatistic(),
                //new TramLoadStatistic(),
                //new EmptyStationStatistic()
            };
        }

        private StablePriorityQueue<Event> InitializeEventQueue()
        {
            var queue = new StablePriorityQueue<Event>(MAX_EVENTS);
            int interval = 60 / Config.c.frequency;

            // Make an arrival event for every tram
            for (int i = 0; i < state.trams.Count; i++)
                queue.Enqueue(new TramArrival(state.trams[i], 0), i * interval);

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
        /// Create stations as declared in config
        /// </summary>
        /// <returns>List of stations</returns>
        public List<Station> CreateStations()
        {
            var stations = new List<Station>{new Endstation(Config.c.stations[0])}; // First element is endstation
            for (int i = 1; i < Config.c.stations.Length - 1; i++) // All stations except first and last
                stations.Add(new Station(Config.c.stations[i]));
            stations.Add(new Endstation(Config.c.stations[Config.c.stations.Length - 1])); // Last element is endstation
            return stations;
        }

        /// <summary>
        /// Make one step in the simulation
        /// </summary>
        /// <returns>If simulation has ended</returns>
        public bool Step()
        {
            // Get next event and execute
            Event _event = eventQueue.Dequeue();
            // Advance clock (mss deel maken van de event.Execute?)
            state.time = (int)_event.Priority;
            _event.Execute(state, eventQueue);

            // Measure statistics
            foreach (var statistic in statistics)
                statistic.measure(state);

            return StoppingConditionMet();
        }

        /// <summary>
        /// Check if the stopping condition of simulation has been met
        /// </summary>
        private bool StoppingConditionMet()
        {
            return eventQueue.Count == 0
                || state.time >= Config.c.endTime;
        }
    }
}
