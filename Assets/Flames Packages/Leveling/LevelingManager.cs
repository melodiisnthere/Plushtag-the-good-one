using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPManager : MonoBehaviour
{
    //The total XP you have. This isn't reduced when leveling up
    public float XP;

    [Space]
    [Header("DO NOT MANUALLY CHANGE THESE")]
    //How much XP you have in relation to the next level
    public float currentXP;
    //How much XP you need to reach the next level
    public float XPReq;
    //Your current level... duh
    public int Level;

    [Header("Leveling Scaling (You may tweak these to your liking)")]
    //How much the XP Requirement is multiplied by for each level
    //The XP requirement for the 1st level
    public float XPReqChange = 50;
    //If true, will add to the XPRequirement. Otherwise, will multiply to it (aka compounding)
    public bool IsAdditive = true;
    //How much XP is needed to level up from Level 1
    public float BaseXPReq = 100;

    [Header("Displays")]
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshPro levelDisplay;
    [SerializeField] private TextMeshPro xpDisplay;

    [Header("Settings")]
    public bool EarnXPOverTime = true;
    public float XPPerSec = 5;

    void Awake()
    {
        StartCoroutine(RunTick());
        StartCoroutine(UpdXP());
    }

    IEnumerator RunTick()
    {
        yield return new WaitForSeconds(0.1f);

        //Below are tests from before -- Just ignore them
        /* XPReq = Mathf.Round(10 * Mathf.Pow(XPReqMulti, Level - 1)); //Will be so leveling changes with scaling changes
        Level = (int)Mathf.Floor(Mathf.Log((((XPReqMulti - 1) * totalXP) / 10) + 1, 10) / Mathf.Log(XPReqMulti, 10)) + 1; */

        if (!IsAdditive)
        {
            XPReq = BaseXPReq * Mathf.Pow(XPReqChange, Level - 1);
            Level = (int)Mathf.Floor(Mathf.Log(((XP * (XPReqChange - 1)) / BaseXPReq) + 1, 10) / Mathf.Log(XPReqChange, 10)) + 1;
            currentXP = XP - BaseXPReq * (Mathf.Pow(XPReqChange, Level - 1) - 1) / (XPReqChange - 1);
        }
        else
        {
            XPReq = BaseXPReq + (XPReqChange * (Level - 1));

            var a = BaseXPReq - (XPReqChange / 2);
            Level = (int)Mathf.Floor(((-a) + Mathf.Sqrt(Mathf.Pow(a, 2) + (2 * XPReqChange * XP))) / XPReqChange) + 1;

            var l1 = Level - 1;
            var twob = 2 * BaseXPReq;
            var mul = 5 * (Level - 2);
            currentXP = XP - ((l1 * (twob + ((Level - 2) * XPReqChange))) / 2);
        }

        progressBar.fillAmount = currentXP / XPReq;
        levelDisplay.text = "Level: " + Level.ToString();
        xpDisplay.text = "(" + (Mathf.Round(currentXP * 10) / 10).ToString() + "/" + (Mathf.Round(XPReq * 10) / 10).ToString() + ")";

        StartCoroutine(RunTick());
    }

    IEnumerator UpdXP()
    {
        yield return new WaitForSeconds(1);
        if (EarnXPOverTime)
        {
            XP += XPPerSec;
        }
        StartCoroutine(UpdXP());
    }
}