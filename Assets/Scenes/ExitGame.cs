using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGame : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Game has quit...");
        Application.Quit();
    }
}
