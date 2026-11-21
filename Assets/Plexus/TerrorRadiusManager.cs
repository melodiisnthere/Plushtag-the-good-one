using UnityEngine;

public class TerrorRadiusManager : MonoBehaviour
{
    [Header("Audio Clips (Drop from Assets)")]
    public AudioClip layer1Clip;
    public AudioClip layer2Clip;
    public AudioClip layer3Clip;
    public AudioClip chaseClip;

    [Header("Radius Zones (Getting Closer)")]
    [Range(0f, 100f)] public float layer1Radius = 40f;
    [Range(0f, 100f)] public float layer2Radius = 25f;
    [Range(0f, 100f)] public float layer3Radius = 12f;
    [Range(0f, 100f)] public float chaseStartRadius = 6f;

    [Header("Chase Settings")]
    [Range(0f, 150f)] public float chaseExitRadius = 50f;
    [Range(0.1f, 10f)] public float fadeSpeed = 2f;

    private enum AudioState { PreChase, Chasing }
    private AudioState currentState = AudioState.PreChase;

    private Transform localPlayerTransform;

    private AudioSource layer1Source;
    private AudioSource layer2Source;
    private AudioSource layer3Source;
    private AudioSource chaseSource;

    private enum ActiveLayer { None, Layer1, Layer2, Layer3, Chase }
    private ActiveLayer activeLayer = ActiveLayer.None;

    void Awake()
    {
        layer1Source = SetupInternalAudioSource(layer1Clip);
        layer2Source = SetupInternalAudioSource(layer2Clip);
        layer3Source = SetupInternalAudioSource(layer3Clip);
        chaseSource = SetupInternalAudioSource(chaseClip);

        ValidateRadii();
        MuteAllImmediate();
    }

    void Start()
    {
        TryInitializeLocalPlayer();
        ApplyImmediateAudioStateIfReady();
    }

    void OnEnable()
    {
        // Force state tracking to reset so it correctly identifies changes when turned back on
        activeLayer = ActiveLayer.None;
        currentState = AudioState.PreChase;

        TryInitializeLocalPlayer();
        ApplyImmediateAudioStateIfReady();
    }

    void OnDisable()
    {
        // Stop audio immediately when disabled so sounds don't hang in the background
        MuteAllImmediate();
        if (layer1Source) layer1Source.Stop();
        if (layer2Source) layer2Source.Stop();
        if (layer3Source) layer3Source.Stop();
        if (chaseSource) chaseSource.Stop();
    }

    void Update()
    {
        if (localPlayerTransform == null)
        {
            TryInitializeLocalPlayer();
            if (localPlayerTransform == null) return;
        }

        float distance = CalculateFlatXZDistance();

        // Update chase state
        ManageState(distance);

        // Update audio with restart on layer changes
        UpdateAudioLayersWithRestart(distance);
    }

    void TryInitializeLocalPlayer()
    {
        if (localPlayerTransform != null) return;

        // Standard tag lookup works universally across singleplayer and multiplayer setups
        var go = GameObject.FindWithTag("Player");
        if (go != null) localPlayerTransform = go.transform;
    }

    void ApplyImmediateAudioStateIfReady()
    {
        if (localPlayerTransform == null) return;

        float distance = CalculateFlatXZDistance();

        // Set chase state based on current distance immediately
        ManageState(distance);

        // Determine which layer should be active immediately
        ActiveLayer newActiveLayer = DetermineActiveLayer(distance);

        // If entering for the first time, start correct clip right away.
        activeLayer = ActiveLayer.None; // force "change"
        UpdateAudioLayersWithRestart(distance, forceImmediateStart: true);

        activeLayer = newActiveLayer;
    }

    float CalculateFlatXZDistance()
    {
        Vector3 killerPos = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 playerPos = new Vector3(localPlayerTransform.position.x, 0f, localPlayerTransform.position.z);
        return Vector3.Distance(killerPos, playerPos);
    }

    void ValidateRadii()
    {
        if (layer1Radius <= layer2Radius || layer2Radius <= layer3Radius)
        {
            Debug.LogWarning(
                $"[{name}] Radii ordering invalid. Expected layer1 > layer2 > layer3. " +
                $"Got L1={layer1Radius}, L2={layer2Radius}, L3={layer3Radius}."
            );
        }
    }

    void ManageState(float distance)
    {
        switch (currentState)
        {
            case AudioState.PreChase:
                if (distance <= chaseStartRadius)
                    currentState = AudioState.Chasing;
                break;

            case AudioState.Chasing:
                if (distance > chaseExitRadius)
                    currentState = AudioState.PreChase;
                break;
        }
    }

    void UpdateAudioLayersWithRestart(float distance, bool forceImmediateStart = false)
    {
        ActiveLayer newActiveLayer = DetermineActiveLayer(distance);

        bool shouldRestart = forceImmediateStart || newActiveLayer != activeLayer;

        if (shouldRestart)
        {
            if (newActiveLayer != ActiveLayer.None)
                RestartLayerIfNeeded(newActiveLayer);

            activeLayer = newActiveLayer;
        }

        float targetL1 = 0f, targetL2 = 0f, targetL3 = 0f, targetChase = 0f;

        switch (activeLayer)
        {
            case ActiveLayer.Layer1: targetL1 = 1f; break;
            case ActiveLayer.Layer2: targetL2 = 1f; break;
            case ActiveLayer.Layer3: targetL3 = 1f; break;
            case ActiveLayer.Chase: targetChase = 1f; break;
        }

        float step = fadeSpeed * Time.deltaTime;

        // Forced completely into linear volume transitions via MoveTowards
        if (layer1Source) layer1Source.volume = Mathf.MoveTowards(layer1Source.volume, targetL1, step);
        if (layer2Source) layer2Source.volume = Mathf.MoveTowards(layer2Source.volume, targetL2, step);
        if (layer3Source) layer3Source.volume = Mathf.MoveTowards(layer3Source.volume, targetL3, step);
        if (chaseSource) chaseSource.volume = Mathf.MoveTowards(chaseSource.volume, targetChase, step);
    }

    ActiveLayer DetermineActiveLayer(float distance)
    {
        if (currentState == AudioState.Chasing)
            return ActiveLayer.Chase;

        if (distance <= layer1Radius && distance > layer2Radius)
            return ActiveLayer.Layer1;

        if (distance <= layer2Radius && distance > layer3Radius)
            return ActiveLayer.Layer2;

        if (distance <= layer3Radius)
            return ActiveLayer.Layer3;

        return ActiveLayer.None;
    }

    void RestartLayerIfNeeded(ActiveLayer layer)
    {
        switch (layer)
        {
            case ActiveLayer.Layer1: RestartSource(layer1Source); break;
            case ActiveLayer.Layer2: RestartSource(layer2Source); break;
            case ActiveLayer.Layer3: RestartSource(layer3Source); break;
            case ActiveLayer.Chase: RestartSource(chaseSource); break;
        }

        if (layer1Source && layer != ActiveLayer.Layer1) layer1Source.volume = 0f;
        if (layer2Source && layer != ActiveLayer.Layer2) layer2Source.volume = 0f;
        if (layer3Source && layer != ActiveLayer.Layer3) layer3Source.volume = 0f;
        if (chaseSource && layer != ActiveLayer.Chase) chaseSource.volume = 0f;
    }

    void RestartSource(AudioSource src)
    {
        if (!src) return;

        if (src.clip == null)
        {
            Debug.LogWarning($"[{name}] Missing AudioClip assigned. Cannot restart.", this);
            return;
        }

        src.Stop();
        src.time = 0f;
        src.Play();
    }

    void MuteAllImmediate()
    {
        if (layer1Source) layer1Source.volume = 0f;
        if (layer2Source) layer2Source.volume = 0f;
        if (layer3Source) layer3Source.volume = 0f;
        if (chaseSource) chaseSource.volume = 0f;
    }

    AudioSource SetupInternalAudioSource(AudioClip clip)
    {
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.clip = clip;

        newSource.loop = true;
        newSource.playOnAwake = false;
        newSource.spatialBlend = 0f; // Handled primarily as global 2D through code
        newSource.volume = 0f;

        newSource.rolloffMode = AudioRolloffMode.Linear;
        newSource.minDistance = chaseStartRadius;
        newSource.maxDistance = chaseExitRadius;

        if (clip != null)
            newSource.Stop();
        else
            Debug.LogWarning($"[{name}] Missing AudioClip assigned. AudioSource will be silent.", this);

        return newSource;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, layer1Radius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, layer2Radius);

        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, layer3Radius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseStartRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, chaseExitRadius);
    }
}