using UnityEngine;

public class PropItem : MonoBehaviour
{
    /// <summary>
    /// Set by PropHuntVRInput. When false, walking into a prop does nothing and hiders
    /// must aim at it and press the disguise button. Defaults to true so the old
    /// touch-to-copy behaviour still works if PropHuntVRInput is not in the scene.
    /// </summary>
    public static bool TouchToDisguise = true;

    [Header("Prop Configuration")]
    [Tooltip("The index of this prop inside the PropDatabase prefab list.")]
    public int propIndex = 0;

    /// <summary>
    /// Call this method when a local player touches, clicks, aims at, or interacts with this prop.
    /// </summary>
    public void InteractWithProp(PropHuntPlayer player)
    {
        if (player == null || !player.isLocalPlayer) return;

        // Ensure transformation only works for Hiders
        if (player.isSeeker) return;

        // Send Command to server to sync the prop transformation to all clients
        player.CmdTransformIntoProp(propIndex);
    }

    // Optional: Standard Unity Trigger / Collision interaction
    private void OnTriggerEnter(Collider other)
    {
        if (!TouchToDisguise) return;

        PropHuntPlayer player = other.GetComponentInParent<PropHuntPlayer>();
        if (player != null && player.isLocalPlayer)
        {
            InteractWithProp(player);
        }
    }
}
