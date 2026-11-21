using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayfabIdViewerManager : MonoBehaviour
{
    public GcsPlayfabIdViewer idViewer;
    public string HandTag = "HandTag";

    private bool IsEnabled = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == HandTag)
        {
            if (!IsEnabled)
            {
                IsEnabled = true;
                idViewer.enabled = true;
            }
            else
            {
                IsEnabled = false;
                idViewer.enabled = false;
            }
        }
    }
}
