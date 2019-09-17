using System;

namespace ADS_Simulation
{
    class Program
    {
        static void Main(string[] args)
        {
            MarshallArgs(args);
            Console.WriteLine("Hello World!");
        }

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

+====================================+===========+====================================+
| Command              Parameters    | Default   | Detail                             |
+====================================+===========+====================================+

+====================================+===========+====================================+
");
        }
    }
}
