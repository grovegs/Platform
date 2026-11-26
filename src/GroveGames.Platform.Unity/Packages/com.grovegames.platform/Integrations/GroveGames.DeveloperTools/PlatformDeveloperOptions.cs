using System;
using GroveGames.DeveloperTools;
using UnityEngine;

namespace GroveGames.Platform.Unity
{
    public sealed class PlatformDeveloperOptions : IDeveloperOptions, IDisposable
    {
        [DeveloperOption(Header = "Time", Min = 0f, Max = 10f)] private float _timeScale;

        public string Category => "Platform";

        public PlatformDeveloperOptions()
        {
            _timeScale = 1f;
        }

        public void Dispose()
        {
            Time.timeScale = 1f;
        }

        private void OnValidate()
        {
            Time.timeScale = _timeScale;
        }
    }
}
