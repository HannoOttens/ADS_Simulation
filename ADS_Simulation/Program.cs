using ADS_Simulation.Configuration;
using ADS_Simulation.Events;
using ADS_Simulation.NS_State;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ADS_Simulation
{
    class Program
    {
        static readonly Stopwatch stopwatch = new Stopwatch();

        static string configPath = "../../../config.json";

        static bool step = false;
        static bool gui = false;

        static void Main(string[] args)
        {

            MarshallArgs(args);

            // Initialize config
            Config.readConfig(configPath);

            int[] tramFrequencies = new int[] { 14, 16, 18, 20, 22 };
            int[] tramNumbers = new int[] { 12, 14, 16, 18, 20 };
            bool[] ucDriverSwitch = new bool[] { true, false };
            int[] turnAroundTimes = new int[] { 60, 120, 150, 180, 210, 240, 300 };
            foreach (int frequency in tramFrequencies)
                foreach (bool ucSwitch in ucDriverSwitch)
                    foreach (int turnAroundTime in turnAroundTimes)
                    {
                        Config.c.ucDualDriverSwitch = ucSwitch;
                        Config.c.frequency = frequency;
                        Config.c.turnAroundTime = turnAroundTime;

                        // Caclulate number of trams programiicly
                        Config.c.numberOfTrams = (int)Math.Ceiling((decimal)Config.c.RoundTripTime() / Config.c.GetIntervalSeconds());

                        Config.c.outputFileName = $"f={frequency}-n={Config.c.numberOfTrams}-uc={ucSwitch}-q={turnAroundTime}";

                        Console.WriteLine("====================================================");
                        Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++");
                        Console.WriteLine($"Run: {Config.c.outputFileName}");
                        Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++");
                        Console.WriteLine("====================================================");

                        Run();
                    }
        }

        private static void Run()
        {
            int eventCount = 0;

            // Bookkeep data
            var statisticalData = new Dictionary<(int, int), List<string>>();

            // Run the number of simulations
            for (int runNo = 0; runNo < Config.c.runs; runNo++)
            {
                stopwatch.Restart();

                // Initialize simulation
                (int events, Simulation simulation) = Simulate();

                // Add headers on first run
                if (runNo == 0)
                    foreach (var timeRange in simulation.statisticsManager.timeRanges)
                        statisticalData.Add(timeRange, new List<string> { simulation.statisticsManager.GetSimulationResultHeaders() });

                // Add data
                foreach (var timeRange in simulation.statisticsManager.timeRanges)
                    statisticalData[timeRange].Add(simulation.statisticsManager.GetSimulationResultForRange(simulation.state, timeRange));

                // Print run result
                Console.WriteLine(@$"Run: {runNo}
Went through {eventCount} events.
The situation ended at {simulation.state.time} and should end at {Config.c.endTime}.
The simulation took {(stopwatch.ElapsedMilliseconds / 1000f).ToString("n2")}s
================================");
            }

            // Save result
            foreach (var timeRange in statisticalData.Keys)
            {
                List<string> results = statisticalData[timeRange];
                string output = String.Join('\n', results);

                string path = Path.Combine(Config.c.outputFilePath, Config.c.outputFileName);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string timerangename = timeRange.ToString().Replace("(", "").Replace(")", "").Replace(", ", "_");
                string filepath = Path.Combine(path, $"{timerangename}_{Config.c.outputFileName}.csv");

                File.WriteAllText(filepath, output);
            }
        }

        private static (int events, Simulation simulation) Simulate()
        {
            int eventCount = 0;
            Simulation simulation = new Simulation();
            while (simulation.Step())
            {
                if (gui)
                    DrawGUI(simulation);

                // Addition between-step functionality
                eventCount++;

                //// First cannot be None if there is a tram at the platform
                //Debug.Assert(simulation.state.stations.OfType<Endstation>()
                //        .All((s) => s.first == Platform.None && s.IsFree(Platform.A) && s.IsFree(Platform.B)
                //        || s.first == Platform.A && !s.IsFree(Platform.A)
                //        || s.first == Platform.B && !s.IsFree(Platform.B)), "Tram cannot depart");

                ////Only trams with upfollowing ids can be on endstation at same time
                //Debug.Assert(simulation.state.stations.OfType<Endstation>()
                //        .All((s) => Abs(s.occupant?.id - s.occupant2?.id) ?? true), "Trams cannot overtake each other");

                if (step)
                    Console.ReadKey();
            }

            return (eventCount, simulation);
        }

        private static bool? Abs(int? v)
        {
            if (v != null)
            {
                int diff = Math.Abs(v.Value);
                return diff == 1 || diff == Config.c.numberOfTrams - 1;
            }
            return null;
        }

        /// <summary>
        /// Code to draw the GUI (don't look at the code, look at the GUI, the GUI is the part that actually looks alright)
        /// </summary>
        /// <param name="simulation">Current simulation</param>
        static void DrawGUI(Simulation simulation)
        {
            int dotDist = 15;
            State state = simulation.state;

            // Basic stats
            Console.WriteLine(new String('~', 150));
            Console.WriteLine($"Time: {simulation.state.time}");
            Console.WriteLine();

            DrawStationLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            DrawStationLine(true);
            Console.WriteLine();

            void DrawStationLine(bool nameAbove = false)
            {
                if (nameAbove)
                    DrawStationNames();
                else
                    DrawOccupants(true);

                string toOut = new string(' ', dotDist / 2);
                for (int i = 0; i * 2 < state.stations.Count; i++)
                    toOut += "O" + new string('-', dotDist - 1);
                toOut += "O    ";
                Console.WriteLine(toOut);

                if (!nameAbove)
                    DrawStationNames();
                else
                    DrawOccupants(false);
            }

            void DrawStationNames()
            {
                string toOut = "";
                for (int i = 0; i * 2 <= state.stations.Count; i++)
                {
                    string name = state.stations[i].name;
                    // Limit name length
                    if (name.Length > 10)
                        name = name.Substring(0, 10);

                    // Calculate space between names
                    string emptySpace = new string(' ', (dotDist - name.Length) / 2);
                    string outt = emptySpace + name + emptySpace;

                    // Account for rounding error
                    if (outt.Length == dotDist - 1)
                        outt = ' ' + outt;

                    toOut += outt;
                }
                Console.WriteLine(toOut);
            }

            void DrawOccupants(bool above)
            {
                if (above)
                {
                    DrawTransitTrams(above);
                    DrawStationQueues(above);
                }

                string toOut = "";
                if (above)
                    for (int i = 0; i <= state.stations.Count / 2; i++)
                        toOut += OccupantString(i, above);
                else
                {
                    toOut += OccupantString(0, above);
                    for (int i = state.stations.Count - 1; i >= state.stations.Count / 2; i--)
                        toOut += OccupantString(i, above);
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(toOut);
                Console.ResetColor();

                if (!above)
                {
                    DrawStationQueues(above);
                    DrawTransitTrams(above);
                }
            }

            string OccupantString(int i, bool above)
            {
                Tram? occupant = state.stations[i].occupant;
                if (state.stations[i] is Endstation endstation)
                    if (i == 0 && !above) // 2e occupant P+R
                        occupant = endstation.occupant2;
                    else if (i != 0 && above) // 2e occupant UC
                        occupant = endstation.occupant2;

                if (occupant != null)
                    return DrawTrainId(occupant.id);
                else return new string(' ', dotDist);
            }

            string DrawTrainId(int id)
            {
                string occupantName = id.ToString();
                string emptySpace = new string(' ', (dotDist - occupantName.Length) / 2);
                string outt = emptySpace + occupantName + emptySpace;
                if (outt.Length == dotDist - 1)
                    outt = ' ' + outt;
                return outt;
            }

            void DrawStationQueues(bool above)
            {
                int biggestQueue = state.stations.Max(v => v.incomingTrams.Count);

                string toOut = "";
                if (above)
                {
                    for (int q = biggestQueue - 1; q >= 0; q--)
                    {
                        for (int i = 0; i < state.stations.Count / 2; i++)
                            if (q < state.stations[i].incomingTrams.Count)
                                toOut += DrawTrainId(state.stations[i].incomingTrams.ToList()[q].id);
                            else
                                toOut += new string(' ', dotDist);
                        toOut += "\n";
                    }
                }
                else
                    for (int q = 0; q < biggestQueue; q++)
                    {
                        toOut += new string(' ', dotDist);
                        for (int i = state.stations.Count - 1; i >= state.stations.Count / 2; i--)
                            if (q < state.stations[i].incomingTrams.Count)
                                toOut += DrawTrainId(state.stations[i].incomingTrams.ToList()[q].id);
                            else
                                toOut += new string(' ', dotDist);
                        toOut += "\n";
                    }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(toOut);
                Console.ResetColor();
            }

            void DrawTransitTrams(bool above)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;

                // Get correct events
                var groupedTransits = simulation.eventQueue.OfType<ExpectedTramArrival>()
                    .GroupBy(e => e.stationIndex);
                var endTransits = simulation.eventQueue.OfType<ExpectedArrivalEndstation>()
                    .GroupBy(e => e.station.name);
                int maxTransits = Math.Max(
                    groupedTransits.Select(g => g.Count()).DefaultIfEmpty(0).Max(),
                    endTransits.Select(g => g.Count()).DefaultIfEmpty(0).Max());

                if (above)
                    for (int q = maxTransits - 1; q >= 0; q--)
                    {
                        string toOut = new string(' ', dotDist / 2);

                        // Middle stations
                        for (int i = 1; i < state.stations.Count / 2; i++)
                        {
                            var ts = groupedTransits.SingleOrDefault(k => k.Key == i);
                            if (q < ts?.Count())
                                toOut += DrawTrainId(ts.ToList()[q].tram.id);
                            else
                                toOut += new string(' ', dotDist);
                        }

                        // Utrecht Centraal
                        var tes = endTransits.SingleOrDefault(k => k.Key == Config.c.endStation);
                        if (q < tes?.Count())
                            toOut += DrawTrainId(tes.ToList()[q].tram.id);
                        else
                            toOut += new string(' ', dotDist);

                        Console.WriteLine(toOut);
                    }
                else
                    for (int q = 0; q < maxTransits; q++)
                    {
                        string toOut = new string(' ', dotDist / 2);

                        // P+R
                        var tes = endTransits.SingleOrDefault(k => k.Key == Config.c.startStation);
                        if (q < tes?.Count())
                            toOut += DrawTrainId(tes.ToList()[q].tram.id);
                        else
                            toOut += new string(' ', dotDist);

                        // Middle stations
                        for (int i = state.stations.Count - 1; i >= state.stations.Count / 2; i--)
                        {
                            var ts = groupedTransits.SingleOrDefault(k => k.Key == i);
                            if (q < ts?.Count())
                                toOut += DrawTrainId(ts.ToList()[q].tram.id);
                            else
                                toOut += new string(' ', dotDist);
                        }

                        Console.WriteLine(toOut);
                    }

                Console.ResetColor();
            }
        }

        #region Console argument parsing
        /// <summary>
        /// Marshall the program arguments
        /// </summary>
        /// <param name="args">The program arguments</param>
        static bool MarshallArgs(string[] args)
        {
            int i = 0;
            try
            {
                for (i = 0; i < args.Length; i++)
                    switch (args[i])
                    {
                        case "-c":
                        case "-config":
                            configPath = args[++i];
                            break;
                        case "-s":
                        case "-step":
                            step = true;
                            break;
                        case "-gui":
                            gui = true;
                            break;
                        case "-h":
                        case "-help":
                            PrintHelp();
                            return false;
                        default:
                            throw new Exception($"Argument {i} not recognised: {args[i]}");
                    }

                return true;
            }
            catch
            {
                throw new Exception($"Failed to parse argument {i}: {args[i]}");
            }
        }

        /// <summary>
        /// Print help
        /// </summary>
        static void PrintHelp()
        {
            System.Diagnostics.Debug.WriteLine(@"
===========================================[HELP]============================================

    HELP ME!

+====================================+===================+====================================+
| Command              Parameters    | Default           | Detail                             |
+====================================+===================+====================================+
  -c, -config          <path>        | ../../config.json | Defines the config path
  -s, -step                          | false             | Per-event stepping
  -gui                               | false             | Show GUI
+====================================+===================+====================================+
");
        }
        #endregion
    }
}
