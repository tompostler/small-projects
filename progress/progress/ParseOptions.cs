namespace progress
{
    using Mono.Options;
    using System;
    using System.Collections.Generic;

    class ParseOptions
    {
        public static void Parse(string[] args, out TimeSpan duration)
        {
            duration = default(TimeSpan);

            // Options
            OptionSet p = new OptionSet()
            {
                "",
                "Usage: progress duration",
                "Count up in percentage on one line the duration in seconds.",
                ""
            };

            // Parse options
            List<String> dur = null;
            try
            {
                dur = p.Parse(args);
            }
            catch (OptionException)
            {
                Console.Error.WriteLine("PROGRESS: Failed to parse options");
                Environment.Exit(Environment.ExitCode);
            }

            // Check for duration
            if (dur.Count != 1)
            {
                Console.Error.WriteLine("PROGRESS: No duration detected");
                Environment.Exit(Environment.ExitCode);
            }

            // Parse duration
            try
            {
                duration = new TimeSpan(0, 0, int.Parse(dur[0]));
            }
            catch (FormatException)
            {
                Console.Error.WriteLine("PROGRESS: Bad duration parameter");
                Environment.Exit(Environment.ExitCode);
            }
            catch (OverflowException)
            {
                Console.Error.WriteLine("PROGRESS: duration parameter too big");
                Environment.Exit(Environment.ExitCode);
            }
        }
    }
}
