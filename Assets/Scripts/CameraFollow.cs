using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.1f;

    private Camera mainCamera;
    private Transform cameraTransform;
    private float shakeTimer = 0f;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        cameraTransform = GetComponent<Camera>().transform;
    }

    private void FixedUpdate()
    {

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.fixedDeltaTime);
        transform.position = smoothedPosition;

        if (shakeTimer > 0f)
        {
            cameraTransform.localPosition = transform.position + Random.insideUnitSphere * shakeMagnitude;
            shakeTimer -= Time.fixedDeltaTime;
        }
        else
        {
            shakeTimer = 0f;
            cameraTransform.localPosition = transform.position;
        }
    }

    public void ShakeCamera()
    {
        shakeTimer = shakeDuration;
    }
}
