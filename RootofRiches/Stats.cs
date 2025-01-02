namespace RootofRiches
{
    public class Stats
    {
        public int GillEarned = 0;
        public int TotalA4nRuns = 0;
        public TimeSpan TotalRunTime = TimeSpan.Zero;
        public TimeSpan FastestRun = TimeSpan.MaxValue;
        public void Reset()
        {
            GillEarned = 0;
            TotalA4nRuns = 0;
            TotalRunTime = TimeSpan.Zero;
            FastestRun = TimeSpan.MaxValue;
        }
    }
}
