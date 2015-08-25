// Adapted from https://msdn.microsoft.com/en-us/library/system.diagnostics.stopwatch.aspx

using System;
using System.Diagnostics;

class PerformanceTesting
{
    /// <summary>
    /// Edit this function to perform the perf test
    /// </summary>
    private static void DoTheThing()
    {
        throw new NotImplementedException();
    }

    static void Main(string[] args)
    {
        DisplayTimerProperties();

        Console.WriteLine();
        Console.WriteLine("Press the Enter key to begin:");
        Console.ReadLine();
        Console.WriteLine();

        TimeOperations();

#if DEBUG
        Console.WriteLine();
        Console.WriteLine("Press the Enter key to exit");
        Console.ReadLine();
#endif
    }

    private static void DisplayTimerProperties()
    {
        // Display the timer frequency and resolution. 
        if (Stopwatch.IsHighResolution)
        {
            Console.WriteLine("Operations timed using the system's high-resolution performance counter.");
        }
        else
        {
            Console.WriteLine("Operations timed using the DateTime class.");
        }

        long frequency = Stopwatch.Frequency;
        Console.WriteLine("  Timer frequency in ticks per second = {0}", frequency);
        long nanosecPerTick = (1000L * 1000L * 1000L) / frequency;
        Console.WriteLine("  Timer is accurate within {0} nanoseconds", nanosecPerTick);
    }

    private static void TimeOperations()
    {
        long nanosecPerTick = (1000L * 1000L * 1000L) / Stopwatch.Frequency;
        const long numIterations = 10000;

        // Define variables for operation statistics.
        long numTicks = 0;
        long numRollovers = 0;
        long maxTicks = 0;
        long minTicks = Int64.MaxValue;
        int indexFastest = -1;
        int indexSlowest = -1;
        long milliSec = 0;

        Stopwatch time10kOperations = Stopwatch.StartNew();

        // Run the current operation 10001 times. 
        // The first execution time will be tossed out, since it can skew the average time.
        for (int i = 0; i <= numIterations; i++)
        {
            long ticksThisTime = 0;
            Stopwatch timePerParse;

            timePerParse = Stopwatch.StartNew();

            // Do the thing
            DoTheThing();

            timePerParse.Stop();
            ticksThisTime = timePerParse.ElapsedTicks;

            // Skip over the time for the first operation, just in case it caused a one-time performance hit.
            if (i == 0)
            {
                time10kOperations.Reset();
                time10kOperations.Start();
            }
            else
            {
                // Update operation statistics for iterations 1-10001. 
                if (maxTicks < ticksThisTime)
                {
                    indexSlowest = i;
                    maxTicks = ticksThisTime;
                }
                if (minTicks > ticksThisTime)
                {
                    indexFastest = i;
                    minTicks = ticksThisTime;
                }
                numTicks += ticksThisTime;
                if (numTicks < ticksThisTime)
                {
                    // Keep track of rollovers.
                    numRollovers++;
                }
            }
        }

        // Display the statistics for 10000 iterations.
        time10kOperations.Stop();
        milliSec = time10kOperations.ElapsedMilliseconds;

        Console.WriteLine("Statistics:");
        Console.WriteLine("  Slowest time:  Iteration {0} = {1,4} ticks = {2,4} nanoseconds", indexSlowest, maxTicks, maxTicks * nanosecPerTick);
        Console.WriteLine("  Fastest time:  Iteration {0} = {1,4} ticks = {2,4} nanoseconds", indexFastest, minTicks, minTicks * nanosecPerTick);
        Console.WriteLine("  Average time:  {0} ticks = {1} nanoseconds", numTicks / numIterations, (numTicks * nanosecPerTick) / numIterations);
        Console.WriteLine("  Total time looping through {0} operations: {1} milliseconds", numIterations, milliSec);
    }
}
