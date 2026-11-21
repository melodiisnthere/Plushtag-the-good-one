using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdvancedQuitBox : MonoBehaviour
{
    [Header("MADE BY @Erm.Dev | DONT STEAL")]
    [Header("------------Main Settings------------")]
    public Transform targetObject;
    public float minYValue = -5f;
    [Header("------------Options------------")]
    public bool shouldTeleport = true;
    public bool shouldLoadScene = false;
    public bool shouldQuitApplication = false;
    [Header("TELEPORT")]
    public Vector3 teleportPosition = new Vector3(0f, 10f, 0f);
    public List<GameObject> objectsToDisable;
    public float disableTime = 2f;
    [Header("Scene")]
    public string sceneToLoad;

    private bool isTeleported = false;

    private void Update()
    {
        if (targetObject != null && targetObject.position.y < minYValue)
        {
            if (shouldTeleport && !isTeleported)
            {
                TeleportObject(targetObject);
                isTeleported = true;
                DisableObjectsForTime(objectsToDisable, disableTime);
            }

            if (shouldLoadScene)
            {
                LoadScene(sceneToLoad);
            }

            if (shouldQuitApplication)
            {
                QuitApplication();
            }
        }
    }

    private void TeleportObject(Transform objToTeleport)
    {
        objToTeleport.position = teleportPosition;
    }

    private void LoadScene(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName) != null)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene not found: " + sceneName);
        }
    }

    private void QuitApplication()
    {
        Application.Quit();
    }

    private void DisableObjectsForTime(List<GameObject> objs, float time)
    {
        foreach (var obj in objs)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
            obj.SetActive(false);
        }

        Invoke("EnableObjects", time);
    }

    private void EnableObjects()
    {
        foreach (var obj in objectsToDisable)
        {
            obj.SetActive(true);

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }

        isTeleported = false;
    }
}
