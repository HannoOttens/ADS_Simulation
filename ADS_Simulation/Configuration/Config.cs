using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// All instatiated at start-of-app with JSON.
#nullable disable

namespace ADS_Simulation.Configuration
{
    static class Config
    {
        public static ConfigData c;

        public static void readConfig(string pathToConfig)
        {
            string configStr = File.ReadAllText(pathToConfig);
            c = JsonConvert.DeserializeObject<ConfigData>(configStr);
        }
    }

    public class ConfigData
    {
        public int oneWayTripTimeMinutes;
        public int turnAroundTimeMinutes;
        public int frequency;
        public int tramCapcity;
        public int stationClearanceTime;
        public int switchClearanceTime;
        public int startTime;
        public int endTime;
        public bool ucDualDriverSwitch;

        //TODO:Mss hier een Dictionary<(from,to),StationData> van maken?
        public List<StationData> transferTimes;
    }

    public class StationData
    {
        public string from;
        public string to;
        public float distance;
        public int averageTime;
    }
}
