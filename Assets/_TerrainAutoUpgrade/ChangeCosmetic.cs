using UnityEngine;
using Mirror;
using Mirror.VR;
public class ChangeCosmetic : MonoBehaviour
{
    public string CosmeticName;
    public enum Slot { Tag, Head, Face, Body, LeftHand, RightHand }
    public Slot slot;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HandTag"))
        {
            MirrorVRManager.SetCosmetic(slot.ToString(), CosmeticName);
        }
    }
}
