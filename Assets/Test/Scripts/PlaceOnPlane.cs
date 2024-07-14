using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;

[HelpURL("https://youtu.be/HkNVp04GOEI")]
[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnPlane : PressInputBase
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject placedPrefab;

    [SerializeField]
    [Tooltip("Maximum number of objects that can be placed on the plane.")]
    int maxObjectsToPlace = 3;

    [SerializeField]
    [Tooltip("The line renderer used to display the raycast.")]
    LineRenderer raycastLine;

    [SerializeField]
    [Tooltip("The maximum distance of the raycast.")]
    float maxRaycastDistance = 10f;

    GameObject[] placedObjects;
    int placedObjectCount = 0;

    ARRaycastManager arRaycastManager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    PlaneSizeLogger planeSizeLogger;
    Camera arCamera;

    protected override void Awake()
    {
        base.Awake();
        arRaycastManager = GetComponent<ARRaycastManager>();
        placedObjects = new GameObject[maxObjectsToPlace];
        planeSizeLogger = FindObjectOfType<PlaneSizeLogger>();
        arCamera = FindObjectOfType<Camera>();

        if (raycastLine == null)
        {
            Debug.LogWarning("Raycast Line is not assigned. Creating a new LineRenderer.");
            raycastLine = gameObject.AddComponent<LineRenderer>();
            raycastLine.startWidth = 0.01f;
            raycastLine.endWidth = 0.01f;
            raycastLine.material = new Material(Shader.Find("Sprites/Default"));
            raycastLine.startColor = Color.red;
            raycastLine.endColor = Color.red;
        }
    }

    void Update()
    {
        if (Pointer.current == null)
            return;

        Vector2 touchPosition = Pointer.current.position.ReadValue();
        UpdateRaycastLine(touchPosition);

        if (isPressed)
        {
            isPressed = false;

            if (arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = hits[0].pose;
                var hitTrackableId = hits[0].trackableId;

                if (placedObjectCount < maxObjectsToPlace)
                {
                    ARPlane plane = arRaycastManager.GetComponent<ARPlaneManager>().GetPlane(hitTrackableId);
                    if (plane != null)
                    {
                        planeSizeLogger.LogPlaneSize(plane);
                    }

                    placedObjects[placedObjectCount] = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
                    placedObjectCount++;
                }
            }
        }
    }

    void UpdateRaycastLine(Vector2 screenPosition)
    {
        Ray ray = arCamera.ScreenPointToRay(screenPosition);
        Vector3 endPosition = ray.origin + ray.direction * maxRaycastDistance;

        if (arRaycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            endPosition = hits[0].pose.position;
        }

        raycastLine.SetPosition(0, ray.origin);
        raycastLine.SetPosition(1, endPosition);
    }

    protected override void OnPress(Vector3 position) => isPressed = true;

    protected override void OnPressCancel()
    {
        isPressed = false;
    }
}