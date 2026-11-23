using UnityEngine.InputSystem;

namespace GroveGames.Platform.Unity
{
    internal sealed class UnityDisplayDevice : IDisplayDevice
    {
        public bool HasInput
        {
            get
            {
                var devices = InputSystem.devices;

                for (var i = 0; i < devices.Count; i++)
                {
                    if (devices[i].wasUpdatedThisFrame)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public int TargetFrameRate => UnityEngine.Application.targetFrameRate;

        public void SetTargetFrameRate(int frameRate)
        {
            UnityEngine.Application.targetFrameRate = frameRate;
        }

        public void SetKeepScreenOn(bool keepScreenOn)
        {
            UnityEngine.Screen.sleepTimeout = keepScreenOn ? UnityEngine.SleepTimeout.NeverSleep : UnityEngine.SleepTimeout.SystemSetting;
        }
    }
}
