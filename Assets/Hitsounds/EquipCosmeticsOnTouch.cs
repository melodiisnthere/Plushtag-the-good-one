using UnityEngine;

public class CosmeticEquipOnTouch : MonoBehaviour
{
    public GameObject cosmeticItem; // The cosmetic to equip
    private bool isEquipped = false;

    void Start()
    {
        if (cosmeticItem != null)
        {
            cosmeticItem.SetActive(false); // Always start with cosmetic off
        }
        isEquipped = false; // Reset equipped state when the game starts
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HandTag") && !isEquipped)
        {
            EquipCosmetic();
        }
    }

    void EquipCosmetic()
    {
        if (cosmeticItem != null)
        {
            cosmeticItem.SetActive(true);
            isEquipped = true;
        }
    }

    public void UnequipCosmetic()
    {
        if (cosmeticItem != null)
        {
            cosmeticItem.SetActive(false);
            isEquipped = false;
        }
    }
}
