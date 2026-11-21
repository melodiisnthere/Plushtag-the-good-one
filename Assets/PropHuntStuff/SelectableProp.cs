using UnityEngine;

public class SelectableProp : MonoBehaviour
{
    [Header("Prop Configuration")]
    [Tooltip("The index corresponding to this prop prefab in the PropDatabase.")]
    public int propIndex;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is a player
        PropHuntPlayer player = other.GetComponentInParent<PropHuntPlayer>();
        
        if (player != null && player.isLocalPlayer && !player.isSeeker)
        {
            // Request prop transformation from the server
            player.CmdTransformIntoProp(propIndex);
        }
    }
}