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

        public static UnityPlatformSettings GetOrCreate()
        {
            if (!EditorBuildSettings.TryGetConfigObject(UnityPlatformSettings.ConfigName, out UnityPlatformSettings settings) || settings == null)
            {
                settings = LoadOrCreate();
                EditorBuildSettings.AddConfigObject(UnityPlatformSettings.ConfigName, settings, true);
            }

            Preload(settings);
            return settings;
        }

        private static UnityPlatformSettings LoadOrCreate()
        {
            CreateFolder();

            var settings = AssetDatabase.LoadAssetAtPath<UnityPlatformSettings>(AssetPath);

            if (settings != null)
            {
                return settings;
            }

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<UnityPlatformSettings>(), AssetPath);
            AssetDatabase.SaveAssets();
            return AssetDatabase.LoadAssetAtPath<UnityPlatformSettings>(AssetPath);
        }

        private static void Preload(UnityPlatformSettings settings)
        {
            var preloaded = PlayerSettings.GetPreloadedAssets();
            var assets = new List<Object>(preloaded.Length + 1);
            var isPreloaded = false;

            for (var i = 0; i < preloaded.Length; i++)
            {
                var asset = preloaded[i];

                if (asset is UnityPlatformSettings && asset != settings)
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
