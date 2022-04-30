using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class CorriderManager : MonoBehaviour
{
    public GameObject m_Corrider;

    private GameObject spawnObject;
    private ARRaycastManager raycastManager;
    private Vector2 clickPosition;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            // Get the touch position
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        // Get the current selected status
        if (ARController.currentObjectType != ARController.ControlObjectType.OBJ_CORRIDOR)
        {
            return;
        }
        
        if (!TryGetTouchPosition(out Vector2 touchPosition))
        {
            return;
        }

        if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinBounds ))
        {
            var hitPose = hits[0].pose;

            if (spawnObject == null)
            {
                spawnObject = Instantiate(m_Corrider, hitPose.position, hitPose.rotation);
            }
            else
            {
                spawnObject.transform.position = hitPose.position;
            }

            TextLogger.Log("Current corrider position: " + hitPose.position);
        }
    }


    public void deleteSpawnObject()
    {
        if (spawnObject == null)
        {
            return;
        }

        Destroy(spawnObject);
    }
}
