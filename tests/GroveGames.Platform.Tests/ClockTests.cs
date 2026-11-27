using System;
using System.Collections.Generic;

namespace GroveGames.Platform.Tests;

public sealed class ClockTests : IDisposable
{
    private static readonly DateTime s_start = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    private readonly FakeClockStorage _storage;
    private readonly FakeApplicationDevice _applicationDevice;
    private readonly Application _application;
    private readonly FakeClockDevice _device;
    private readonly List<Clock> _clocks;

    public ClockTests()
    {
        _storage = new FakeClockStorage();
        _applicationDevice = new FakeApplicationDevice(true);
        _application = new Application(_applicationDevice);
        _device = new FakeClockDevice();
        _clocks = new List<Clock>();
    }

    public void Dispose()
    {
        foreach (var clock in _clocks)
        {
            clock.Dispose();
        }

        _application.Dispose();
    }

    [Fact]
    public void UtcNow_FirstUse_ReturnsDeviceTime()
    {
        var clock = CreateClock();

        var utcNow = clock.UtcNow;

        Assert.Equal(s_start, utcNow);
    }

    [Fact]
    public void UtcNow_TimePasses_FollowsTimeSinceBoot()
    {
        var clock = CreateClock();
        _ = clock.UtcNow;

        _device.Advance(TimeSpan.FromHours(2d));

        Assert.Equal(s_start.AddHours(2d), clock.UtcNow);
    }

    [Fact]
    public void UtcNow_DeviceTimeMovedForward_IgnoresTheChange()
    {
        var clock = CreateClock();
        _ = clock.UtcNow;

        _device.ChangeUtcNow(TimeSpan.FromDays(1d));
        _device.Advance(TimeSpan.FromMinutes(10d));

        Assert.Equal(s_start.AddMinutes(10d), clock.UtcNow);
    }

    [Fact]
    public void UtcNow_RestartedInSameBootWithDeviceTimeMovedForward_FollowsTimeSinceBoot()
    {
        var first = CreateClock();
        _ = first.UtcNow;
        _device.ChangeUtcNow(TimeSpan.FromDays(1d));
        _device.Advance(TimeSpan.FromHours(3d));

        var second = CreateClock();

        Assert.Equal(s_start.AddHours(3d), second.UtcNow);
    }

    [Fact]
    public void UtcNow_RestartedInSameBootWithDeviceTimeMovedBack_FollowsTimeSinceBoot()
    {
        var first = CreateClock();
        _ = first.UtcNow;
        _device.ChangeUtcNow(TimeSpan.FromDays(-1d));
        _device.Advance(TimeSpan.FromHours(3d));

        var second = CreateClock();

        Assert.Equal(s_start.AddHours(3d), second.UtcNow);
    }

    [Fact]
    public void UtcNow_AfterReboot_ReturnsDeviceTime()
    {
        var first = CreateClock();
        _ = first.UtcNow;
        _device.ChangeUtcNow(TimeSpan.FromHours(8d));
        _device.Reboot("2", TimeSpan.FromMinutes(1d));

        var second = CreateClock();

        Assert.Equal(s_start.AddHours(8d), second.UtcNow);
    }

    [Fact]
    public void UtcNow_AfterRebootWithLongerTimeSinceBoot_ReturnsDeviceTime()
    {
        var first = CreateClock();
        _ = first.UtcNow;
        _device.ChangeUtcNow(TimeSpan.FromDays(2d));
        _device.Reboot("2", TimeSpan.FromHours(30d));

        var second = CreateClock();

        Assert.Equal(s_start.AddDays(2d), second.UtcNow);
    }

    [Fact]
    public void UtcNow_AfterRebootWithDeviceTimeMovedBack_ContinuesFromLastTime()
    {
        var first = CreateClock();
        _ = first.UtcNow;
        _device.ChangeUtcNow(TimeSpan.FromHours(-6d));
        _device.Reboot("2", TimeSpan.FromMinutes(1d));
        var second = CreateClock();
        _ = second.UtcNow;

        _device.Advance(TimeSpan.FromHours(7d));

        Assert.Equal(s_start.AddHours(7d), second.UtcNow);
    }

    [Fact]
    public void UtcNow_SameBootIdWithShorterTimeSinceBoot_ReturnsDeviceTime()
    {
        var first = CreateClock();
        _ = first.UtcNow;
        _device.ChangeUtcNow(TimeSpan.FromHours(8d));
        _device.Reboot("1", TimeSpan.FromMinutes(1d));

        var second = CreateClock();

        Assert.Equal(s_start.AddHours(8d), second.UtcNow);
    }

    [Fact]
    public void UtcNow_AfterRebootBehindTimeSavedWhenInactive_ContinuesFromInactiveTime()
    {
        var first = CreateClock();
        _ = first.UtcNow;
        _device.Advance(TimeSpan.FromHours(4d));
        _applicationDevice.ChangeFocus(false);
        _device.ChangeUtcNow(TimeSpan.FromHours(-2d));
        _device.Reboot("2", TimeSpan.FromMinutes(1d));

        var second = CreateClock();

        Assert.Equal(s_start.AddHours(4d), second.UtcNow);
    }

    [Fact]
    public void UtcNow_AfterRebootBehindTimeSavedInBackground_ContinuesFromBackgroundTime()
    {
        var first = CreateClock();
        _ = first.UtcNow;
        _device.Advance(TimeSpan.FromHours(4d));
        _applicationDevice.ChangePause(true);
        _device.ChangeUtcNow(TimeSpan.FromHours(-2d));
        _device.Reboot("2", TimeSpan.FromMinutes(1d));

        var second = CreateClock();

        Assert.Equal(s_start.AddHours(4d), second.UtcNow);
    }

    [Fact]
    public void UtcNow_RestartedWithoutBootId_ReturnsDeviceTime()
    {
        _device.Reboot(string.Empty, TimeSpan.FromHours(5d));
        var first = CreateClock();
        _ = first.UtcNow;
        _device.ChangeUtcNow(TimeSpan.FromHours(8d));
        _device.Advance(TimeSpan.FromHours(1d));

        var second = CreateClock();

        Assert.Equal(s_start.AddHours(9d), second.UtcNow);
    }

    [Fact]
    public void Dispose_Anchored_SavesTheCurrentTime()
    {
        var first = CreateClock();
        _ = first.UtcNow;
        _device.Advance(TimeSpan.FromHours(4d));
        first.Dispose();
        _device.ChangeUtcNow(TimeSpan.FromHours(-2d));
        _device.Reboot("2", TimeSpan.FromMinutes(1d));

        var second = CreateClock();

        Assert.Equal(s_start.AddHours(4d), second.UtcNow);
    }

    [Fact]
    public void Dispose_BeforeFirstUse_SavesNothing()
    {
        var clock = CreateClock();

        clock.Dispose();

        Assert.Equal(0, _storage.SaveCount);
    }

    [Fact]
    public void GetDeviceTime_DeviceTimeMovedForward_ShiftsByTheDifference()
    {
        var clock = CreateClock();
        _ = clock.UtcNow;
        _device.ChangeUtcNow(TimeSpan.FromDays(1d));

        var deviceTime = clock.GetDeviceTime(s_start.AddHours(3d));

        Assert.Equal(s_start.AddDays(1d).AddHours(3d), deviceTime);
    }

    [Fact]
    public void GetDeviceTime_DeviceTimeUnchanged_ReturnsTheSameTime()
    {
        var clock = CreateClock();

        var deviceTime = clock.GetDeviceTime(s_start.AddHours(3d));

        Assert.Equal(s_start.AddHours(3d), deviceTime);
    }

    [Fact]
    public void UtcNow_StoredTimeNearTheEndOfTime_IgnoresItAndReturnsDeviceTime()
    {
        _storage.Save(new ClockAnchor(DateTime.MaxValue.AddMinutes(-1d), TimeSpan.FromHours(1d), "1"));

        var clock = CreateClock();

        Assert.Equal(s_start, clock.UtcNow);
    }

    private Clock CreateClock()
    {
        var clock = new Clock(_device, _storage, _application);
        _clocks.Add(clock);
        return clock;
    }

    private sealed class FakeApplicationDevice : IApplicationDevice
    {
        private readonly bool _isFocused;
        private int _quitCount;

        public event Action<bool>? PauseChanged;
        public event Action<bool>? FocusChanged;

        public bool IsFocused => _isFocused;

        public FakeApplicationDevice(bool isFocused)
        {
            _isFocused = isFocused;
        }

        public int QuitCount => _quitCount;

        public void Quit()
        {
            _quitCount++;
        }

        public void ChangePause(bool isPaused)
        {
            PauseChanged?.Invoke(isPaused);
        }

        public void ChangeFocus(bool isFocused)
        {
            FocusChanged?.Invoke(isFocused);
        }
    }

    private sealed class FakeClockDevice : IClockDevice
    {
        private DateTime _utcNow = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        private TimeSpan _timeSinceBoot = TimeSpan.FromHours(5d);
        private string _bootId = "1";

        public DateTime UtcNow => _utcNow;
        public TimeSpan TimeSinceBoot => _timeSinceBoot;
        public string BootId => _bootId;

        public void Advance(TimeSpan duration)
        {
            _utcNow += duration;
            _timeSinceBoot += duration;
        }

        public void ChangeUtcNow(TimeSpan offset)
        {
            _utcNow += offset;
        }

        public void Reboot(string bootId, TimeSpan timeSinceBoot)
        {
            _bootId = bootId;
            _timeSinceBoot = timeSinceBoot;
        }
    }

    private sealed class FakeClockStorage : IClockStorage
    {
        private ClockAnchor _anchor;
        private bool _hasAnchor;
        private int _saveCount;

        public int SaveCount => _saveCount;

        public bool TryLoad(out ClockAnchor anchor)
        {
            anchor = _anchor;
            return _hasAnchor;
        }

        public void Save(ClockAnchor anchor)
        {
            _anchor = anchor;
            _hasAnchor = true;
            _saveCount++;
        }
    }
}
