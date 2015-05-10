using System;

namespace CleanEmulatorFrontend.GUI
{
    public class GameStats
    {
        public int LaunchCount { get; set; }
        public DateTime LastLaunched { get; set; }

        public void IncrementLaunchCounter()
        {
            LaunchCount++;
            LastLaunched = DateTime.Now;
        }
    }
}