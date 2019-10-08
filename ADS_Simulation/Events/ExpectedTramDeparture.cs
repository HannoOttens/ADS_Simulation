using ADS_Simulation.NS_State;
using Priority_Queue;

namespace ADS_Simulation.Events
{
    class ExpectedTramDeparture : Event
    {
        private readonly Tram tram;
        private readonly int stationIndex;

        public ExpectedTramDeparture(Tram tram, int stationIndex)
        {
            this.tram = tram;
            this.stationIndex = stationIndex;
        }

        public override void Execute(State state, FastPriorityQueue<Event> eventQueue)
        {
            var station = state.stations[stationIndex];
            System.Diagnostics.Debug.WriteLine($"ExpectedTramDeparture: tram {tram.id}, station: {station.name}, dir: {station.direction}");

            if (!tram.IsFull() && station.HasPassengers())
            {
                int pIn = station.BoardPassengers(tram);
                eventQueue.Enqueue(new ExpectedTramDeparture(tram, stationIndex), state.time + Sampling.passengerExchangeTime(0, pIn));
            }
            else
                eventQueue.Enqueue(new TramDeparture(tram, stationIndex), state.time);
        }
    }
}
