using UnityEngine;
using System.Collections;

public class FPSBoosterV2 : MonoBehaviour
{
    [Header("Dynamic Optimization")]
    public int targetFPS = 90;
    public bool adaptiveOptimization = true;

    [Header("Thresholds")]
    public float lowFPSThreshold = 70f;  // below this = drop quality
    public float highFPSThreshold = 100f; // above this = relax optimization
    public float checkInterval = 3f; // seconds between FPS checks

    private float avgFPS = 0f;
    private float frameCounter = 0f;
    private float timer = 0f;
    private bool performanceMode = false;

    private void Start()
    {
        Application.targetFrameRate = targetFPS;
        QualitySettings.vSyncCount = 0;
        StartCoroutine(CheckFPSLoop());
    }

    private IEnumerator CheckFPSLoop()
    {
        while (adaptiveOptimization)
        {
            frameCounter = 0;
            timer = 0;

            // sample FPS for checkInterval seconds
            while (timer < checkInterval)
            {
                frameCounter++;
                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            avgFPS = frameCounter / timer;

            if (!performanceMode && avgFPS < lowFPSThreshold)
            {
                EnablePerformanceMode();
            }
            else if (performanceMode && avgFPS > highFPSThreshold)
            {
                DisablePerformanceMode();
            }
        }
    }

    private void EnablePerformanceMode()
    {
        Debug.Log("[FPSBoosterDynamic] FPS dropped, enabling performance mode...");
        performanceMode = true;

        QualitySettings.shadowDistance = 0f;
        QualitySettings.shadows = ShadowQuality.Disable;
        QualitySettings.antiAliasing = 0;
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
        RenderSettings.reflectionIntensity = 0f;

        foreach (var ps in FindObjectsOfType<ParticleSystem>())
        {
            var main = ps.main;
            main.maxParticles = Mathf.Min(main.maxParticles, 50);
        }

        foreach (var light in FindObjectsOfType<Light>())
        {
            if (light.type != LightType.Directional)
                light.enabled = false;
        }

        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    private void DisablePerformanceMode()
    {
        Debug.Log("[FPSBoosterDynamic] FPS stable, restoring quality...");
        performanceMode = false;

        QualitySettings.shadowDistance = 50f;
        QualitySettings.shadows = ShadowQuality.All;
        QualitySettings.antiAliasing = 2;
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
        RenderSettings.reflectionIntensity = 1f;
    }
}
