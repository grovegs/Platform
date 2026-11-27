using System;

namespace GroveGames.Platform;

public sealed class Clock : IClock, IDisposable
{
    private static readonly DateTime s_latestStoredUtcNow = DateTime.MaxValue.AddYears(-100);

    private readonly IClockDevice _device;
    private readonly IClockStorage _storage;
    private readonly IApplication _application;
    private DateTime _anchorUtcNow;
    private TimeSpan _anchorTimeSinceBoot;
    private bool _isAnchored;

    public DateTime UtcNow => GetUtcNow(_device.TimeSinceBoot);

    public Clock(IClockDevice device, IClockStorage storage, IApplication application)
    {
        _device = device;
        _storage = storage;
        _application = application;
        _application.StateChanged += HandleStateChanged;
    }

    public void Dispose()
    {
        _application.StateChanged -= HandleStateChanged;
        Save();
    }

    public DateTime GetDeviceTime(DateTime utcTime)
    {
        return _device.UtcNow + (utcTime - UtcNow);
    }

    private DateTime GetUtcNow(TimeSpan timeSinceBoot)
    {
        if (!_isAnchored)
        {
            Anchor(timeSinceBoot);
        }

        return _anchorUtcNow + (timeSinceBoot - _anchorTimeSinceBoot);
    }

    private void Anchor(TimeSpan timeSinceBoot)
    {
        var hasStored = _storage.TryLoad(out var stored) && stored.UtcNow <= s_latestStoredUtcNow;

        if (hasStored && !string.IsNullOrEmpty(stored.BootId) && stored.BootId == _device.BootId && timeSinceBoot >= stored.TimeSinceBoot)
        {
            _anchorUtcNow = stored.UtcNow;
            _anchorTimeSinceBoot = stored.TimeSinceBoot;
            _isAnchored = true;
            return;
        }

        var utcNow = _device.UtcNow;
        _anchorUtcNow = hasStored && utcNow < stored.UtcNow ? stored.UtcNow : utcNow;
        _anchorTimeSinceBoot = timeSinceBoot;
        _isAnchored = true;
        Save();
    }

    private void Save()
    {
        if (!_isAnchored)
        {
            return;
        }

        var timeSinceBoot = _device.TimeSinceBoot;
        _storage.Save(new ClockAnchor(GetUtcNow(timeSinceBoot), timeSinceBoot, _device.BootId));
    }

    private void HandleStateChanged(ApplicationStateChangedEvent change)
    {
        if (change.Current != ApplicationState.Active)
        {
            Save();
        }
    }
}
