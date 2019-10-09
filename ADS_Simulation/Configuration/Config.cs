using Newtonsoft.Json;
using System;
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

            // Sort stations on index
            Array.Sort(c.transferTimes, (a, b) => a.index.CompareTo(b.index));

            // Read in the data & create dictionary to quickly index stations
            int maxIndex = c.endTime / 15;
            int direction = 0;
            Dictionary<(string, int), int> nameToIndex = new Dictionary<(string, int), int>();
            for (int i = 0; i < c.transferTimes.Length; i++)
            {
                c.transferTimes[i].averageExit = new double[maxIndex];
                c.transferTimes[i].standardDeviationExit = new double[maxIndex];
                c.transferTimes[i].arivalRate = new double[maxIndex];
                nameToIndex.Add((c.transferTimes[i].from, direction), i);

                // Swap direction halfway
                if (direction == 0 && i == c.transferTimes.Length / 2)
                {
                    direction = 1;  i--;
                }
            }

            // Parse in the CSVs
            bool header = true;
            using (var reader = new StreamReader(inPath))
                while (!reader.EndOfStream)
                {
                    string[] data = reader.ReadLine().Split(',');
                    if (header) { header = false; continue; } // Skip header

                    var idx = (data[0], int.Parse(data[1]));
                    var idxT = int.Parse(data[2]) / 15;
                    c.transferTimes[nameToIndex[idx]].averageExit[idxT] = double.Parse(data[3]); 
                    c.transferTimes[nameToIndex[idx]].standardDeviationExit[idxT] = double.Parse(data[4]);
                }
            header = true;
            using (var reader = new StreamReader(outPath))
                while (!reader.EndOfStream)
                {
                    string[] data = reader.ReadLine().Split(',');
                    if (header) { header = false; continue; } // Skip header

                    var idx = (data[0], int.Parse(data[1]));
                    var idxT = int.Parse(data[2]) / 15;
                    c.transferTimes[nameToIndex[idx]].arivalRate[idxT] = double.Parse(data[3]);
                }
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
        public string startStation;
        public string endStation;
        public StationData[] transferTimes;

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

        // Arrays for distrubutions, indexed on x-th 15 minute range
        public double[] averageExit;
        public double[] standardDeviationExit;
        public double[] arivalRate;
    }
}
