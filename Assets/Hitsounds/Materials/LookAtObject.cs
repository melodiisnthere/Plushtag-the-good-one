using UnityEngine;

public class LookAtObject : MonoBehaviour
{
    [Header("Made by Keo.cs")]
    [Header("You do not have to give creds you you can")]
    public Transform target;
    public float smoothness = 5f;
    public Vector3 offset;

    void Update()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position + offset;
            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothness * Time.deltaTime);
        }
    }
}
