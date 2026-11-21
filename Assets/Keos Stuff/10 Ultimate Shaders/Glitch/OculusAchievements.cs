using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Platform;
using Oculus.Platform.Models;

public class OculusAchievements : MonoBehaviour
{

    [Header("Made by Glitched Cat Studios, Please give credits!")]
    [Space]

    public string ApiName = "API NAME HERE";
    public AchievementType AchievementType;
    public WhenToCall whenToCall;

    [Header("For OnTriggerEnter")]
    public string TriggerTag = "HandTag";

    [Header("For Count Achievements")]
    public ulong addCount = 1;

    private void Start()
    {
        if (AchievementType == AchievementType.Simple)
        {
            Achievements.Unlock(ApiName);
        }
        else
        {
            Achievements.AddCount(ApiName, addCount);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == TriggerTag)
        {
            if (AchievementType == AchievementType.Simple)
            {
                Achievements.Unlock(ApiName);
            }
            else
            {
                Achievements.AddCount(ApiName, addCount);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == TriggerTag)
        {
            if (AchievementType == AchievementType.Simple)
            {
                Achievements.Unlock(ApiName);
            }
            else
            {
                Achievements.AddCount(ApiName, addCount);
            }
        }
    }
}

public enum AchievementType
{
    Simple,
    Count
}

public enum WhenToCall
{
    Start,
    OnTriggerEnter,
    OnTriggerExit,
}

//This Script is made by Glitched Cat Studios!
//You may distribute this script but LEAVE THIS WATERMARK IN THIS SCRIPT!
//Thanks, The Glitched Cat Studios Team.