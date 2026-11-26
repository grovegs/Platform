using System;
using System.Diagnostics;

namespace GroveGames.Platform;

public sealed class SystemClockDevice : IClockDevice
{
    private readonly long _startTimestamp;
    private readonly string _bootId;

    public DateTime UtcNow => DateTime.UtcNow;
    public TimeSpan TimeSinceBoot => ToTimeSpan(Stopwatch.GetTimestamp() - _startTimestamp);
    public string BootId => _bootId;

    public SystemClockDevice()
    {
        _startTimestamp = Stopwatch.GetTimestamp();
        _bootId = Guid.NewGuid().ToString("N");
    }

    private static TimeSpan ToTimeSpan(long elapsed)
    {
        var seconds = elapsed / Stopwatch.Frequency;
        var remainder = elapsed % Stopwatch.Frequency;
        return new TimeSpan(seconds * TimeSpan.TicksPerSecond + remainder * TimeSpan.TicksPerSecond / Stopwatch.Frequency);
    }
}
