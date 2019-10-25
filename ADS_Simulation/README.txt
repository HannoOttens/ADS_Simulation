We used three NuGet packages:
- MathNet.Numerics
    Used for sampling distributions 
- Newtonsoft.Json
    Used for json config
- OptimizedPriorityQueue
    Used for eventqueue

The solution is currently set up in a way to do a mass amount of runs.
The results of these runs is written to the /output folder with files for every separate time interval.
If you want to run the simulation just once, you can call the Simulate() method with a simulation instance.

Furthermore, you can use console arguments to change some options, these can be viewed with -h.