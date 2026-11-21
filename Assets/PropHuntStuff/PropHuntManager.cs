using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public enum PropHuntPhase
{
    WaitingForPlayers,
    IntermissionPhase,
    HidingPhase,
    SeekingPhase,
    PostRoundPhase
}

public enum PropHuntTeam { None, Hiders, Seekers }
public enum ForcedRole { None, ForceHunter, ForceSurvivor }

public class PropHuntManager : NetworkBehaviour
{
    public static PropHuntManager Instance { get; private set; }

    [Header("VR Rig Reference")]
    [Tooltip("Your scene's GorillaPlayer / VR rig, used for local teleporting.")]
    public GameObject gorillaPlayerObject;

    [Header("Round Configuration")]
    [Min(2)] public int minPlayers = 2;
    public float intermissionTime = 10f;
    public float hidingTime = 30f;
    public float seekingTime = 240f;
    public float postRoundWaitTime = 10f;
    [Tooltip("How often (seconds) win conditions are re-checked while seeking.")]
    public float winCheckInterval = 0.5f;

    [Header("Debug & Testing Tools")]
    public bool freezeTimer = false;
    public ForcedRole forceNextRole = ForcedRole.None;
    [Tooltip("Player index to force a role onto (0 = Host, 1 = Client 1, ...).")]
    public int forcedPlayerIndex = 0;

    [Header("Map GameObject Folders")]
    public GameObject gameMapObject;
    public GameObject lobbyMapObject;

    [Header("Synced State")]
    [SyncVar(hook = nameof(OnPhaseChanged))]
    public PropHuntPhase currentPhase = PropHuntPhase.WaitingForPlayers;
    // Whole seconds only, so it syncs once per second instead of every frame.
    [SyncVar] public float timeRemaining;
    [SyncVar(hook = nameof(OnWinningTeamChanged))]
    public PropHuntTeam winningTeam = PropHuntTeam.None;

    [Header("Team Object Toggles (active during match only)")]
    public List<GameObject> survivorToggleStuff = new List<GameObject>();
    public List<GameObject> hunterToggleStuff = new List<GameObject>();

    [Header("UI Win Screens")]
    public GameObject seekersWinUI;
    public GameObject hidersWinUI;

    [Header("Teleport Spawns")]
    public Transform lobbySpawnPoint;
    public Transform[] gameMapSpawnPoints;

    private Coroutine gameLoop;
    private Coroutine rigReenable;

    #region Lifecycle

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public override void OnStartServer()
    {
        currentPhase = PropHuntPhase.WaitingForPlayers;
        winningTeam = PropHuntTeam.None;
        timeRemaining = 0;
        gameLoop = null;
    }

    public override void OnStopServer()
    {
        if (gameLoop != null) StopCoroutine(gameLoop);
        gameLoop = null;
    }

    public override void OnStartClient()
    {
        // Late joiners: apply current synced state (hooks may not cover every case).
        ApplyPhaseVisuals(currentPhase);
        ApplyWinUI(winningTeam);
    }

    [ServerCallback]
    private void Update()
    {
        if (gameLoop == null
            && currentPhase == PropHuntPhase.WaitingForPlayers
            && GetPlayers().Length >= minPlayers)
        {
            gameLoop = StartCoroutine(GameLoopRoutine());
        }
    }

    #endregion

    #region Game Loop

    private static PropHuntPlayer[] GetPlayers()
    {
        return FindObjectsOfType<PropHuntPlayer>();
    }

    /// <summary>Counts down a phase. Returns false from the callback to abort early.</summary>
    [Server]
    private IEnumerator RunTimer(float duration, System.Func<bool> shouldAbort = null)
    {
        float t = duration;
        float nextCheck = 0f;
        timeRemaining = Mathf.Ceil(t);

        while (t > 0f)
        {
            if (shouldAbort != null && Time.time >= nextCheck)
            {
                nextCheck = Time.time + winCheckInterval;
                if (shouldAbort()) yield break;
            }

            if (!freezeTimer) t -= Time.deltaTime;

            float whole = Mathf.Ceil(Mathf.Max(t, 0f));
            if (whole != timeRemaining) timeRemaining = whole;

            yield return null;
        }
        timeRemaining = 0;
    }

    [Server]
    private IEnumerator GameLoopRoutine()
    {
        // 1. Intermission - abort back to waiting if players leave.
        currentPhase = PropHuntPhase.IntermissionPhase;
        bool notEnough = false;
        yield return RunTimer(intermissionTime, () =>
        {
            notEnough = GetPlayers().Length < minPlayers;
            return notEnough;
        });

        if (notEnough)
        {
            currentPhase = PropHuntPhase.WaitingForPlayers;
            gameLoop = null;
            yield break;
        }

        // 2. Hiding
        AssignRolesAndTeleport();
        currentPhase = PropHuntPhase.HidingPhase;
        yield return RunTimer(hidingTime);

        // 3. Seeking - ends early on win condition.
        currentPhase = PropHuntPhase.SeekingPhase;
        bool ended = false;
        yield return RunTimer(seekingTime, () => ended = CheckWinConditions());
        if (ended) yield break;   // EndRound already started post-round

        // 4. Time expired
        EndRound(PropHuntTeam.Hiders);
    }

    [Server]
    public void ServerEliminateHider(NetworkIdentity hiderIdentity)
    {
        if (currentPhase != PropHuntPhase.SeekingPhase || hiderIdentity == null) return;

        PropHuntPlayer player = hiderIdentity.GetComponent<PropHuntPlayer>();
        if (player == null || player.isSeeker) return;

        player.ServerSetRole(true);
        CheckWinConditions();
    }

    [Server]
    private bool CheckWinConditions()
    {
        if (currentPhase != PropHuntPhase.SeekingPhase) return false;

        int hiders = 0, seekers = 0;
        foreach (PropHuntPlayer p in GetPlayers())
        {
            if (p.isSeeker) seekers++;
            else hiders++;
        }

        if (hiders == 0) { EndRound(PropHuntTeam.Seekers); return true; }
        if (seekers == 0) { EndRound(PropHuntTeam.Hiders); return true; }
        return false;
    }

    [Server]
    public void EndRound(PropHuntTeam winner)
    {
        if (currentPhase == PropHuntPhase.PostRoundPhase) return;

        // Make sure the main loop can't keep running alongside post-round.
        if (gameLoop != null) StopCoroutine(gameLoop);

        winningTeam = winner;
        currentPhase = PropHuntPhase.PostRoundPhase;
        gameLoop = StartCoroutine(PostRoundRoutine());
    }

    [Server]
    private IEnumerator PostRoundRoutine()
    {
        yield return RunTimer(postRoundWaitTime);

        winningTeam = PropHuntTeam.None;
        ResetPlayersAndTeleportLobby();

        yield return new WaitForSeconds(1f);
        currentPhase = PropHuntPhase.WaitingForPlayers;
        gameLoop = null;
    }

    #endregion

    #region Roles & Teleporting

    [Server]
    private void AssignRolesAndTeleport()
    {
        PropHuntPlayer[] players = GetPlayers();
        if (players.Length == 0) return;

        int seekerIndex = Random.Range(0, players.Length);

        if (forceNextRole != ForcedRole.None
            && forcedPlayerIndex >= 0 && forcedPlayerIndex < players.Length)
        {
            seekerIndex = forceNextRole == ForcedRole.ForceHunter
                ? forcedPlayerIndex
                : (forcedPlayerIndex + 1) % players.Length;
        }
        forceNextRole = ForcedRole.None;

        for (int i = 0; i < players.Length; i++)
        {
            players[i].ServerSetRole(i == seekerIndex);

            if (gameMapSpawnPoints != null && gameMapSpawnPoints.Length > 0)
            {
                Transform spawn = gameMapSpawnPoints[i % gameMapSpawnPoints.Length];
                players[i].RpcTeleportPlayer(spawn.position, spawn.rotation);
            }
        }
    }

    [Server]
    private void ResetPlayersAndTeleportLobby()
    {
        foreach (PropHuntPlayer player in GetPlayers())
        {
            player.ServerSetRole(false);
            if (lobbySpawnPoint != null)
                player.RpcTeleportPlayer(lobbySpawnPoint.position, lobbySpawnPoint.rotation);
        }
    }

    /// <summary>Instantly swaps map + team objects. Call before moving the player.</summary>
    public void SetMapStateInstant(bool isGameplayMapActive)
    {
        if (gameMapObject != null) gameMapObject.SetActive(isGameplayMapActive);
        if (lobbyMapObject != null) lobbyMapObject.SetActive(!isGameplayMapActive);

        ToggleObjects(survivorToggleStuff, isGameplayMapActive);
        ToggleObjects(hunterToggleStuff, isGameplayMapActive);
    }

    public void TeleportGorillaRig(Vector3 position, Quaternion rotation)
    {
        if (gorillaPlayerObject == null) return;

        Rigidbody rb = gorillaPlayerObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            ZeroVelocity(rb);
        }

        gorillaPlayerObject.transform.SetPositionAndRotation(position, rotation);
        Physics.SyncTransforms();

        if (rb != null)
        {
            if (rigReenable != null) StopCoroutine(rigReenable);
            rigReenable = StartCoroutine(ReenableRigPhysics(rb));
        }
    }

    private IEnumerator ReenableRigPhysics(Rigidbody rb)
    {
        yield return new WaitForFixedUpdate();
        if (rb != null)
        {
            rb.isKinematic = false;
            ZeroVelocity(rb);
        }
        rigReenable = null;
    }

    private static void ZeroVelocity(Rigidbody rb)
    {
#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = Vector3.zero;
#else
        rb.velocity = Vector3.zero;
#endif
        rb.angularVelocity = Vector3.zero;
    }

    #endregion

    #region Client Visuals

    private void OnPhaseChanged(PropHuntPhase oldPhase, PropHuntPhase newPhase)
    {
        ApplyPhaseVisuals(newPhase);
    }

    private void OnWinningTeamChanged(PropHuntTeam oldTeam, PropHuntTeam newTeam)
    {
        ApplyWinUI(newTeam);
    }

    private void ApplyPhaseVisuals(PropHuntPhase phase)
    {
        bool gameplay = phase == PropHuntPhase.HidingPhase
                     || phase == PropHuntPhase.SeekingPhase
                     || phase == PropHuntPhase.PostRoundPhase;
        SetMapStateInstant(gameplay);
    }

    private void ApplyWinUI(PropHuntTeam team)
    {
        if (seekersWinUI != null) seekersWinUI.SetActive(team == PropHuntTeam.Seekers);
        if (hidersWinUI != null) hidersWinUI.SetActive(team == PropHuntTeam.Hiders);
    }

    private static void ToggleObjects(List<GameObject> list, bool active)
    {
        foreach (GameObject go in list)
            if (go != null) go.SetActive(active);
    }

    #endregion
}
