using System;
using System.Collections.Generic;
using System.Text;

namespace ADS_Simulation.NS_State
{
    class TimeTable
    {
        int startAt;
        int interval;
        IEnumerable<int> timeTableEnumerator;

        public TimeTable(int startAt, int interval)
        {
            this.startAt = startAt;
            this.interval = interval;
            timeTableEnumerator = MakeEnumerator();
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
            return 0;
        }
    }
}
