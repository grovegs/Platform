using System;

namespace GroveGames.Platform;

public readonly struct SleepTimeout : IEquatable<SleepTimeout>
{
    public static readonly SleepTimeout SystemSetting = new(SleepTimeoutMode.SystemSetting, 0f);
    public static readonly SleepTimeout NeverSleep = new(SleepTimeoutMode.NeverSleep, 0f);

    private readonly SleepTimeoutMode _mode;
    private readonly float _seconds;

    public SleepTimeoutMode Mode => _mode;
    public float Seconds => _seconds;
    public bool IsSystemSetting => _mode == SleepTimeoutMode.SystemSetting;
    public bool IsNeverSleep => _mode == SleepTimeoutMode.NeverSleep;
    public bool IsTimed => _mode == SleepTimeoutMode.Timed;

    public SleepTimeout(SleepTimeoutMode mode, float seconds)
    {
        _mode = mode;
        _seconds = mode == SleepTimeoutMode.Timed ? Math.Max(seconds, 0f) : 0f;
    }

    public static SleepTimeout FromSeconds(float seconds)
    {
        return new SleepTimeout(SleepTimeoutMode.Timed, seconds);
    }

    public bool Equals(SleepTimeout other)
    {
        return _mode == other._mode && _seconds == other._seconds;
    }

    public override bool Equals(object? obj)
    {
        return obj is SleepTimeout other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_mode, _seconds);
    }

    public override string ToString()
    {
        return IsTimed ? $"{_seconds}s" : _mode.ToString();
    }

    public static bool operator ==(SleepTimeout left, SleepTimeout right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SleepTimeout left, SleepTimeout right)
    {
        return !left.Equals(right);
    }
}
