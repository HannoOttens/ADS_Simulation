using System;
using System.Collections.Generic;
using System.Text;
using ADS_Simulation.NS_State;
using Priority_Queue;

namespace ADS_Simulation.Events
{
    class ExpectedArrivalEndstation : Event
    {
        Tram tram;
        Endstation station;

        public ExpectedArrivalEndstation(Tram tram, Endstation station)
        {
            this.tram = tram;
            this.station = station;
        }

        public override void Execute(State state, FastPriorityQueue<Event> eventQueue)
        {
            // Check if there is a free platform available and if switch lane is free
            // Cross is used for platform 1
            int goToPlatform = station.BestFreePlatform();
            if(goToPlatform > 0)
            {
                station.Occupy(tram, goToPlatform);
                eventQueue.Enqueue(new ArrivalEndstation(tram, station, goToPlatform), Configuration.Config.c.switchClearanceTime);
            }
            else
                station.incomingTrams.Enqueue(tram);
        }
    }
}
