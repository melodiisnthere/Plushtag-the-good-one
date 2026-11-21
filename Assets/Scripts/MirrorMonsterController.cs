using UnityEngine;

public class MirrorMonsterController : MonoBehaviour
{
    public Transform playerCamera;
    public Transform mirror;
    public GameObject mirrorMonster;
    public float viewAngleThreshold = 25f;

    void Update()
    {
        Vector3 toMirror = mirror.position - playerCamera.position;
        float angle = Vector3.Angle(playerCamera.forward, toMirror);

        mirrorMonster.SetActive(angle < viewAngleThreshold);
    }
}