using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.ARFoundation;

public class SpawnAnchorFromRaySelect : MonoBehaviour
{
    public XRRayInteractor rayInteractor;
    public ARAnchorManager anchorManager;
    public GameObject anchorPrefab; // Optional: A prefab for visual feedback of the anchor

    void Start()
    {
        // Add listener to rayInteractor's selectEntered event
        rayInteractor.selectEntered.AddListener(SpawnAnchor);
    }

    void Update()
    {
        // Check for mouse click or touch input
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            TrySpawnAnchor();
        }
    }

    public void SpawnAnchor(SelectEnterEventArgs args)
    {
        TrySpawnAnchor();
    }

    private void TrySpawnAnchor()
    {
        // Try to get the current 3D raycast hit from the rayInteractor
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            // Calculate forward direction and rotation
            Vector3 forward = Vector3.Cross(hit.normal, rayInteractor.transform.right);
            Quaternion rotation = Quaternion.LookRotation(forward, hit.normal);
            Pose hitPose = new Pose(hit.point, rotation);

            // Create an AR anchor at the hit position
            ARAnchor anchor = anchorManager.AddAnchor(hitPose);

            if (anchor == null)
            {
                Debug.LogError("Failed to create anchor.");
            }
            else
            {
                Debug.Log($"Anchor created successfully at {hit.point} with rotation {rotation}.");

                // Optional: Instantiate visual feedback
                if (anchorPrefab != null)
                {
                    Instantiate(anchorPrefab, hit.point, rotation);
                }
            }
        }
    }
}
