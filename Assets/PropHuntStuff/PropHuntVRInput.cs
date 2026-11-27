using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Local-player input for Prop Hunt in VR. Add this to the same Player prefab that has PropHuntPlayer.
///
///   Hiders:  aim at a prop and press the DISGUISE button to copy it (hiding phase only).
///            Press the FREEZE button to freeze / unfreeze in place.
///   Seekers: press the SHOOT button to fire the SeekerWeapon (seeking phase only).
///
/// Aiming uses the headset camera by default, or a controller transform if you assign "Aim Origin".
/// Props must have a PropItem component (with the right Prop Index) and a collider.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(PropHuntPlayer))]
public class PropHuntVRInput : MonoBehaviour
{
    public enum Hand { Left, Right }
    public enum PadButton { Trigger, Grip, PrimaryFace, SecondaryFace }   // Primary = A/X, Secondary = B/Y

    [Header("Hider: aim and copy")]
    public Hand disguiseHand = Hand.Right;
    public PadButton disguiseButton = PadButton.PrimaryFace;
    [Tooltip("Optional. Leave empty to aim with the headset camera. Or drag in a controller transform.")]
    public Transform aimOrigin;
    public float aimRange = 8f;
    public LayerMask aimLayers = ~0;
    [Tooltip("If on, walking into a prop also copies it (the old behaviour). Off = aim and press only.")]
    public bool allowTouchDisguise = false;

    [Header("Hider: freeze")]
    public Hand freezeHand = Hand.Right;
    public PadButton freezeButton = PadButton.SecondaryFace;

    [Header("Seeker: shoot")]
    public Hand shootHand = Hand.Right;
    public PadButton shootButton = PadButton.Trigger;
    [Tooltip("Minimum seconds between shots.")]
    public float shootCooldown = 0.5f;

    [Header("Editor testing (no headset)")]
    [Tooltip("E = disguise, left mouse = shoot. Uses the camera's forward direction.")]
    public bool allowKeyboardTesting = true;

    private static readonly List<InputDevice> deviceBuffer = new List<InputDevice>();

    private PropHuntPlayer player;
    private SeekerWeapon weapon;
    private bool disguiseWasDown;
    private bool freezeWasDown;
    private bool shootWasDown;
    private float nextShootTime;

    private void Awake()
    {
        player = GetComponent<PropHuntPlayer>();
        PropItem.TouchToDisguise = allowTouchDisguise;
    }

    private void Update()
    {
        if (player == null || !player.isLocalPlayer) return;

        bool disguisePressed = Edge(disguiseHand, disguiseButton, ref disguiseWasDown);
        bool freezePressed = Edge(freezeHand, freezeButton, ref freezeWasDown);
        bool shootPressed = Edge(shootHand, shootButton, ref shootWasDown);

#if ENABLE_LEGACY_INPUT_MANAGER
        if (allowKeyboardTesting)
        {
            disguisePressed |= Input.GetKeyDown(KeyCode.E);
            shootPressed |= Input.GetMouseButtonDown(0);
        }
#endif

        PropHuntManager manager = PropHuntManager.Instance;
        bool hidingPhase = manager == null || manager.currentPhase == PropHuntPhase.HidingPhase;
        bool seekingPhase = manager == null || manager.currentPhase == PropHuntPhase.SeekingPhase;

        if (player.isSeeker)
        {
            if (shootPressed && seekingPhase) TryShoot();
            return;
        }

        if (disguisePressed && hidingPhase) TryDisguise();

        if (freezePressed && (hidingPhase || seekingPhase) && player.currentPropIndex != -1)
        {
            player.CmdToggleFreeze();
        }
    }

    // ---------------------------------------------------------------- hider

    private void TryDisguise()
    {
        Transform origin = aimOrigin;
        if (origin == null && Camera.main != null) origin = Camera.main.transform;
        if (origin == null) return;

        RaycastHit[] hits = Physics.RaycastAll(
            origin.position, origin.forward, aimRange, aimLayers, QueryTriggerInteraction.Collide);
        if (hits.Length == 0) return;

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        Transform rig = (PropHuntManager.Instance != null && PropHuntManager.Instance.gorillaPlayerObject != null)
            ? PropHuntManager.Instance.gorillaPlayerObject.transform
            : null;

        foreach (RaycastHit hit in hits)
        {
            // Ignore our own body and VR rig so they never block the aim.
            if (hit.collider.GetComponentInParent<PropHuntPlayer>() == player) continue;
            if (rig != null && hit.transform.IsChildOf(rig)) continue;

            PropItem item = hit.collider.GetComponentInParent<PropItem>();
            if (item == null) item = hit.collider.GetComponentInChildren<PropItem>();

            // Invisible trigger zones that are not props should not block the aim.
            if (item == null && hit.collider.isTrigger) continue;

            // The nearest solid thing is either a prop (copy it) or a wall/other object (do nothing).
            if (item != null) item.InteractWithProp(player);
            return;
        }
    }

    // --------------------------------------------------------------- seeker

    private void TryShoot()
    {
        if (Time.time < nextShootTime) return;

        if (weapon == null) weapon = GetComponentInChildren<SeekerWeapon>(true);
        if (weapon == null)
        {
            Debug.LogWarning("[PropHuntVRInput] No SeekerWeapon found under this player.");
            return;
        }

        nextShootTime = Time.time + shootCooldown;
        weapon.Shoot();
    }

    // ---------------------------------------------------------------- input

    private static bool Edge(Hand hand, PadButton button, ref bool wasDown)
    {
        bool down = IsDown(hand, button);
        bool pressed = down && !wasDown;
        wasDown = down;
        return pressed;
    }

    private static bool IsDown(Hand hand, PadButton button)
    {
        InputDeviceCharacteristics characteristics = InputDeviceCharacteristics.Controller
            | (hand == Hand.Left ? InputDeviceCharacteristics.Left : InputDeviceCharacteristics.Right);

        deviceBuffer.Clear();
        InputDevices.GetDevicesWithCharacteristics(characteristics, deviceBuffer);
        if (deviceBuffer.Count == 0) return false;

        InputFeatureUsage<bool> usage;
        switch (button)
        {
            case PadButton.Grip: usage = CommonUsages.gripButton; break;
            case PadButton.PrimaryFace: usage = CommonUsages.primaryButton; break;
            case PadButton.SecondaryFace: usage = CommonUsages.secondaryButton; break;
            default: usage = CommonUsages.triggerButton; break;
        }

        bool value;
        return deviceBuffer[0].TryGetFeatureValue(usage, out value) && value;
    }
}
