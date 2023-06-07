using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Camera mainCamera; // Kameran�n referans�
    public float zoomSpeed = 5f; // Yak�nla�ma h�z�
    public float maxZoomSize = 5f; // Maksimum zoom b�y�kl���

    float initialCameraSize;
    float yValue;
    bool isZooming = false;
    private void Start()
    {
        initialCameraSize = mainCamera.orthographicSize;
        yValue = mainCamera.transform.position.y;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isZooming = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isZooming = false;
        }
        if (isZooming)
        {
            if (mainCamera.orthographicSize > maxZoomSize)
            {
                mainCamera.orthographicSize -= (1 * Time.deltaTime);
            }
            if (mainCamera.transform.position.y > -0.9f)
            {
                float y = mainCamera.transform.position.y - 4f * Time.deltaTime;
                mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, y, mainCamera.transform.position.z);
            }
        }
        else
        {
            if (mainCamera.orthographicSize < 2)
            {
                mainCamera.orthographicSize += (1 * Time.deltaTime);
            }
        }
    }
}
