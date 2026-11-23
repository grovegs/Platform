using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GroveGames.Platform.Unity.Editor
{
    internal static class PlatformSettingsAsset
    {
        public const string AssetPath = "Assets/Settings/GroveGames/PlatformSettings.asset";

        public static PlatformSettings GetOrCreate()
        {
            if (!EditorBuildSettings.TryGetConfigObject(PlatformSettings.ConfigName, out PlatformSettings settings) || settings == null)
            {
                settings = LoadOrCreate();
                EditorBuildSettings.AddConfigObject(PlatformSettings.ConfigName, settings, true);
            }

            Preload(settings);
            return settings;
        }

        private static PlatformSettings LoadOrCreate()
        {
            CreateFolder();

            var settings = AssetDatabase.LoadAssetAtPath<PlatformSettings>(AssetPath);

            if (settings != null)
            {
                return settings;
            }

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<PlatformSettings>(), AssetPath);
            AssetDatabase.SaveAssets();
            return AssetDatabase.LoadAssetAtPath<PlatformSettings>(AssetPath);
        }

        private static void Preload(PlatformSettings settings)
        {
            var preloaded = PlayerSettings.GetPreloadedAssets();
            var assets = new List<Object>(preloaded.Length + 1);
            var isPreloaded = false;

            for (var i = 0; i < preloaded.Length; i++)
            {
                var asset = preloaded[i];

                if (asset is PlatformSettings && asset != settings)
                {
                    continue;
                }

                isPreloaded |= asset == settings;
                assets.Add(asset);
            }

            if (!isPreloaded)
            {
                assets.Add(settings);
            }

            if (isPreloaded && assets.Count == preloaded.Length)
            {
                return;
            }

            PlayerSettings.SetPreloadedAssets(assets.ToArray());
        }

        private static void CreateFolder()
        {
            var current = "Assets";

            foreach (var part in AssetPath.Substring("Assets/".Length).Split('/'))
            {
                if (part.EndsWith(".asset", StringComparison.Ordinal))
                {
                    return;
                }

                var next = current + "/" + part;

                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, part);
                }

                current = next;
            }
        }
    }
}
