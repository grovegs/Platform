using UnityEngine;

namespace GroveGames.Platform.Unity
{
    public sealed class UnityPlatformSettings : ScriptableObject, IPlatformSettings
    {
        public const string ConfigName = "com.grovegames.platform.settings";

        private static UnityPlatformSettings? s_instance;

        [SerializeField, Min(-1)] private int _targetFrameRate = 60;
        [SerializeField] private SleepTimeoutMode _sleepTimeoutMode = SleepTimeoutMode.SystemSetting;
        [SerializeField, Min(0f)] private float _sleepTimeoutSeconds = 60f;

        public int TargetFrameRate => _targetFrameRate;
        public SleepTimeout SleepTimeout => new(_sleepTimeoutMode, _sleepTimeoutSeconds);

        public static UnityPlatformSettings GetOrCreate()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorBuildSettings.TryGetConfigObject(ConfigName, out UnityPlatformSettings settings) && settings != null)
            {
                return settings;
            }
#else
            if (s_instance == null)
            {
                var preloaded = Resources.FindObjectsOfTypeAll<UnityPlatformSettings>();
                s_instance = preloaded.Length > 0 ? preloaded[0] : null;
            }
#endif
            if (s_instance == null)
            {
                s_instance = CreateInstance<UnityPlatformSettings>();
                s_instance.hideFlags = HideFlags.DontSave;
            }

            return s_instance;
        }
    }
}
