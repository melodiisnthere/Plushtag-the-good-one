using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PropHuntPlayer : NetworkBehaviour
{
    [Header("Controls & Customization")]
    [Tooltip("Key used by hiders to freeze/unfreeze in place.")]
    public KeyCode freezeKey = KeyCode.F;

    [Tooltip("Lock prop rotation to upright Y-axis only when transformed.")]
    public bool lockPropRotation = true;

    [Tooltip("Optional offset applied to spawned prop models inside the container.")]
    public Vector3 propPivotOffset = Vector3.zero;

    [Header("Role & Network State")]
    [SyncVar(hook = nameof(OnSeekerStateChanged))] public bool isSeeker = false;
    [SyncVar(hook = nameof(OnFrozenStateChanged))] public bool isFrozen = false;
    [SyncVar(hook = nameof(OnPropMeshChanged))] public int currentPropIndex = -1;

    [Header("VR Rig Visual Elements")]
    [SerializeField] private List<GameObject> normalAvatarVisuals = new List<GameObject>();
    [SerializeField] private List<GameObject> cosmeticsContainers = new List<GameObject>();
    [SerializeField] private List<GameObject> nameTagCanvases = new List<GameObject>();

    [Header("Seeker Setup")]
    [SerializeField] private GameObject seekerGunWeapon;

    [Header("Prop Container Pivot")]
    [SerializeField] private Transform propPivotContainer;
    [SerializeField] private Camera localVrCamera;

    private GameObject activePropInstance;
    private Rigidbody playerRb;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        if (localVrCamera == null) localVrCamera = Camera.main;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        // Block interaction inputs during non-gameplay phases
        if (PropHuntManager.Instance != null &&
           (PropHuntManager.Instance.currentPhase == PropHuntPhase.WaitingForPlayers ||
            PropHuntManager.Instance.currentPhase == PropHuntPhase.IntermissionPhase ||
            PropHuntManager.Instance.currentPhase == PropHuntPhase.PostRoundPhase))
        {
            return;
        }

        // FREEZE INPUT
        if (!isSeeker && Input.GetKeyDown(freezeKey))
        {
            CmdToggleFreeze();
        }

        // LOCK ROTATION (Yaw only for props)
        if (!isSeeker && activePropInstance != null && lockPropRotation && propPivotContainer != null)
        {
            Vector3 currentEuler = propPivotContainer.localEulerAngles;
            propPivotContainer.localRotation = Quaternion.Euler(0f, currentEuler.y, 0f);
        }
    }

    #region Teleportation & Server Calls

    [ClientRpc]
    public void RpcTeleportPlayer(Vector3 targetPosition, Quaternion targetRotation)
    {
        if (isLocalPlayer)
        {
            // Route through Manager if GorillaPlayer object is defined in scene
            if (PropHuntManager.Instance != null && PropHuntManager.Instance.gorillaPlayerObject != null)
            {
                PropHuntManager.Instance.TeleportGorillaRig(targetPosition, targetRotation);
            }
            else
            {
                TeleportLocalTransform(targetPosition, targetRotation);
            }
        }
        else
        {
            // Non-local player representation network sync
            transform.position = targetPosition;
            transform.rotation = targetRotation;
        }
    }

    private void TeleportLocalTransform(Vector3 targetPosition, Quaternion targetRotation)
    {
        if (playerRb != null)
        {
            playerRb.isKinematic = true;
#if UNITY_6000_0_OR_NEWER
            playerRb.linearVelocity = Vector3.zero;
#else
            playerRb.velocity = Vector3.zero;
#endif
            playerRb.angularVelocity = Vector3.zero;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;

        Physics.SyncTransforms();

        StartCoroutine(ReenableLocalPhysics());
    }

    private IEnumerator ReenableLocalPhysics()
    {
        yield return new WaitForFixedUpdate();

        if (playerRb != null && !isFrozen)
        {
            playerRb.isKinematic = false;
#if UNITY_6000_0_OR_NEWER
            playerRb.linearVelocity = Vector3.zero;
#else
            playerRb.velocity = Vector3.zero;
#endif
            playerRb.angularVelocity = Vector3.zero;
        }
    }

    [Server]
    public void ServerSetRole(bool seekerState)
    {
        isSeeker = seekerState;
        isFrozen = false;
        currentPropIndex = -1;
    }

    [Command]
    public void CmdTransformIntoProp(int propIndex)
    {
        if (isSeeker) return;
        if (PropHuntManager.Instance != null && PropHuntManager.Instance.currentPhase != PropHuntPhase.HidingPhase) return;

        currentPropIndex = propIndex;
    }

    [Command]
    public void CmdToggleFreeze()
    {
        if (isSeeker || currentPropIndex == -1) return;
        isFrozen = !isFrozen;
    }

    [Server]
    public void ServerTakeDamage(int damage, NetworkIdentity attacker)
    {
        if (isSeeker) return;

        if (PropHuntManager.Instance != null)
        {
            PropHuntManager.Instance.ServerEliminateHider(netIdentity);
        }
    }

    #endregion

    #region Visual Hooks & Culling

    private void OnSeekerStateChanged(bool oldVal, bool newVal)
    {
        UpdateGunState();
    }

    private void OnPropMeshChanged(int oldIdx, int newIdx)
    {
        if (activePropInstance != null) Destroy(activePropInstance);

        if (newIdx == -1)
        {
            SetBaseVisualsActive(true);
            UpdateGunState();
            return;
        }

        SetBaseVisualsActive(false);
        if (seekerGunWeapon != null) seekerGunWeapon.SetActive(false);

        if (PropDatabase.Instance != null && newIdx >= 0 && newIdx < PropDatabase.Instance.propPrefabs.Count)
        {
            GameObject prefab = PropDatabase.Instance.propPrefabs[newIdx];
            Transform parentTransform = (propPivotContainer != null) ? propPivotContainer : transform;

            activePropInstance = Instantiate(prefab, parentTransform);
            activePropInstance.transform.localPosition = propPivotOffset;

            int hiderLayer = LayerMask.NameToLayer("HiderProp");
            if (hiderLayer != -1) activePropInstance.layer = hiderLayer;

            if (isLocalPlayer)
            {
                int localPlayerLayer = LayerMask.NameToLayer("LocalPlayer");
                if (localPlayerLayer != -1)
                {
                    SetLayerRecursively(activePropInstance, localPlayerLayer);
                    if (localVrCamera != null)
                    {
                        localVrCamera.cullingMask &= ~(1 << localPlayerLayer);
                    }
                }
            }
        }
    }

    private void OnFrozenStateChanged(bool oldVal, bool newVal)
    {
        if (playerRb != null)
        {
            playerRb.constraints = newVal
                ? RigidbodyConstraints.FreezeAll
                : RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    private void UpdateGunState()
    {
        if (seekerGunWeapon != null)
        {
            seekerGunWeapon.SetActive(isSeeker && currentPropIndex == -1);
        }
    }

    private void SetBaseVisualsActive(bool isActive)
    {
        foreach (GameObject avatarPart in normalAvatarVisuals)
            if (avatarPart != null) avatarPart.SetActive(isActive);

        foreach (GameObject cosmeticFolder in cosmeticsContainers)
            if (cosmeticFolder != null) cosmeticFolder.SetActive(isActive);

        foreach (GameObject canvas in nameTagCanvases)
            if (canvas != null) canvas.SetActive(isActive);
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        if (obj == null) return;
        obj.layer = layer;
        foreach (Transform child in obj.transform) SetLayerRecursively(child.gameObject, layer);
    }

    #endregion
}