using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class VersionChecker : MonoBehaviour
{
    [Header("When Version is VALID")]
    public GameObject[] enableIfValid;
    public GameObject[] disableIfValid;

    [Header("When Version is OUTDATED")]
    public GameObject[] enableIfInvalid;
    public GameObject[] disableIfInvalid;

    private class Result
    {
        public bool result;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => PlayFabClientAPI.IsClientLoggedIn());

        var request = new ExecuteCloudScriptRequest()
        {
            FunctionName = "GcsVersionChecker",
            FunctionParameter = new Dictionary<string, string>
            {
                { "version", Application.version }
            },
            GeneratePlayStreamEvent = false
        };

        PlayFabClientAPI.ExecuteCloudScript(request,
        result =>
        {
            var json = result.FunctionResult.ToString();
            Result data = JsonUtility.FromJson<Result>(json);

            if (data.result)
            {
                Debug.Log("✅ Version is valid.");
                ApplyGameObjectToggles(enableIfValid, disableIfValid);
            }
            else
            {
                Debug.LogWarning("❌ Version is outdated.");
                ApplyGameObjectToggles(enableIfInvalid, disableIfInvalid);
            }
        },
        error =>
        {
            Debug.LogError("Error during version check: " + error.GenerateErrorReport());
            // Optional: fallback to outdated behavior
            ApplyGameObjectToggles(enableIfInvalid, disableIfInvalid);
        });
    }

    private void ApplyGameObjectToggles(GameObject[] enableList, GameObject[] disableList)
    {
        foreach (var go in enableList)
        {
            if (go != null) go.SetActive(true);
        }
        foreach (var go in disableList)
        {
            if (go != null) go.SetActive(false);
        }
    }
}
