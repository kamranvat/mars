using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomController : MonoBehaviour
{
    public float zoomSpeed = 5f;
    public float minimumZoomLevel = 1f;
    public float maximumZoomLevel = 10f;
    public Camera mainCamera;

    private float originalZoomLevel;
    private float currentZoomLevel;
    private float targetZoomLevel;
    private Vector2 originalPosition;
    private Vector2 targetPosition; 
    private bool shouldZoomIn;

    private void Start()
    {
        originalZoomLevel = mainCamera.orthographicSize;
        currentZoomLevel = originalZoomLevel;
        targetZoomLevel = originalZoomLevel;
        originalPosition = mainCamera.transform.position; 
        targetPosition = originalPosition;

        shouldZoomIn = false;
    }

    private void Update()
    {
        if (shouldZoomIn && !Mathf.Approximately(currentZoomLevel, targetZoomLevel))
        {
            currentZoomLevel = Mathf.MoveTowards(currentZoomLevel, targetZoomLevel, zoomSpeed * Time.deltaTime);
            mainCamera.orthographicSize = currentZoomLevel;

            Vector2 currentPosition = mainCamera.transform.position;
            Vector2 newPosition = Vector2.MoveTowards(currentPosition, targetPosition, zoomSpeed * Time.deltaTime);
            mainCamera.transform.position = new Vector3(newPosition.x, newPosition.y, mainCamera.transform.position.z);
        }

        if (!shouldZoomIn && !Mathf.Approximately(currentZoomLevel, targetZoomLevel))
        {
            currentZoomLevel = Mathf.MoveTowards(currentZoomLevel, targetZoomLevel, zoomSpeed * Time.deltaTime);
            mainCamera.orthographicSize = currentZoomLevel;

            Vector2 currentPosition = mainCamera.transform.position;
            Vector2 newPosition = Vector2.MoveTowards(currentPosition, targetPosition, zoomSpeed * Time.deltaTime);
            mainCamera.transform.position = new Vector3(newPosition.x, newPosition.y, mainCamera.transform.position.z);
        }

        shouldZoomIn = false;
    }

    public void ZoomIn(float zoomLevel, Vector2 position)
    {
        targetZoomLevel = Mathf.Clamp(zoomLevel, minimumZoomLevel, maximumZoomLevel);
        targetPosition = position;
        shouldZoomIn = true;
    }

    public void ZoomOut()
    {
        targetZoomLevel = originalZoomLevel;
        targetPosition = originalPosition;
        shouldZoomIn = false;
    }
}
