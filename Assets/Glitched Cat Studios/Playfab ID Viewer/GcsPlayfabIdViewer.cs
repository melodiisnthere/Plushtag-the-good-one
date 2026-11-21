using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;

public class GcsPlayfabIdViewer : MonoBehaviour
{
    public TextMeshPro IdText;
    [Range(1, 16)] public int charactersToShow = 16;

    private void Start()
    {
        enabled = false;
    }

    private void OnEnable()
    {
        string playFabId = PlayFabSettings.staticPlayer.PlayFabId;
        IdText.text = playFabId.Substring(0, Mathf.Min(charactersToShow, playFabId.Length));
    }

    private void OnDisable()
    {
        IdText.text = "HIDDEN";
    }
}
