using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using easyInputs;
public class BetterFlashLight : MonoBehaviour
{
    private bool enabled;
    public EasyHand hand;
    public AudioSource sound = null;
    public Light light;
    void Update()
    {
        if (!enabled)
        {
            if (EasyInputs.GetTriggerButtonDown(hand))
            {
                light.enabled = true;
                enabled = true;
                if (sound != null)
                {
                    sound.Play();
                }
            }
        }
        else
        {
            if (EasyInputs.GetTriggerButtonDown(hand))
            {
                light.enabled = false;
                enabled = false;
                if (sound != null)
                {
                    sound.Play();
                }
            }
        }
    }
}
