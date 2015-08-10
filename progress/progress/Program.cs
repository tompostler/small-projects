namespace progress
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            TimeSpan seconds;
            ParseOptions.Parse(args, out seconds);

            Percentage perprog = new Percentage()
            {
                seconds = seconds,
            };
            perprog.Run();
        }
    }
}
