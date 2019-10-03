using ADS_Simulation.Configuration;
using ADS_Simulation.NS_State;
using System;
using System.Threading;

namespace ADS_Simulation
{
    class Program
    {
        static string configPath = "../../../config.json";

        static void Main(string[] args)
        {
            MarshallArgs(args);

            // Initialize config
            Config.readConfig(configPath);

            // Initialize simulation
            Simulation simulation = new Simulation();
            while(simulation.Step()) {
                Thread.Sleep(100);
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
            Console.WriteLine(@"
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
