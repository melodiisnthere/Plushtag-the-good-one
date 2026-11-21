using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using easyInputs;
public class SlendyShadow : MonoBehaviour
{
    [Header("Settings")]
    public EasyHand Flashlight;
    public float Range;
    [Space(10)]
    public GameObject Gorillaplayer;
    void Update()
    {
        float GorillaDistance = Vector3.Distance(transform.position, Gorillaplayer.transform.position);
        if (EasyInputs.GetTriggerButtonDown(Flashlight) && GorillaDistance < Range)
        {
            this.gameObject.SetActive(false);
        }
    }
}