using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.Collections;
using System.Linq;

[RequireComponent(typeof(ARPlaneManager))]
public class PlaneSizeLogger : MonoBehaviour
{
    ARPlaneManager arPlaneManager;

    void Awake()
    {
        arPlaneManager = GetComponent<ARPlaneManager>();
        arPlaneManager.planesChanged += OnPlanesChanged;
    }

    void OnDestroy()
    {
        arPlaneManager.planesChanged -= OnPlanesChanged;
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs eventArgs)
    {
        foreach (var plane in eventArgs.added)
        {
            LogPlaneSize(plane);
        }

        foreach (var plane in eventArgs.updated)
        {
            LogPlaneSize(plane);
        }
    }

    public void LogPlaneSize(ARPlane plane)
    {
        var boundary = plane.boundary;
        var planeDimensions = CalculatePlaneDimensions(boundary);
        Debug.Log($"Plane detected with dimensions: {planeDimensions.width} cm (width) x {planeDimensions.length} cm (length)");
    }

    (float width, float length) CalculatePlaneDimensions(NativeArray<Vector2> boundary)
    {
        if (boundary.Length < 2)
            return (0, 0);

        var minX = boundary.Min(point => point.x);
        var maxX = boundary.Max(point => point.x);
        var minY = boundary.Min(point => point.y);
        var maxY = boundary.Max(point => point.y);

        float width = (maxX - minX) * 100; // Convert to cm
        float length = (maxY - minY) * 100; // Convert to cm

        return (width, length);
    }
}
