using System.Collections.Generic;
using UnityEngine;

public class PropDatabase : MonoBehaviour
{
    public static PropDatabase Instance;

    [Header("Assign all huntable prop prefabs here")]
    public List<GameObject> propPrefabs = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}