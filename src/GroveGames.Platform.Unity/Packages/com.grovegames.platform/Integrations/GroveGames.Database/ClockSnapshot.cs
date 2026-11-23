using System;
using GroveGames.Serialization;

namespace GroveGames.Platform.Unity
{
    [Schema]
    public sealed class ClockSnapshot
    {
        public DateTime UtcNow;
        public TimeSpan TimeSinceBoot;
        public string BootId = string.Empty;
    }
}
