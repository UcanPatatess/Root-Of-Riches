namespace RootofRiches
{
    public class Stats
    {
        public TimeSpan FastestRun = TimeSpan.MaxValue;

        public int GillEarned = 0;
        public TimeSpan TotalRunTime = TimeSpan.Zero;
        public int TotalA4nRuns = 0;
        public int TotalO3nRuns = 0;
        public TimeSpan FastestA4NRun = TimeSpan.MaxValue;
        public TimeSpan FastestO3NRun = TimeSpan.MaxValue;
        public TimeSpan TotalTimeA4N = TimeSpan.Zero;
        public TimeSpan TotalTimeO3N = TimeSpan.Zero;

        public void Reset()
        {
            GillEarned = 0;
            TotalRunTime = TimeSpan.Zero;
            TotalA4nRuns = 0;
            TotalO3nRuns = 0;
            FastestA4NRun = TimeSpan.MaxValue;
            FastestO3NRun = TimeSpan.MaxValue;
            TotalTimeA4N = TimeSpan.Zero;
            TotalTimeO3N = TimeSpan.Zero;
        }
    }
}
