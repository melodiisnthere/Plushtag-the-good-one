using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startSceenchenger : MonoBehaviour
{
    public string SceneName;

    private void Start()
    {
        SceneManager.LoadScene(SceneName);
    }

}
