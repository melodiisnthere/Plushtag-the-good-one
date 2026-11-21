using System.Collections.Generic;
using UnityEngine;

public class EnableDisableList : MonoBehaviour
{
    public List<GameObject> enabledObjects;
    public List<GameObject> disabledObjects;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HandTag"))
        {
            ToggleObjects();
        }
    }

    private void ToggleObjects()
    {
        foreach (GameObject obj in enabledObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        foreach (GameObject obj in disabledObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }
}