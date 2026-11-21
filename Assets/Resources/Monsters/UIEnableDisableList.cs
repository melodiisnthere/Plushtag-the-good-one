using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Optional, but good if you want to reference UI components directly

public class UIEnableDisableList : MonoBehaviour
{
    public List<GameObject> enabledObjects;
    public List<GameObject> disabledObjects;

    /// <summary>
    /// Call this method from a UI Button's OnClick() event.
    /// </summary>
    public void ToggleObjects()
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