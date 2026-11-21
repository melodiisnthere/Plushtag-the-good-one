using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PromoCodeRewardPair
{
    public string promoCode;
    public GameObject rewardthing;
}

public class PromocodesManager : MonoBehaviour
{
    public List<PromoCodeRewardPair> promoCodeRewardPairs = new List<PromoCodeRewardPair>();

    public Text promoCodeText;
    public void AddPromoCode(string promoCode, GameObject rewardthing)
    {
        foreach (var pair in promoCodeRewardPairs)
        {
            if (pair.promoCode == promoCode)
            {
                Debug.LogWarning("promo code alr exists");
                return;
            }
        }

        promoCodeRewardPairs.Add(new PromoCodeRewardPair { promoCode = promoCode, rewardthing = rewardthing });
    }

    public void UpdatePromoCodeText(string newText)
    {
        promoCodeText.text = newText;
        CheckPromoCode();
    }

    private void CheckPromoCode()
    {
        string inputCode = promoCodeText.text;
        foreach (var pair in promoCodeRewardPairs)
        {
            if (pair.promoCode == inputCode)
            {
                pair.rewardthing.SetActive(true);
                Debug.Log("granted the reward");
                return;
            }
        }
        Debug.LogWarning("invalid code");
    }
}
