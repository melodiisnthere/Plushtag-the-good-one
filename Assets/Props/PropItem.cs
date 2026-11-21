using UnityEngine;

public class PropItem : MonoBehaviour
{
    [Header("Prop Configuration")]
    [Tooltip("The index of this prop inside the PropDatabase prefab list.")]
    public int propIndex = 0;

    /// <summary>
    /// Call this method when a local player touches, clicks, or interacts with this prop.
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
        PropHuntPlayer player = other.GetComponentInParent<PropHuntPlayer>();
        if (player != null && player.isLocalPlayer)
        {
            InteractWithProp(player);
        }
    }
}