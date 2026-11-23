using System;
using UnityEngine;

namespace GroveGames.Platform.Unity
{
    internal sealed class UnityApplicationDevice : MonoBehaviour, IApplicationDevice
    {
        public event Action<bool>? PauseChanged;
        public event Action<bool>? FocusChanged;

        public bool IsFocused => UnityEngine.Application.isFocused;

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }

        private void OnApplicationPause(bool isPaused)
        {
            PauseChanged?.Invoke(isPaused);
        }

        private void OnApplicationFocus(bool isFocused)
        {
            FocusChanged?.Invoke(isFocused);
        }
    }
}
