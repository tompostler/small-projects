namespace progress
{
    using System;
    using System.Threading;

    class Percentage
    {
        public TimeSpan seconds { get; set; }

        public float percentageAccuracy { get; set; } = 0.9f;

        public void Run()
        {
            DateTime start = DateTime.UtcNow;
            DateTime end = start + seconds;

            int millisecondsPerPercent = (int)((end - start).TotalMilliseconds / 100 * percentageAccuracy);

            while (DateTime.UtcNow < end)
            {
                Console.Write("{0}%  ", (int)((DateTime.UtcNow - start).TotalSeconds * 100 / seconds.TotalSeconds));
                Thread.Sleep(millisecondsPerPercent);
                Console.SetCursorPosition(0, Console.CursorTop);
            }

            Console.WriteLine("100%");
        }
    }
}
