using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace MarketHustle.Build
{
    /// <summary>
    /// Build setup utilities for iOS and Android deployment.
    /// </summary>
    public class BuildSetup : MonoBehaviour
    {
        [MenuItem("MarketHustle/Build/Setup Android Build")]
        static void SetupAndroidBuild()
        {
            // Set platform to Android
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

            // Player settings for Android
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel21;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;

            // Graphics settings
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.OpenGLES3, GraphicsDeviceType.Vulkan });

            // Other settings
            PlayerSettings.companyName = "MarketHustle";
            PlayerSettings.productName = "Market Hustle";
            PlayerSettings.bundleVersion = "1.0.0";
            PlayerSettings.Android.bundleVersionCode = 1;

            // Enable ARM64 for better performance
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

            Debug.Log("Android build setup complete!");
        }

        [MenuItem("MarketHustle/Build/Setup iOS Build")]
        static void SetupIOSBuild()
        {
            // Set platform to iOS
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);

            // Player settings for iOS
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
            PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneAndiPad;
            PlayerSettings.iOS.targetOSVersionString = "11.0";
            PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;

            // Architecture
            PlayerSettings.SetArchitecture(BuildTarget.iOS, 2); // ARM64

            // Other settings
            PlayerSettings.companyName = "MarketHustle";
            PlayerSettings.productName = "Market Hustle";
            PlayerSettings.bundleVersion = "1.0.0";
            PlayerSettings.iOS.buildNumber = "1";

            Debug.Log("iOS build setup complete!");
        }

        [MenuItem("MarketHustle/Build/Build Android APK")]
        static void BuildAndroidAPK()
        {
            string[] scenes = {
                "Assets/Scenes/Jacamenoville.unity",
                "Assets/Scenes/SupermarketInterior.unity",
                "Assets/Scenes/ApartmentScene.unity",
                "Assets/Scenes/CondoScene.unity",
                "Assets/Scenes/VillaScene.unity",
                "Assets/Scenes/MansionScene.unity"
            };

            string buildPath = "Builds/Android/MarketHustle.apk";

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = scenes;
            buildPlayerOptions.locationPathName = buildPath;
            buildPlayerOptions.target = BuildTarget.Android;
            buildPlayerOptions.options = BuildOptions.None;

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Android build succeeded: " + summary.totalSize + " bytes");
            }
            else if (summary.result == BuildResult.Failed)
            {
                Debug.LogError("Android build failed");
            }
        }

        [MenuItem("MarketHustle/Build/Build iOS Project")]
        static void BuildIOSProject()
        {
            string[] scenes = {
                "Assets/Scenes/Jacamenoville.unity",
                "Assets/Scenes/SupermarketInterior.unity",
                "Assets/Scenes/ApartmentScene.unity",
                "Assets/Scenes/CondoScene.unity",
                "Assets/Scenes/VillaScene.unity",
                "Assets/Scenes/MansionScene.unity"
            };

            string buildPath = "Builds/iOS";

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = scenes;
            buildPlayerOptions.locationPathName = buildPath;
            buildPlayerOptions.target = BuildTarget.iOS;
            buildPlayerOptions.options = BuildOptions.None;

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("iOS build succeeded: " + summary.totalSize + " bytes");
                Debug.Log("Open the Xcode project at: " + buildPath);
            }
            else if (summary.result == BuildResult.Failed)
            {
                Debug.LogError("iOS build failed");
            }
        }

        [MenuItem("MarketHustle/Build/Optimize for Mobile")]
        static void OptimizeForMobile()
        {
            // Quality settings
            QualitySettings.SetQualityLevel(1, true); // Medium quality for mobile

            // Physics settings
            Physics.defaultSolverIterations = 6;
            Physics.defaultSolverVelocityIterations = 2;

            // Rendering settings
            QualitySettings.pixelLightCount = 2;
            QualitySettings.shadowDistance = 20f;
            QualitySettings.shadowCascades = 0;

            // Audio settings
            AudioSettings.SetDSPBufferSize(256);

            Debug.Log("Mobile optimizations applied!");
        }
    }
}