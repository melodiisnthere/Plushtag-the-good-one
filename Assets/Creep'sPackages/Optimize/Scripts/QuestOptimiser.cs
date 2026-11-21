using UnityEngine;
// Made by ThatOneGorilla https://discord.gg/W7WGp8tS6k
public class QuestOptimiser : MonoBehaviour
{
    private void Start() => ApplyOptimizationSettings();
    private void ApplyOptimizationSettings()
    {
        QualitySettings.vSyncCount = 0;
        QualitySettings.shadows = ShadowQuality.Disable;
        QualitySettings.SetQualityLevel(0);
        QualitySettings.lodBias = 1.0f;
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
        QualitySettings.asyncUploadTimeSlice = 1;
        QualitySettings.particleRaycastBudget = 128;

        foreach (var renderer in FindObjectsOfType<Renderer>())
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        Physics.defaultContactOffset = 0.01f;
        Physics.sleepThreshold = 0.005f;

        Time.maximumDeltaTime = 0.02f;

        Application.lowMemory += OnLowMemory;
        Application.backgroundLoadingPriority = ThreadPriority.Low;

        Debug.Log("Optimization settings applied.");
    }
    private void OnLowMemory()
    {
        Resources.UnloadUnusedAssets();
        QualitySettings.SetQualityLevel(1);
        Debug.LogWarning("Low memory detected. Unloading unused assets and reducing quality.");
    }
}
