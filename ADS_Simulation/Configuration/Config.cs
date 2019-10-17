using ADS_Simulation.NS_State;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

// All instantiated at start-of-app with JSON.
#nullable disable

namespace ADS_Simulation.Configuration
{
    static class Config
    {
        public static ConfigData c;

        public static void readConfig(string pathToConfig, string inPath, string outPath, string artPath, bool useArt)
        {
            string configStr = File.ReadAllText(pathToConfig);
            c = JsonConvert.DeserializeObject<ConfigData>(configStr);

            // Sort stations on index
            Array.Sort(c.transferTimes, (a, b) => a.index.CompareTo(b.index));

            // Read in the data & create dictionary to quickly index stations
            // Data contains entries with time at end of simulation
            int maxIndex = c.endTime / (15 * 60) + 1;
            int direction = 0;
            Dictionary<(string, int), int> nameToIndex = new Dictionary<(string, int), int>();
            for (int i = 0; i < c.transferTimes.Length; i++)
            {
                c.transferTimes[i].averageExit = new double[maxIndex];
                c.transferTimes[i].standardDeviationExit = new double[maxIndex];
                c.transferTimes[i].arrivalRate = new double[maxIndex];
                nameToIndex.Add((c.transferTimes[i].from, direction), i);

                // Swap direction halfway
                if (direction == 0 && i == c.transferTimes.Length / 2)
                {
                    direction = 1;
                    i--;
                }
            }

            // Parse in the CSVs
            bool header = true;

            if (useArt)
            {
                // Parse using artificial file
                using (var reader = new StreamReader(artPath))
                    while (!reader.EndOfStream)
                    {
                        string[] data = reader.ReadLine().Split(';');
                        if (header)
                        {
                            header = false;
                            continue;
                        } // Skip header

                        (string, int) idx;
                        if (data[0] == "P+R De Uithof")
                            idx = ("P+R De Uithof", 0);
                        else
                            idx = (data[0], int.Parse(data[1]));
                        var l = (int) (double.Parse(data[2], CultureInfo.InvariantCulture) * 4);
                        var u = (int) (double.Parse(data[3], CultureInfo.InvariantCulture) * 4);
                        var intervals = u - l;
                        for (int i = l; i < u; i++)
                        {
                            var arr = c.transferTimes[nameToIndex[idx]].arrivalRate[i];
                            var exx = c.transferTimes[nameToIndex[idx]].averageExit[i];
                            arr = Math.Max(arr, double.Parse(data[4], CultureInfo.InvariantCulture) / intervals);
                            exx = Math.Max(exx, double.Parse(data[5], CultureInfo.InvariantCulture) / intervals);
                            c.transferTimes[nameToIndex[idx]].arrivalRate[i] = arr;
                            c.transferTimes[nameToIndex[idx]].averageExit[i] = exx;
                        }
                    }
            }
            else
            {
                // Parse using in/out file
                using (var reader = new StreamReader(inPath))
                    while (!reader.EndOfStream)
                    {
                        string[] data = reader.ReadLine().Split(',');
                        if (header)
                        {
                            header = false;
                            continue;
                        } // Skip header

                        var idx = (data[0], int.Parse(data[1]));
                        var idxT = int.Parse(data[2]) / 15;
                        c.transferTimes[nameToIndex[idx]].arrivalRate[idxT] =
                            double.Parse(data[3], CultureInfo.InvariantCulture);
                    }

                header = true;
                using (var reader = new StreamReader(outPath))
                    while (!reader.EndOfStream)
                    {
                        string[] data = reader.ReadLine().Split(',');
                        if (header)
                        {
                            header = false;
                            continue;
                        } // Skip header

                        var idx = (data[0], int.Parse(data[1]));
                        var idxT = int.Parse(data[2]) / 15;
                        c.transferTimes[nameToIndex[idx]].averageExit[idxT] =
                            double.Parse(data[3], CultureInfo.InvariantCulture);
                        c.transferTimes[nameToIndex[idx]].standardDeviationExit[idxT] =
                            double.Parse(data[4], CultureInfo.InvariantCulture);
                    }
            }
        }
    }

    public class ConfigData
    {
        public int oneWayTripTimeMinutes;
        public int turnAroundTime;
        public int turnAroundTimeDualDriver;
        public int frequency;
        public int tramCapacity;
        public int stationClearanceTime;
        public int switchClearanceTime;
        public int startTime;
        public int endTime;
        public int maximumDelayBeforeDelayed;
        public int maximumWaitForExtraPassengers;
        public int numberOfTrams;
        public bool ucDualDriverSwitch;
        public float sdDrivingTimes;
        public string startStation;
        public string endStation;
        public StationData[] transferTimes;

        internal int GetIntervalSeconds()
        {
            return 3600 / frequency;
        }

        internal int RoundTripTime()
        {
            return 2 * turnAroundTime
                + 2 * 60 * oneWayTripTimeMinutes
                + 60;
        }

        internal int roundTripOffsetFor(string name)
        {
            if (name != endStation) return 0;
            return RoundTripTime() / 2;
        }

        internal int turnAroundTimeFor(Endstation station)
        {
            if (ucDualDriverSwitch && station.name == endStation)
                return turnAroundTimeDualDriver;
            return turnAroundTime;
        }
    }

    public class StationData
    {
        public string from;
        public string to;
        public float distance;
        public int averageTime;
        public int index;

        // Arrays for distributions, indexed on x-th 15 minute range
        public double[] averageExit;
        public double[] standardDeviationExit;
        public double[] arrivalRate;
    }
}
