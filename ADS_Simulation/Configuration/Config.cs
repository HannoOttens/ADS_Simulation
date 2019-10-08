using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

// All instantiated at start-of-app with JSON.
#nullable disable

namespace ADS_Simulation.Configuration
{
    static class Config
    {
        public static ConfigData c;
        public static List<string[]> indata = new List<string[]>();
        public static List<string[]> outdata = new List<string[]>();

        public static void readConfig(string pathToConfig, string inPath, string outPath)
        {
            string configStr = File.ReadAllText(pathToConfig);
            c = JsonConvert.DeserializeObject<ConfigData>(configStr);
            using (var reader = new StreamReader(inPath))
                while (!reader.EndOfStream)
                    indata.Add(reader.ReadLine().Split(','));
            using (var reader = new StreamReader(outPath))
                while (!reader.EndOfStream)
                    outdata.Add(reader.ReadLine().Split(','));
        }
    }

    public class ConfigData
    {
        public int oneWayTripTimeMinutes;
        public int turnAroundTimeMinutes;
        public int frequency;
        public int tramCapacity;
        public int stationClearanceTime;
        public int switchClearanceTime;
        public int startTime;
        public int endTime;
        public bool ucDualDriverSwitch;
        public float sdDrivingTimes;
        public string[] stations;
        public string startStation;
        public string endStation;
        public List<StationData> transferTimes;

        internal int GetIntervalSeconds()
        {
            return 3600 / frequency;
        }
    }

    public class StationData
    {
        public string from;
        public string to;
        public float distance;
        public int averageTime;
        public int index;
    }
}
