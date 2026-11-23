using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace GroveGames.Platform.Unity.Editor
{
    internal sealed class PlatformSettingsBuildProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            PlatformSettingsAsset.GetOrCreate();
        }
    }
}
