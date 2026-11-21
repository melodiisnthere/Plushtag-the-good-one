using UnityEngine;

public class MirrorCameraController : MonoBehaviour
{
    public Transform playerCamera;
    public Transform mirrorPlane;
    public Camera mirrorCamera;

    void LateUpdate()
    {
        Vector3 camOffset = playerCamera.position - mirrorPlane.position;
        Vector3 reflectedPosition = mirrorPlane.position - Vector3.Reflect(camOffset, mirrorPlane.forward);
        mirrorCamera.transform.position = reflectedPosition;

        Vector3 lookDir = Vector3.Reflect(playerCamera.forward, mirrorPlane.forward);
        mirrorCamera.transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
    }
}