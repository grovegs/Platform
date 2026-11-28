using System;
using GroveGames.DeveloperTools;

namespace GroveGames.Platform.Unity
{
    public sealed class ClockDeveloperOptions : IDeveloperOptions
    {
        private readonly Clock _clock;

        public string Category => "Platform";

        public ClockDeveloperOptions(Clock clock)
        {
            _clock = clock;
        }

        [DeveloperOption("Advance 1 Hour", Header = "Clock", Tooltip = "Moves the game clock forward and saves it. The clock never goes back, so only clearing the data undoes it.")]
        private void AdvanceHour()
        {
            _clock.Advance(TimeSpan.FromHours(1d));
        }

        [DeveloperOption("Advance 1 Day")]
        private void AdvanceDay()
        {
            _clock.Advance(TimeSpan.FromDays(1d));
        }
    }
}
