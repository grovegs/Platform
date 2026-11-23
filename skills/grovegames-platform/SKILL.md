---
name: grovegames-platform
description: Rules for using GroveGames.Platform in game code, covering app state, the clock, frame rate, screen sleep and the environment. Use when code reads the time, reacts to the app going to the background, sets the frame rate or sleep timeout, or checks editor, development or production.
---

# Using GroveGames.Platform

Rules for code that uses this package. They hold in every game. The API is in the package's `README.md`.

- React to the app leaving or returning to the foreground through `IApplication.StateChanged` (`Active`, `Inactive`, `Background`). Shutdown work goes in `Dispose`; there is no quit event.
- Set the frame rate and screen sleep through `IDisplay`, never through the engine.
- A sleep timeout is the system setting, never sleep, or a number of idle seconds. `IDisplay.SetSleepTimeout` overrides the default at runtime. A timed timeout keeps the screen on while there is input and hands it back to the system setting after the idle seconds.
- Read the time from `IClock.UtcNow`, never `DateTime.UtcNow`. The clock takes the device time once and then advances by the device's boot clock, so changing the phone's time does not move it. After a reboot it takes the device time again, but never goes back before its saved time.
- A time handed to the operating system, such as a notification's fire time, goes through `IClock.GetDeviceTime` first.
- Game code takes `IApplication`, `IClock` and `IDisplay` through the constructor. It never implements or calls `IApplicationDevice`, `IDisplayDevice` or `IClockDevice`; those are for the engine package.
