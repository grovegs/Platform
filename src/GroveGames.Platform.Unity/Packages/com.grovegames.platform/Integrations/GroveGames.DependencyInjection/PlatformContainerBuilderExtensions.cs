using GroveGames.DependencyInjection;
using GroveGames.DependencyInjection.Unity;
#if GROVEGAMES_DEVELOPERTOOLS
using GroveGames.DeveloperTools.Unity;
#endif

namespace GroveGames.Platform.Unity
{
    public static class PlatformContainerBuilderExtensions
    {
        public static IContainerBuilder AddApplication(this IContainerBuilder builder)
        {
            builder.AddSingleton<IApplicationDevice>(resolver => resolver.Instantiate<UnityApplicationDevice>());
#if GROVEGAMES_DEVELOPERTOOLS
            builder.AddDeveloperOptions<PlatformDeveloperOptions>();
#endif
            return builder.AddSingleton<IApplication, Application>();
        }

        public static IContainerBuilder AddClock(this IContainerBuilder builder)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            builder.AddSingleton<IClockDevice, AndroidClockDevice>();
#elif UNITY_IOS && !UNITY_EDITOR
            builder.AddSingleton<IClockDevice, IosClockDevice>();
#else
            builder.AddSingleton<IClockDevice, SystemClockDevice>();
#endif
#if GROVEGAMES_DATABASE
            builder.AddSingleton<IClockStorage, DatabaseClockStorage>();
#else
            builder.AddSingleton<IClockStorage, PlayerPrefsClockStorage>();
#endif
            return builder.AddSingleton<IClock, Clock>();
        }

        public static IContainerBuilder AddDisplay(this IContainerBuilder builder)
        {
            var settings = UnityPlatformSettings.GetOrCreate();
            builder.AddSingleton(settings);
            builder.AddSingleton<IPlatformSettings>(settings);
            builder.AddSingleton<IDisplayDevice, UnityDisplayDevice>();
            builder.AddSingleton(resolver => new Display(resolver.Resolve<IDisplayDevice>(), settings));
            builder.AddSingleton<IDisplay>(resolver => resolver.Resolve<Display>());
            return builder.AddSingleton<DisplayUpdater>();
        }
    }
}
