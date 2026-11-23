using System;

namespace GroveGames.Platform;

public interface IClockDevice
{
    public DateTime UtcNow { get; }
    public TimeSpan TimeSinceBoot { get; }
    public string BootId { get; }
}
