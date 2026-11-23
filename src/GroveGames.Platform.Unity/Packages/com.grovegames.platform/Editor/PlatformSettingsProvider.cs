using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GroveGames.Platform.Unity.Editor
{
    internal static class PlatformSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            return new SettingsProvider("Project/GroveGames/Platform", SettingsScope.Project)
            {
                label = "Platform",
                activateHandler = (searchContext, rootElement) =>
                {
                    var settings = PlatformSettingsAsset.GetOrCreate();
                    var serializedObject = new SerializedObject(settings);

                    var container = new VisualElement
                    {
                        style =
                        {
                            paddingLeft = 10,
                            paddingRight = 10,
                            paddingTop = 10,
                            paddingBottom = 10
                        }
                    };

                    container.Add(new Label("Platform Settings")
                    {
                        style =
                        {
                            fontSize = 19,
                            unityFontStyleAndWeight = FontStyle.Bold,
                            marginBottom = 10
                        }
                    });

                    var assetField = new ObjectField("Settings Asset")
                    {
                        objectType = typeof(PlatformSettings),
                        value = settings,
                        style = { marginBottom = 10 }
                    };
                    assetField.SetEnabled(false);
                    container.Add(assetField);

                    container.Add(new PropertyField(serializedObject.FindProperty("_targetFrameRate"), "Target Frame Rate"));

                    var modeProperty = serializedObject.FindProperty("_sleepTimeoutMode");
                    var modeField = new PropertyField(modeProperty, "Sleep Timeout");
                    var secondsField = new PropertyField(serializedObject.FindProperty("_sleepTimeoutSeconds"), "Sleep Timeout Seconds");
                    secondsField.style.display = GetSecondsDisplay(modeProperty);
                    modeField.RegisterValueChangeCallback(changeEvent => secondsField.style.display = GetSecondsDisplay(changeEvent.changedProperty));
                    container.Add(modeField);
                    container.Add(secondsField);

                    rootElement.Add(container);
                    rootElement.Bind(serializedObject);
                },
                keywords = new HashSet<string>(new[] { "Platform", "Frame Rate", "Sleep", "Timeout", "Screen", "Grove Games" })
            };
        }

        private static DisplayStyle GetSecondsDisplay(SerializedProperty modeProperty)
        {
            return modeProperty.enumValueIndex == (int)SleepTimeoutMode.Timed ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
