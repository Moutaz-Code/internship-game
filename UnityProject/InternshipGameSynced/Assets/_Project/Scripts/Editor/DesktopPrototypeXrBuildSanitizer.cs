using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.XR.Management;
using UnityEngine;
using UnityEngine.XR.OpenXR;

namespace InternshipGame.Prototype.Editor
{
    /// <summary>
    /// Keeps a non-XR Windows prototype build from serializing desktop-only OpenXR feature
    /// objects. The feature list is changed in memory for the build only and restored
    /// immediately afterward, so enabling a Standalone XR loader restores the normal
    /// headset build path without another code change.
    /// </summary>
    internal sealed class DesktopPrototypeXrBuildSanitizer : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private const int BuildOrder = 10000;

        private static readonly List<Object> SavedFeatures = new List<Object>();
        private static OpenXRSettings sanitizedSettings;
        private static bool settingsWereDirty;

        public int callbackOrder => BuildOrder;

        public void OnPreprocessBuild(BuildReport report)
        {
            RestoreFeatureList();

            if (report.summary.platformGroup != BuildTargetGroup.Standalone || HasActiveStandaloneXrLoader())
            {
                return;
            }

            var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Standalone);
            if (settings == null)
            {
                return;
            }

            var serializedSettings = new SerializedObject(settings);
            var featureList = serializedSettings.FindProperty("features");
            if (featureList == null || featureList.arraySize == 0)
            {
                return;
            }

            settingsWereDirty = EditorUtility.IsDirty(settings);
            for (var index = 0; index < featureList.arraySize; index++)
            {
                SavedFeatures.Add(featureList.GetArrayElementAtIndex(index).objectReferenceValue);
            }

            sanitizedSettings = settings;
            featureList.arraySize = 0;
            serializedSettings.ApplyModifiedPropertiesWithoutUndo();

            // A failed build may skip some post-process callbacks. The first editor
            // update after the synchronous build is therefore a second restoration path.
            EditorApplication.delayCall -= RestoreFeatureList;
            EditorApplication.delayCall += RestoreFeatureList;

            Debug.Log($"Desktop prototype build: temporarily excluded {SavedFeatures.Count} OpenXR feature objects.");
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            RestoreFeatureList();
        }

        private static bool HasActiveStandaloneXrLoader()
        {
            var generalSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Standalone);
            return generalSettings != null
                && generalSettings.Manager != null
                && generalSettings.Manager.activeLoaders.Count > 0;
        }

        private static void RestoreFeatureList()
        {
            EditorApplication.delayCall -= RestoreFeatureList;

            if (sanitizedSettings == null)
            {
                SavedFeatures.Clear();
                return;
            }

            var serializedSettings = new SerializedObject(sanitizedSettings);
            var featureList = serializedSettings.FindProperty("features");
            if (featureList != null)
            {
                featureList.arraySize = SavedFeatures.Count;
                for (var index = 0; index < SavedFeatures.Count; index++)
                {
                    featureList.GetArrayElementAtIndex(index).objectReferenceValue = SavedFeatures[index];
                }

                serializedSettings.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(sanitizedSettings);
                AssetDatabase.SaveAssetIfDirty(sanitizedSettings);
                if (settingsWereDirty)
                {
                    EditorUtility.SetDirty(sanitizedSettings);
                }
            }

            sanitizedSettings = null;
            settingsWereDirty = false;
            SavedFeatures.Clear();
        }
    }
}
