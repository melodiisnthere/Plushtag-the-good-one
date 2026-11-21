using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ClientCull : NetworkBehaviour
{
    [SerializeField] private int cullLayer = 6;
    void Start()
    {
        if (isLocalPlayer)
        {
            gameObject.layer = cullLayer;
        }
    }
}
