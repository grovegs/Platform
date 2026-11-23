using System;

namespace GroveGames.Platform;

public sealed class Application : IApplication, IDisposable
{
    private readonly IApplicationDevice _device;
    private bool _isPaused;
    private bool _isFocused;
    private ApplicationState _state;

    public event Action<ApplicationStateChangedEvent>? StateChanged;

    public ApplicationState State => _state;

    public Application(IApplicationDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);

        _device = device;
        _isPaused = false;
        _isFocused = device.IsFocused;
        _state = GetState();
        _device.PauseChanged += HandlePauseChanged;
        _device.FocusChanged += HandleFocusChanged;
    }

    public void Dispose()
    {
        _device.PauseChanged -= HandlePauseChanged;
        _device.FocusChanged -= HandleFocusChanged;
    }

    public void Quit()
    {
        _device.Quit();
    }

    private void HandlePauseChanged(bool isPaused)
    {
        _isPaused = isPaused;
        UpdateState();
    }

    private void HandleFocusChanged(bool isFocused)
    {
        _isFocused = isFocused;
        UpdateState();
    }

    private void UpdateState()
    {
        var state = GetState();

        if (state == _state)
        {
            return;
        }

        var previous = _state;
        _state = state;
        StateChanged?.Invoke(new ApplicationStateChangedEvent(previous, state));
    }

    private ApplicationState GetState()
    {
        if (_isPaused)
        {
            return ApplicationState.Background;
        }

        return _isFocused ? ApplicationState.Active : ApplicationState.Inactive;
    }
}
