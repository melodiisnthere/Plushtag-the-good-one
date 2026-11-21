using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGiver : MonoBehaviour
{
    [Header("MADE BY The Kachow")]
    [Header("DO NOT STEAL AND CREDIT ME IN YOUR VIDEO")]
    public int coinsReward;

    private void OnTriggerEnter(Collider other)
    {
        int s = PlayerPrefs.GetInt("coins");
        s += coinsReward;
        PlayerPrefs.SetInt("coins", s);
    }
}
