using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Mirror;

public enum HandType
{
    Left,
    Right
}

public class XRHandController : NetworkBehaviour
{
    [Header("Hand")]
    public HandType handType;

    [Header("Animation Speed")]
    public float fingerMoveSpeed = 10f;

    private Animator animator;
    private InputDevice inputDevice;

    private float pose1Value;
    private float pose2Value;
    private float pose3Value;

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
            InputDeviceCharacteristics.Controller;

        if (handType == HandType.Left)
            characteristics |= InputDeviceCharacteristics.Left;
        else
            characteristics |= InputDeviceCharacteristics.Right;

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
        bool thumbTouched = false;

        inputDevice.TryGetFeatureValue(
            CommonUsages.primaryTouch,
            out bool primaryTouched
        );

        inputDevice.TryGetFeatureValue(
            CommonUsages.secondaryTouch,
            out bool secondaryTouched
        );

        thumbTouched = primaryTouched || secondaryTouched;

        float thumbTarget = thumbTouched ? 1f : 0f;

        // ALL fingers use the same movement speed.
        pose1Value = Mathf.MoveTowards(
            pose1Value,
            triggerValue,
            fingerMoveSpeed * Time.deltaTime
        );

        pose2Value = Mathf.MoveTowards(
            pose2Value,
            gripValue,
            fingerMoveSpeed * Time.deltaTime
        );

        pose3Value = Mathf.MoveTowards(
            pose3Value,
            thumbTarget,
            fingerMoveSpeed * Time.deltaTime
        );

        animator.SetFloat("pose1", pose1Value);
        animator.SetFloat("pose2", pose2Value);
        animator.SetFloat("pose3", pose3Value);
    }
}