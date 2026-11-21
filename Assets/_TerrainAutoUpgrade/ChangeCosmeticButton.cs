using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Mirror.VR;

public class ChangeCosmeticButton : MonoBehaviour
{
    public string CosmeticName;
    public enum Slot { Tag, Head, Face, Body, LeftHand, RightHand }
    public Slot slot;

    private void Start()
    {
        // Automatically find and link the Button component on this GameObject
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnClickCosmetic);
        }
    }

    private void OnClickCosmetic()
    {
        MirrorVRManager.SetCosmetic(slot.ToString(), CosmeticName);
    }
}