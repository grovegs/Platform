using System;

namespace GroveGames.Platform.Unity
{
    public static class UnityEnvironment
    {
        public const EnvironmentType Type =
#if UNITY_EDITOR
            EnvironmentType.Editor;
#elif DEVELOPMENT
            EnvironmentType.Development;
#else
            EnvironmentType.Production;
#endif

        public const bool IsEditor = Type == EnvironmentType.Editor;
        public const bool IsDevelopment = Type == EnvironmentType.Development;
        public const bool IsProduction = Type == EnvironmentType.Production;

        public static readonly Environment Current = new(Type, UnityEngine.Application.version);

        public static Version Version => Current.Version;
        public static string VersionName => Current.VersionName;
    }
}
