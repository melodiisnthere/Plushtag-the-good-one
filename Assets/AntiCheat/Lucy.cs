using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lucy : MonoBehaviour
{
    [Header("SCRIPT MADE BY RATEIX. DONT CLAIM AS YOURS EVEN IF EDITED.")]
    [Header("put main camera in")]
    public GameObject mainCamera;
    public string HandTag = "HandTag";
    public float speed = 1f;
    public float acceleration = 0.01f;
    public float riseDistance = 10f;
    public float yForce = 10f;
    public float yForceDuration = 3f;
    public float restartTime = 1200f;
    public GameObject gorillaPlayer;
    private Rigidbody gorillaRigidbody;
    private bool isMoving = false;
    private float startTime;
    private Vector3 initialPosition;
    private Vector3 targetPosition;
    void Start()
    {
        gorillaRigidbody = gorillaPlayer.GetComponent<Rigidbody>();
        initialPosition = transform.position;
        targetPosition = initialPosition + new Vector3(0, riseDistance, 0);

        if (!gameObject.activeSelf)
        {
            transform.position = initialPosition;
        }
        startTime = Time.time;
        isMoving = true;
    }
    void FixedUpdate()
    {
        if (isMoving)
        {
            MoveTowardsTarget();
        }
    }
  void MoveTowardsTarget()
{
    float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

    if (distanceToTarget > 0.1f && gameObject.activeSelf)
    {
        float currentSpeed = speed + acceleration * (Time.time - startTime);
        Vector3 directionToTarget = (mainCamera.transform.position - transform.position).normalized;
        transform.position += directionToTarget * currentSpeed * Time.deltaTime;
        Vector3 cameraDirection = Camera.main.transform.position - transform.position;
        cameraDirection.y = 0;
        cameraDirection.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(cameraDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 180f * Time.deltaTime);
    }
    else
    {
        isMoving = false;
    }
}
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(HandTag))
        {
            gorillaRigidbody.AddForce(new Vector3(0, yForce, 0), ForceMode.Impulse);
            transform.position = mainCamera.transform.position + mainCamera.transform.forward;
            StartCoroutine(DisappearAfterDuration());
        }
    }
IEnumerator DisappearAfterDuration()
{
    yield return new WaitForSeconds(yForceDuration);

    if (Time.time - startTime > restartTime)
    {
        transform.position = initialPosition;
        isMoving = true;
    }
    else
    {
        gameObject.SetActive(false);
    }
}
}