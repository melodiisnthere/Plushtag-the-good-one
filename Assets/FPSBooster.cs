using UnityEngine;
using UnityEngine.Rendering;

public class FPSBooster : MonoBehaviour
{
    [Header("?? Auto Optimization Settings")]
    public bool autoOptimizeOnStart = true;
    public bool disableRealtimeLights = true;
    public bool disableShadows = true;
    public bool disableVSync = true;
    public bool lowerPhysicsQuality = true;
    public bool disablePostProcessing = true;
    public bool disableReflections = true;
    public bool disableUnusedAudioListeners = true;
    public bool limitParticles = true;

    [Header("?? Runtime Settings")]
    [Tooltip("Target frame rate (set to -1 for unlimited)")]
    public int targetFPS = 90;

    private void Start()
    {
        if (autoOptimizeOnStart)
            OptimizeForVR();
    }

    public void OptimizeForVR()
    {
        Debug.Log("[FPSBooster] Applying VR optimizations...");

        // ?? Frame rate
        Application.targetFrameRate = targetFPS;
        QualitySettings.vSyncCount = disableVSync ? 0 : 1;

        // ?? Lighting
        if (disableRealtimeLights)
        {
            foreach (Light light in FindObjectsOfType<Light>())
            {
                if (light.type == LightType.Point || light.type == LightType.Spot)
                    light.enabled = false;
            }
        }

        if (disableShadows)
        {
            QualitySettings.shadows = ShadowQuality.Disable;
        }

        // ?? Reflections
        if (disableReflections)
        {
            RenderSettings.reflectionIntensity = 0f;
            RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
        }

        // ?? Post-processing (safe check)
        if (disablePostProcessing)
        {
#if UNITY_POST_PROCESSING_STACK_V2
            foreach (var volume in FindObjectsOfType<UnityEngine.Rendering.PostProcessing.PostProcessVolume>())
                volume.enabled = false;
#endif
            foreach (var volume in FindObjectsOfType<Volume>())
                volume.enabled = false;
        }

        // ?? Disable extra audio listeners
        if (disableUnusedAudioListeners)
        {
            AudioListener[] listeners = FindObjectsOfType<AudioListener>();
            if (listeners.Length > 1)
            {
                for (int i = 1; i < listeners.Length; i++)
                    listeners[i].enabled = false;
            }
        }

        // ?? Particle limits
        if (limitParticles)
        {
            foreach (var ps in FindObjectsOfType<ParticleSystem>())
            {
                var main = ps.main;
                main.maxParticles = Mathf.Min(main.maxParticles, 100);
            }
        }

        // ?? Physics simplification
        if (lowerPhysicsQuality)
        {
            Physics.defaultSolverIterations = 4;
            Physics.defaultSolverVelocityIterations = 2;
            Time.fixedDeltaTime = 1f / 60f; // smoother physics for VR
        }

        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        Debug.Log("[FPSBooster] Optimization complete ?");
    }
}
