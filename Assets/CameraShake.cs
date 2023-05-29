using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.1f;

    private Transform cameraTransform;
    private Vector3 originalPosition;
    private float shakeTimer = 0f;

    private void Start()
    {
        cameraTransform = GetComponent<Camera>().transform;
        originalPosition = cameraTransform.localPosition;
    }

    private void Update()
    {
        if (shakeTimer > 0f)
        {
            cameraTransform.localPosition = originalPosition + Random.insideUnitSphere * shakeMagnitude;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            shakeTimer = 0f;
            cameraTransform.localPosition = originalPosition;
        }
    }

    public void Shake()
    {
        shakeTimer = shakeDuration;
    }
}
