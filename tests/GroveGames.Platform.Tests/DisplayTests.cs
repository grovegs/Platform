
namespace GroveGames.Platform.Tests;

public sealed class DisplayTests
{
    private const float TimeoutSeconds = 10f;
    private const float DeltaTime = 1f;
    private const int FrameRate = 45;
    private const int DefaultFrameRate = 60;

    private readonly FakeDisplayDevice _device;
    private readonly Display _display;

    public DisplayTests()
    {
        _device = new FakeDisplayDevice();
        _display = new Display(_device, new PlatformSettings(DefaultFrameRate, SleepTimeout.SystemSetting));
    }

    [Fact]
    public void Constructor_Settings_AppliesTargetFrameRate()
    {
        var display = new Display(_device, new PlatformSettings(FrameRate, SleepTimeout.SystemSetting));

        Assert.Equal(FrameRate, display.TargetFrameRate);
    }

    [Fact]
    public void SetTargetFrameRate_Value_OverridesSettings()
    {
        _display.SetTargetFrameRate(FrameRate);

        Assert.Equal(FrameRate, _display.TargetFrameRate);
    }

    [Fact]
    public void Constructor_SettingsNeverSleep_KeepsScreenOn()
    {
        var display = new Display(_device, new PlatformSettings(DefaultFrameRate, SleepTimeout.NeverSleep));

        Assert.Equal(SleepTimeout.NeverSleep, display.SleepTimeout);
        Assert.True(display.KeepScreenOn);
    }

    [Fact]
    public void Constructor_SettingsTimed_UsesConfiguredSeconds()
    {
        var display = new Display(_device, new PlatformSettings(DefaultFrameRate, SleepTimeout.FromSeconds(TimeoutSeconds)));

        Assert.Equal(SleepTimeout.FromSeconds(TimeoutSeconds), display.SleepTimeout);
        Assert.True(display.KeepScreenOn);
    }

    [Fact]
    public void Constructor_Default_IsSystemSetting()
    {
        Assert.Equal(SleepTimeout.SystemSetting, _display.SleepTimeout);
        Assert.False(_display.KeepScreenOn);
    }

    [Fact]
    public void SetSleepTimeout_NeverSleep_KeepsScreenOn()
    {
        _display.SetSleepTimeout(SleepTimeout.NeverSleep);

        Assert.Equal(SleepTimeout.NeverSleep, _display.SleepTimeout);
        Assert.True(_display.KeepScreenOn);
    }

    [Fact]
    public void SetSleepTimeout_SystemSettingAfterNeverSleep_ReleasesScreen()
    {
        _display.SetSleepTimeout(SleepTimeout.NeverSleep);

        _display.SetSleepTimeout(SleepTimeout.SystemSetting);

        Assert.False(_display.KeepScreenOn);
    }

    [Fact]
    public void SetSleepTimeout_Timed_KeepsScreenOnUntilTimeout()
    {
        _display.SetSleepTimeout(SleepTimeout.FromSeconds(TimeoutSeconds));

        Assert.True(_display.KeepScreenOn);
    }

    [Fact]
    public void Update_IdlePastTimeout_ReleasesScreen()
    {
        _display.SetSleepTimeout(SleepTimeout.FromSeconds(TimeoutSeconds));

        for (var i = 0; i < TimeoutSeconds; i++)
        {
            _display.Update(DeltaTime);
        }

        Assert.False(_display.KeepScreenOn);
    }

    [Fact]
    public void Update_IdleBeforeTimeout_KeepsScreenOn()
    {
        _display.SetSleepTimeout(SleepTimeout.FromSeconds(TimeoutSeconds));

        for (var i = 0; i < TimeoutSeconds - 1; i++)
        {
            _display.Update(DeltaTime);
        }

        Assert.True(_display.KeepScreenOn);
    }

    [Fact]
    public void Update_InputAfterTimeout_KeepsScreenOnAgain()
    {
        _display.SetSleepTimeout(SleepTimeout.FromSeconds(TimeoutSeconds));

        for (var i = 0; i < TimeoutSeconds; i++)
        {
            _display.Update(DeltaTime);
        }

        _device.SetHasInput(true);
        _display.Update(DeltaTime);

        Assert.True(_display.KeepScreenOn);
    }

    [Fact]
    public void Update_InputResetsIdleTime_KeepsScreenOnPastOriginalTimeout()
    {
        _display.SetSleepTimeout(SleepTimeout.FromSeconds(TimeoutSeconds));

        for (var i = 0; i < TimeoutSeconds - 1; i++)
        {
            _display.Update(DeltaTime);
        }

        _device.SetHasInput(true);
        _display.Update(DeltaTime);
        _device.SetHasInput(false);
        _display.Update(DeltaTime);

        Assert.True(_display.KeepScreenOn);
    }

    [Fact]
    public void Update_NeverSleepWhileIdle_KeepsScreenOn()
    {
        _display.SetSleepTimeout(SleepTimeout.NeverSleep);

        for (var i = 0; i < TimeoutSeconds; i++)
        {
            _display.Update(DeltaTime);
        }

        Assert.True(_display.KeepScreenOn);
    }

    [Fact]
    public void Update_SystemSettingWithInput_DoesNotKeepScreenOn()
    {
        _display.SetSleepTimeout(SleepTimeout.SystemSetting);
        _device.SetHasInput(true);

        _display.Update(DeltaTime);

        Assert.False(_display.KeepScreenOn);
    }

    [Fact]
    public void SetSleepTimeout_TimedAfterIdle_RestartsTimeout()
    {
        _display.SetSleepTimeout(SleepTimeout.FromSeconds(TimeoutSeconds));

        for (var i = 0; i < TimeoutSeconds; i++)
        {
            _display.Update(DeltaTime);
        }

        _display.SetSleepTimeout(SleepTimeout.FromSeconds(TimeoutSeconds));

        Assert.True(_display.KeepScreenOn);
    }

    [Fact]
    public void FromSeconds_Negative_ClampsToZero()
    {
        var sleepTimeout = SleepTimeout.FromSeconds(-5f);

        Assert.True(sleepTimeout.IsTimed);
        Assert.Equal(0f, sleepTimeout.Seconds);
    }

    [Fact]
    public void Constructor_NeverSleepWithSeconds_IgnoresSeconds()
    {
        var sleepTimeout = new SleepTimeout(SleepTimeoutMode.NeverSleep, TimeoutSeconds);

        Assert.Equal(SleepTimeout.NeverSleep, sleepTimeout);
        Assert.Equal(0f, sleepTimeout.Seconds);
    }

    [Fact]
    public void FromSeconds_Positive_IsTimed()
    {
        var sleepTimeout = SleepTimeout.FromSeconds(TimeoutSeconds);

        Assert.True(sleepTimeout.IsTimed);
        Assert.Equal(TimeoutSeconds, sleepTimeout.Seconds);
        Assert.False(sleepTimeout.IsNeverSleep);
        Assert.False(sleepTimeout.IsSystemSetting);
    }

    [Fact]
    public void Constructor_FrameRate_ReachesTheDevice()
    {
        Assert.Equal(DefaultFrameRate, _device.TargetFrameRate);
    }

    [Fact]
    public void SetSleepTimeout_NeverSleepTwice_TellsTheDeviceOnce()
    {
        _display.SetSleepTimeout(SleepTimeout.NeverSleep);
        _display.SetSleepTimeout(SleepTimeout.NeverSleep);

        Assert.True(_device.KeepScreenOn);
        Assert.Equal(1, _device.KeepScreenOnChangeCount);
    }

    [Fact]
    public void Constructor_SystemSetting_DoesNotTouchTheScreen()
    {
        Assert.Equal(0, _device.KeepScreenOnChangeCount);
    }

    private sealed class FakeDisplayDevice : IDisplayDevice
    {
        private bool _hasInput;
        private int _targetFrameRate;
        private bool _keepScreenOn;
        private int _keepScreenOnChangeCount;

        public bool HasInput => _hasInput;
        public int TargetFrameRate => _targetFrameRate;
        public bool KeepScreenOn => _keepScreenOn;
        public int KeepScreenOnChangeCount => _keepScreenOnChangeCount;

        public void SetHasInput(bool hasInput)
        {
            _hasInput = hasInput;
        }

        public void SetTargetFrameRate(int frameRate)
        {
            _targetFrameRate = frameRate;
        }

        public void SetKeepScreenOn(bool keepScreenOn)
        {
            _keepScreenOn = keepScreenOn;
            _keepScreenOnChangeCount++;
        }
    }
}
