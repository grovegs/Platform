using System;

namespace GroveGames.Platform;

public readonly struct ClockAnchor
{
    public DateTime UtcNow { get; }
    public TimeSpan TimeSinceBoot { get; }
    public string BootId { get; }

    public ClockAnchor(DateTime utcNow, TimeSpan timeSinceBoot, string bootId)
    {
        UtcNow = utcNow;
        TimeSinceBoot = timeSinceBoot;
        BootId = bootId;
    }
}
