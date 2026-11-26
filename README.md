# GroveGames.Platform

[![Build Status](https://github.com/grovegs/Platform/actions/workflows/release.yml/badge.svg)](https://github.com/grovegs/Platform/actions/workflows/release.yml)
[![Tests](https://github.com/grovegs/Platform/actions/workflows/tests.yml/badge.svg)](https://github.com/grovegs/Platform/actions/workflows/tests.yml)
[![NuGet](https://img.shields.io/nuget/v/GroveGames.Platform)](https://www.nuget.org/packages/GroveGames.Platform)
[![Latest Release](https://img.shields.io/github/v/release/grovegs/Platform)](https://github.com/grovegs/Platform/releases/latest)

Application state, display settings, a clock that resists device time changes, and the build environment for .NET and Unity

## Features

- `IApplication`: `Active`, `Inactive` and `Background`, with a `StateChanged` event, worked out from pause and focus
- `IDisplay`: the target frame rate, and a screen sleep timeout that can be the system setting, never, or a number of idle seconds
- `IClock`: UTC time that starts from the device time once and then follows the device's boot clock, so changing the device time does not move it
- `IPlatformSettings`: the target frame rate and sleep timeout a display starts with, with `PlatformSettings` as the plain implementation
- `Environment`: editor, development or production, with the app version and a version name
- Each of these talks to the platform through one small interface: `IApplicationDevice`, `IDisplayDevice`, `IClockDevice`, `IClockStorage`
- Unity: the devices for the Editor, Android and iOS, clock storage in PlayerPrefs or GroveGames.Database, and `UnityPlatformSettings`, the same settings as an asset
- Test doubles for your own tests: `FakeApplication`, `FakeClock` and `FakeDisplay` in the editor-only `GroveGames.Platform.Unity.Testing` assembly

## .NET

### Installation

```bash
dotnet add package GroveGames.Platform
```

### Usage

```csharp
using GroveGames.Platform;

var application = new Application(applicationDevice);
application.StateChanged += change => Console.WriteLine($"{change.Previous} -> {change.Current}");

var clock = new Clock(new SystemClockDevice(), clockStorage, application);
var now = clock.UtcNow;
var fireTime = clock.GetDeviceTime(now.AddHours(8));

var display = new Display(displayDevice, new PlatformSettings(targetFrameRate: 60, SleepTimeout.FromSeconds(120f)));
display.Update(deltaTime);
```

- The application is `Background` while paused, `Active` while focused and `Inactive` otherwise.
- After a reboot the clock takes the device time again, but never goes back before the time it saved. It saves when the application leaves `Active` and when it is disposed.
- A time handed to the operating system, such as a notification's fire time, goes through `GetDeviceTime` first.
- With a timed sleep timeout, call `Display.Update` every frame. It keeps the screen on while there is input and releases it after the idle seconds.
- `SystemClockDevice` counts from process start with a monotonic timer, so changing the system clock does not move it. It is the device for platforms that have no boot clock.

## Unity

### Installation

1. Install the core package via [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity):
   - Open Window > NuGet > Manage NuGet Packages
   - Search for `GroveGames.Platform` and install

2. Add the Unity package via Git URL:
   - Open Package Manager (Window > Package Manager)
   - Click "+" > "Add package from git URL"
   - Enter: `https://github.com/grovegs/Platform.git?path=src/GroveGames.Platform.Unity/Packages/com.grovegames.platform`

3. Create a `csc.rsp` file in your `Assets/` directory with the following content to enable C# 10 features:

   ```text
   -langversion:10
   -nullable:enable
   ```

### Usage

With [GroveGames.DependencyInjection](https://github.com/grovegs/DependencyInjection) installed:

```csharp
using GroveGames.Platform.Unity;

builder.AddApplication();
builder.AddClock();
builder.AddDisplay();
```

- `AddClock()` needs `AddApplication()`. With [GroveGames.Database](https://github.com/grovegs/Database) installed it stores its state there and needs the database registered; otherwise it uses PlayerPrefs.
- The default target frame rate and sleep timeout are in Project Settings > GroveGames > Platform, stored in a `UnityPlatformSettings` asset, which implements `IPlatformSettings`.
- `UnityEnvironment.IsEditor`, `IsDevelopment` and `IsProduction` are constants, so the compiler removes the branch that does not apply. A build is a development build when the `DEVELOPMENT` symbol is defined. `UnityEnvironment.Current` is the same information as a core `Environment`.

## Development

### Project Structure

- `src/GroveGames.Platform/` - Core .NET library (`net10.0`, `netstandard2.1`)
- `src/GroveGames.Platform.Unity/` - Unity package
- `tests/` - xUnit tests
- `sandbox/` - Sample applications for testing

### Commands

```bash
dotnet build                        # Build all projects
dotnet format                       # Format code
dotnet test                         # Run tests
dotnet pack -c Release              # Create NuGet packages
```

### Requirements

- .NET 10.0 SDK

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
