﻿using System.Collections.Generic;

namespace ADS_Simulation.NS_State
{
    class TimeTable
    {
        readonly int startAt;
        readonly int interval;
        readonly IEnumerator<int> timeTableEnumerator;

        public TimeTable(int startAt, int interval)
        {
            this.startAt = startAt;
            this.interval = interval;
            timeTableEnumerator = MakeEnumerator().GetEnumerator();
            // Init value
            timeTableEnumerator.MoveNext();
        }

        private IEnumerable<int> MakeEnumerator()
        {
            int t = startAt;
            while(true)
            {
                yield return t;
                t += interval;
            }
        }

        public int Next()
        {
            int t = timeTableEnumerator.Current;
            timeTableEnumerator.MoveNext();
            return t;
        }

        public int Current()
        {
            return timeTableEnumerator.Current;
        }
    }
}
