using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR;
using Mirror;

public enum Hand
{
    Left,
    Right
}

public class FingerMovement : MonoBehaviour
{
    [Header("Very Sigma Finger Movement Script, For Mirror VR ofc")]
    [Header("")]
    public Animator animator;
    public Hand Hand;
    public float ThumbSpeed = 0.1f;
    private InputDevice inputDevice;
    public NetworkIdentity networkIdentity;
    private float TriggerValue;
    private float GripValue;
    private float FaceButtonValue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        if (networkIdentity.isLocalPlayer)
        {
            if(!inputDevice.isValid)
            {
                inputDevice = GetInputDevice();
            }
            else
            {
                AnimateHand();
            }
        }
    }

    InputDevice GetInputDevice()
    {
        InputDeviceCharacteristics controllerCharacteristic = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller;

        if (Hand == Hand.Left)
        {
            controllerCharacteristic |= InputDeviceCharacteristics.Left;
        }
        else
        {
            controllerCharacteristic |= InputDeviceCharacteristics.Right;
        }

        List<InputDevice> inputDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristic, inputDevices);

        if (inputDevices.Count > 0)
        {
            return inputDevices[0];
        }

        return new InputDevice();
    }

    void AnimateHand()
    {
        inputDevice.TryGetFeatureValue(CommonUsages.trigger, out TriggerValue);
        inputDevice.TryGetFeatureValue(CommonUsages.grip, out GripValue);
        inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool FaceButton);

        if (FaceButton)
        {
            FaceButtonValue += ThumbSpeed;
        }
        else
        {
            FaceButtonValue -= ThumbSpeed;
        }

        FaceButtonValue = Mathf.Clamp(FaceButtonValue, 0, 1);

        if (Hand == Hand.Left)
        {
            animator.SetFloat("LeftTrigger", TriggerValue);
            animator.SetFloat("LeftGrip", GripValue);
            animator.SetFloat("LeftFaceButton", FaceButtonValue);

        }
        else
        {
            animator.SetFloat("RightTrigger", TriggerValue);
            animator.SetFloat("RightGrip", GripValue);
            animator.SetFloat("RightFaceButton", FaceButtonValue);
        }
    }
}
