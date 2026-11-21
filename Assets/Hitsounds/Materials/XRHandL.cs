using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Mirror;

public enum HandTypeLeft
{
    Left
}

public class XRHandL : NetworkBehaviour
{
    [Header("Hand")]
    public HandTypeLeft handType;

    [Header("Animation Speed")]
    public float fingerMoveSpeed = 10f;

    private Animator animator;
    private InputDevice inputDevice;

    private float pose4Value;
    private float pose5Value;
    private float pose6Value;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (isOwned)
            inputDevice = GetInputDevice();
    }

    void Update()
    {
        if (!isOwned)
            return;

        if (!inputDevice.isValid)
        {
            inputDevice = GetInputDevice();

            if (!inputDevice.isValid)
                return;
        }

        AnimateHand();
    }

    InputDevice GetInputDevice()
    {
        InputDeviceCharacteristics characteristics =
            InputDeviceCharacteristics.HeldInHand |
            InputDeviceCharacteristics.Controller |
            InputDeviceCharacteristics.Left;

        List<InputDevice> devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(
            characteristics,
            devices
        );

        if (devices.Count > 0)
            return devices[0];

        return default;
    }

    void AnimateHand()
    {
        // Trigger
        inputDevice.TryGetFeatureValue(
            CommonUsages.trigger,
            out float triggerValue
        );

        // Grip
        inputDevice.TryGetFeatureValue(
            CommonUsages.grip,
            out float gripValue
        );

        // Thumb
        inputDevice.TryGetFeatureValue(
            CommonUsages.primaryTouch,
            out bool primaryTouched
        );

        inputDevice.TryGetFeatureValue(
            CommonUsages.secondaryTouch,
            out bool secondaryTouched
        );

        bool thumbTouched = primaryTouched || secondaryTouched;

        float thumbTarget = thumbTouched ? 1f : 0f;

        // ALL fingers use the same movement speed.
        pose4Value = Mathf.MoveTowards(
            pose4Value,
            triggerValue,
            fingerMoveSpeed * Time.deltaTime
        );

        pose5Value = Mathf.MoveTowards(
            pose5Value,
            gripValue,
            fingerMoveSpeed * Time.deltaTime
        );

        pose6Value = Mathf.MoveTowards(
            pose6Value,
            thumbTarget,
            fingerMoveSpeed * Time.deltaTime
        );

        animator.SetFloat("pose4", pose4Value);
        animator.SetFloat("pose5", pose5Value);
        animator.SetFloat("pose6", pose6Value);
    }
}