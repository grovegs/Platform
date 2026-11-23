namespace GroveGames.Platform;

public sealed class Display : IDisplay
{
    private readonly IDisplayDevice _device;
    private SleepTimeout _sleepTimeout;
    private float _idleTime;
    private bool _keepScreenOn;

    public int TargetFrameRate => _device.TargetFrameRate;
    public SleepTimeout SleepTimeout => _sleepTimeout;
    public bool KeepScreenOn => _keepScreenOn;

    public Display(IDisplayDevice device, IPlatformSettings settings)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(settings);

        _device = device;
        _sleepTimeout = settings.SleepTimeout;
        _idleTime = 0f;
        _keepScreenOn = false;
        SetTargetFrameRate(settings.TargetFrameRate);
        UpdateKeepScreenOn();
    }

    public void SetTargetFrameRate(int frameRate)
    {
        _device.SetTargetFrameRate(frameRate);
    }

    public void SetSleepTimeout(SleepTimeout sleepTimeout)
    {
        _sleepTimeout = sleepTimeout;
        _idleTime = 0f;
        UpdateKeepScreenOn();
    }

    public void Update(float deltaTime)
    {
        if (!_sleepTimeout.IsTimed)
        {
            return;
        }

        _idleTime = _device.HasInput ? 0f : _idleTime + deltaTime;
        UpdateKeepScreenOn();
    }

    private void UpdateKeepScreenOn()
    {
        var keepScreenOn = _sleepTimeout.IsNeverSleep || (_sleepTimeout.IsTimed && _idleTime < _sleepTimeout.Seconds);

        if (keepScreenOn == _keepScreenOn)
        {
            return;
        }

        _keepScreenOn = keepScreenOn;
        _device.SetKeepScreenOn(keepScreenOn);
    }
}
