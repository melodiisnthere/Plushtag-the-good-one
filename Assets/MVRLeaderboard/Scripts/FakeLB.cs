using UnityEngine;
using Mirror.VR;

public class FakeLB : MonoBehaviour
{
    public GameObject fakeLB;

    private void FixedUpdate()
    {
        if (MirrorVRManager.GetLobbyCode() != null)
        {
            fakeLB.SetActive(false);
        }
        else
        {
            fakeLB.SetActive(true);
        }
    }
}