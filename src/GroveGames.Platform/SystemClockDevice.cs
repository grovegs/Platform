using System;

namespace GroveGames.Platform;

public sealed class SystemClockDevice : IClockDevice
{
    private readonly DateTime _startUtcNow;
    private readonly string _bootId;

    public DateTime UtcNow => DateTime.UtcNow;
    public TimeSpan TimeSinceBoot => DateTime.UtcNow - _startUtcNow;
    public string BootId => _bootId;

    public SystemClockDevice()
    {
        _startUtcNow = DateTime.UtcNow;
        _bootId = Guid.NewGuid().ToString("N");
    }
}
