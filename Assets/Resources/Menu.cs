using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class Menu : MonoBehaviour
{
    [Header("This Script Was Made By Pear :) ")]
    public GameObject thing;

    [Header("Settings")]
    public float cooldown = 0.5f;          // seconds between toggles
    public bool alsoUseXButton = false;    // set true to also toggle with X

    private float lastToggleTime = -999f;
    private bool menuWasPressed = false;
    private bool xWasPressed = false;
    private InputDevice leftHand;

    private void TryGetLeftHand()
    {
        if (leftHand.isValid) return;

        var devices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, devices);
        if (devices.Count > 0) leftHand = devices[0];
    }

    private void Update()
    {
        TryGetLeftHand();
        if (!leftHand.isValid || thing == null) return;

        // Left controller menu button (Quest)
        leftHand.TryGetFeatureValue(CommonUsages.menuButton, out bool menuPressed);
        bool menuDown = menuPressed && !menuWasPressed;
        menuWasPressed = menuPressed;

        // Optional X button
        bool xDown = false;
        if (alsoUseXButton)
        {
            leftHand.TryGetFeatureValue(CommonUsages.primaryButton, out bool xPressed);
            xDown = xPressed && !xWasPressed;
            xWasPressed = xPressed;
        }

        if ((menuDown || xDown) && Time.unscaledTime - lastToggleTime >= cooldown)
        {
            lastToggleTime = Time.unscaledTime;
            thing.SetActive(!thing.activeSelf);
        }
    }
}
