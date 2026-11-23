## Unity

- `IApplication`, `IClock`, `IDisplay` and their types are in the `GroveGames.Platform` namespace. `GroveGames.Platform.Unity` holds the registration, `UnityPlatformSettings` and `UnityEnvironment`.
- Never set `Application.targetFrameRate` or `Screen.sleepTimeout`. Never read `Application.isFocused` or use `OnApplicationPause` in game code.
- The default target frame rate and sleep timeout live in Project Settings > GroveGames > Platform (`UnityPlatformSettings`) and are applied when `IDisplay` starts.
- Register with `builder.AddApplication()`, `builder.AddClock()` and `builder.AddDisplay()` in the root installer. The clock stores its state with GroveGames.Database, so `AddClock()` needs `AddDatabase()` and `AddApplication()` registered first.
- Read `UnityEnvironment.IsEditor`, `IsDevelopment` and `IsProduction`. They are constants. Never use `Application.isEditor`, `Debug.isDebugBuild` or `#if UNITY_EDITOR` for this in game code.
- Read the app version from `UnityEnvironment.Version` or `VersionName`, never `Application.version`.
- An asmdef that overrides references lists `GroveGames.Platform.dll` in its precompiled references.
