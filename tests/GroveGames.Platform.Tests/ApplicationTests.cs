using System.Collections.Generic;

namespace GroveGames.Platform.Tests;

public sealed class ApplicationTests : IDisposable
{
    private readonly TestApplicationDevice _device;
    private readonly Application _application;
    private readonly List<ApplicationStateChangedEvent> _events;

    public ApplicationTests()
    {
        _device = new TestApplicationDevice(true);
        _application = new Application(_device);
        _events = new List<ApplicationStateChangedEvent>();
        _application.StateChanged += _events.Add;
    }

    public void Dispose()
    {
        _application.Dispose();
    }

    [Fact]
    public void Constructor_Focused_IsActive()
    {
        Assert.Equal(ApplicationState.Active, _application.State);
    }

    [Fact]
    public void Constructor_Unfocused_IsInactive()
    {
        var device = new TestApplicationDevice(false);

        var application = new Application(device);

        Assert.Equal(ApplicationState.Inactive, application.State);
    }

    [Fact]
    public void FocusChanged_Lost_RaisesInactive()
    {
        _device.ChangeFocus(false);

        Assert.Equal(ApplicationState.Inactive, _application.State);
        Assert.Single(_events);
        Assert.Equal(ApplicationState.Active, _events[0].Previous);
        Assert.Equal(ApplicationState.Inactive, _events[0].Current);
    }

    [Fact]
    public void PauseChanged_Paused_RaisesBackground()
    {
        _device.ChangePause(true);

        Assert.Equal(ApplicationState.Background, _application.State);
        Assert.Single(_events);
        Assert.Equal(ApplicationState.Background, _events[0].Current);
    }

    [Fact]
    public void BackgroundAndBack_MobileOrder_RaisesEachState()
    {
        _device.ChangeFocus(false);
        _device.ChangePause(true);
        _device.ChangePause(false);
        _device.ChangeFocus(true);

        Assert.Equal(4, _events.Count);
        Assert.Equal(ApplicationState.Inactive, _events[0].Current);
        Assert.Equal(ApplicationState.Background, _events[1].Current);
        Assert.Equal(ApplicationState.Inactive, _events[2].Current);
        Assert.Equal(ApplicationState.Active, _events[3].Current);
    }

    [Fact]
    public void FocusChanged_WhileBackground_StaysBackground()
    {
        _device.ChangePause(true);

        _device.ChangeFocus(true);
        _device.ChangeFocus(false);

        Assert.Equal(ApplicationState.Background, _application.State);
        Assert.Single(_events);
    }

    [Fact]
    public void PauseChanged_ResumedWhileFocused_RaisesActive()
    {
        _device.ChangePause(true);

        _device.ChangePause(false);

        Assert.Equal(2, _events.Count);
        Assert.Equal(ApplicationState.Background, _events[1].Previous);
        Assert.Equal(ApplicationState.Active, _events[1].Current);
    }

    [Fact]
    public void FocusChanged_SameValue_DoesNotRaise()
    {
        _device.ChangeFocus(true);

        Assert.Empty(_events);
    }

    [Fact]
    public void Dispose_ThenFocusChanged_DoesNotRaise()
    {
        _application.Dispose();

        _device.ChangeFocus(false);

        Assert.Equal(ApplicationState.Active, _application.State);
        Assert.Empty(_events);
    }

    [Fact]
    public void Quit_Device_QuitsOnce()
    {
        _application.Quit();

        Assert.Equal(1, _device.QuitCount);
    }

    private sealed class TestApplicationDevice : IApplicationDevice
    {
        private readonly bool _isFocused;
        private int _quitCount;

        public event Action<bool>? PauseChanged;
        public event Action<bool>? FocusChanged;

        public bool IsFocused => _isFocused;

        public TestApplicationDevice(bool isFocused)
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
}
