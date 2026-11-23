using System;

namespace GroveGames.Platform;

public interface IClock
{
    public DateTime UtcNow { get; }

    public DateTime GetDeviceTime(DateTime utcTime);
}
