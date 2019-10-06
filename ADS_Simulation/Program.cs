using ADS_Simulation.Configuration;
using ADS_Simulation.NS_State;
using ADS_Simulation.Statistics;
using System;
using System.Diagnostics;
using System.Threading;

namespace ADS_Simulation
{
    class Program
    {
        static string configPath = "../../../config.json";
        static bool step = false;
        static bool gui = true;

        static void Main(string[] args)
        {
            args = new string[] { "-s" };
            MarshallArgs(args);

            // Initialize config
            Config.readConfig(configPath);

            // Initialize simulation
            Simulation simulation = new Simulation();
            while (simulation.Step())
            {
                if (step)
                    Console.ReadKey();

                if (gui)
                    DrawGUI(simulation);

                // Graphical representation or addition between-step fuctionality
            }

            // Print statistic output
            foreach (var s in simulation.statistics)
                s.Print(simulation.state);
        }

        static void DrawGUI(Simulation simulation)
        {
            int dotDist = 15;
            State state = simulation.state;

            // Basic stats
            Console.WriteLine($"Time: {simulation.state.time}");
            Console.WriteLine();

            DrawStationLine();
            Console.WriteLine();
            Console.WriteLine();
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
                    DrawOccupants(Direction.A);

                Console.Write(new string(' ', dotDist / 2));
                for (int i = 0; i * 2 < state.stations.Count; i++)
                    Console.Write("O" + new string('-', dotDist - 1));
                Console.WriteLine("O    ");

                if (!nameAbove)
                    DrawStationNames();
                else
                    DrawOccupants(Direction.B);
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

            void DrawOccupants(Direction direction)
            {
                if (direction == Direction.A)
                    for (int i = 0; i <= state.stations.Count / 2; i++)
                    {
                        DrawOccupant(i);
                    }
                else
                    for (int i = state.stations.Count - 1; i >= state.stations.Count / 2; i--)
                    {
                        DrawOccupant(i);
                    }
                Console.WriteLine();
            }

            void DrawOccupant(int i)
            {
                Tram? occupant = state.stations[i].occupant;
                if (occupant != null)
                {
                    string occupantName = occupant.id.ToString();
                    string emptySpace = new string(' ', (dotDist - occupantName.Length) / 2);
                    string outt = emptySpace + occupantName + emptySpace;
                    if (outt.Length == dotDist - 1)
                        outt = ' ' + outt;
                    Console.Write(outt);
                }
                else Console.Write(new string(' ', dotDist));
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
  -c, -config          <path>        | ../../config.json |
+====================================+===================+====================================+
");
        }
        #endregion
    }
}
