using Mono.Options;
using System;

namespace sendemail
{
    class Util
    {
        public static void fail(OptionSet p = null, string msg = null)
        {
            if (!String.IsNullOrEmpty(msg))
                Console.Error.WriteLine("SENDEMAIL: {0}", msg);
            if (p != null)
                p.WriteOptionDescriptions(Console.Error);

#if (DEBUG)
            Console.ReadLine();
#endif

            Environment.Exit(Environment.ExitCode);
        }

        public static string HomeDirectoryPath()
        {
            // Home dir (http://stackoverflow.com/q/1143706/4062072)
            return (Environment.OSVersion.Platform == PlatformID.Unix ||
                    Environment.OSVersion.Platform == PlatformID.MacOSX)
                ? Environment.GetEnvironmentVariable("HOME")
                : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
        }
    }
}
