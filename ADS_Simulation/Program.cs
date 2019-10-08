using ADS_Simulation.Configuration;
using ADS_Simulation.Events;
using ADS_Simulation.NS_State;
using System;
using System.Diagnostics;
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
            int eventCount = 0;
            stopwatch.Start();

            MarshallArgs(args);

            // Initialize config
            Config.readConfig(configPath);

            // Initialize simulation
            Simulation simulation = new Simulation();
            while (simulation.Step())
            {
                if (gui)
                    DrawGUI(simulation);

                // Addition between-step functionality
                eventCount++;

                if (step)
                    Console.ReadKey();
            }

            Console.WriteLine(@$"Went through {eventCount} events.
The situation ended at {simulation.state.time} and should end at {Config.c.endTime}.
The simulation took {(stopwatch.ElapsedMilliseconds/1000f).ToString("n2")}s
================================");

            // Print statistic output
            foreach (var s in simulation.statistics)
                s.Print(simulation.state);
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

                Console.Write(new string(' ', dotDist / 2));
                for (int i = 0; i * 2 < state.stations.Count; i++)
                    Console.Write("O" + new string('-', dotDist - 1));
                Console.WriteLine("O    ");

                if (!nameAbove)
                    DrawStationNames();
                else
                    DrawOccupants(false);
            }

            void DrawStationNames()
            {
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

                    Console.Write(outt);
                }
                Console.WriteLine();
            }

            void DrawOccupants(bool above)
            {
                if (above)
                {
                    DrawTransitTrams(above);
                    DrawStationQueues(above);
                }

                Console.ForegroundColor = ConsoleColor.Green;
                if (above)
                    for (int i = 0; i <= state.stations.Count / 2; i++)
                        DrawOccupant(i, above);
                else
                {
                    DrawOccupant(0, above);
                    for (int i = state.stations.Count - 1; i >= state.stations.Count / 2; i--)
                        DrawOccupant(i, above);
                }
                Console.ResetColor();
                Console.WriteLine();

                if (!above)
                {
                    DrawStationQueues(above);
                    DrawTransitTrams(above);
                }
            }

            void DrawOccupant(int i, bool above)
            {
                Tram? occupant = state.stations[i].occupant;
                if (state.stations[i] is Endstation endstation)
                    if (i == 0 && !above) // 2e occupant P+R
                        occupant = endstation.occupant2;
                    else if (i != 0 && above) // 2e occupant UC
                        occupant = endstation.occupant2;

                if (occupant != null)
                    DrawTrainId(occupant.id);
                else Console.Write(new string(' ', dotDist));
            }

            void DrawTrainId(int id)
            {
                string occupantName = id.ToString();
                string emptySpace = new string(' ', (dotDist - occupantName.Length) / 2);
                string outt = emptySpace + occupantName + emptySpace;
                if (outt.Length == dotDist - 1)
                    outt = ' ' + outt;
                Console.Write(outt);
            }

            void DrawStationQueues(bool above)
            {
                int biggestQueue = state.stations.Max(v => v.incomingTrams.Count);
                Console.ForegroundColor = ConsoleColor.Yellow;

                if (above)
                {
                    for (int q = biggestQueue - 1; q >= 0; q--)
                    {
                        for (int i = 0; i < state.stations.Count / 2; i++)
                            if (q < state.stations[i].incomingTrams.Count)
                                DrawTrainId(state.stations[i].incomingTrams.ToList()[q].id);
                            else
                                Console.Write(new string(' ', dotDist));
                        Console.WriteLine();
                    }
                }
                else
                    for (int q = 0; q < biggestQueue; q++)
                    {
                        Console.Write(new string(' ', dotDist));
                        for (int i = state.stations.Count - 1; i >= state.stations.Count / 2; i--)
                            if (q < state.stations[i].incomingTrams.Count)
                                DrawTrainId(state.stations[i].incomingTrams.ToList()[q].id);
                            else
                                Console.Write(new string(' ', dotDist));
                        Console.WriteLine();
                    }

                Console.ResetColor();
            }

            void DrawTransitTrams(bool above)
            {
                // Get correct events
                var groupedTransits = simulation.eventQueue.OfType<ExpectedTramArrival>()
                    .GroupBy(e => e.stationIndex);
                var endTransits = simulation.eventQueue.OfType<ExpectedArrivalEndstation>()
                    .GroupBy(e => e.station.name);
                int maxTransits = Math.Max(
                    groupedTransits.Select(g => g.Count()).DefaultIfEmpty(0).Max(), 
                    endTransits.Select(g => g.Count()).DefaultIfEmpty(0).Max());

                Console.ForegroundColor = ConsoleColor.Cyan;

                if (above)
                    for (int q = maxTransits - 1; q >= 0; q--)
                    {
                        Console.Write(new string(' ', dotDist/2));

                        // Middle stations
                        for (int i = 1; i < state.stations.Count / 2; i++)
                        {
                            var ts = groupedTransits.SingleOrDefault(k => k.Key == i);
                            if (q < ts?.Count())
                                DrawTrainId(ts.ToList()[q].tram.id);
                            else
                                Console.Write(new string(' ', dotDist));
                        }

                        // Utrecht Centraal
                        var tes = endTransits.SingleOrDefault(k => k.Key == Config.c.endStation);
                        if (q < tes?.Count())
                            DrawTrainId(tes.ToList()[q].tram.id);
                        else
                            Console.Write(new string(' ', dotDist));

                        Console.WriteLine();
                    }
                else
                    for (int q = 0; q < maxTransits; q++)
                    {
                        Console.Write(new string(' ', dotDist / 2));

                        // P+R
                        var tes = endTransits.SingleOrDefault(k => k.Key == Config.c.startStation);
                        if (q < tes?.Count())
                            DrawTrainId(tes.ToList()[q].tram.id);
                        else
                            Console.Write(new string(' ', dotDist));

                        // Middle stations
                        Console.Write(new string(' ', dotDist));
                        for (int i = state.stations.Count - 1; i >= state.stations.Count / 2; i--)
                        {
                            var ts = groupedTransits.SingleOrDefault(k => k.Key == i);
                            if (q < ts?.Count())
                                DrawTrainId(ts.ToList()[q].tram.id);
                            else
                                Console.Write(new string(' ', dotDist));
                        }

                        Console.WriteLine();
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
                            configPath = args[i++];
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
