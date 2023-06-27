using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomController : MonoBehaviour
{
    public float zoomSpeed = 20f;
    public float minimumZoomLevel = 1f;
    public float maximumZoomLevel = 10f;
    public Camera mainCamera;

    private float originalZoomLevel;
    private float currentZoomLevel;
    private float targetZoomLevel;
    private Vector2 originalPosition;
    private Vector2 targetPosition; 
    private bool shouldZoomIn;

    private float normalizeZoomSpeed;

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
        // TODO: either fix the movement issue or just zoom into the center like a reasonable person
        if (shouldZoomIn && !Mathf.Approximately(currentZoomLevel, targetZoomLevel))
        {
            currentZoomLevel = Mathf.MoveTowards(currentZoomLevel, targetZoomLevel, zoomSpeed * Time.deltaTime);
            mainCamera.orthographicSize = currentZoomLevel;

            Vector2 currentPosition = mainCamera.transform.position;
            Vector2.MoveTowards(currentPosition, targetPosition, zoomSpeed * Time.deltaTime * normalizeZoomSpeed);
        }

        if (!shouldZoomIn && !Mathf.Approximately(currentZoomLevel, targetZoomLevel))
        {
            currentZoomLevel = Mathf.MoveTowards(currentZoomLevel, targetZoomLevel, zoomSpeed * Time.deltaTime);
            mainCamera.orthographicSize = currentZoomLevel;

            Vector2 currentPosition = mainCamera.transform.position;
            Vector2.MoveTowards(currentPosition, targetPosition, zoomSpeed * Time.deltaTime);
        }

        shouldZoomIn = false;
    }

    public void ZoomIn(float zoomLevel, Vector2 position)
    {
        Debug.Log((originalZoomLevel - zoomLevel)/(transform.position.magnitude - position.magnitude));
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
