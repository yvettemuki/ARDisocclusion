using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PortalManager : MonoBehaviour
{
    public GameObject m_Portal;

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

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Get the current selected status
        if (ARContorller.currentObjectType != ARContorller.ControlObjectType.OBJ_PORTAL)
        {
            return;
        }

        if (!TryGetTouchPosition(out Vector2 touchPosition))
        {
            return;
        }

        if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinBounds))
        {
            var hitPose = hits[0].pose;

            if (spawnObject == null)
            {
                spawnObject = Instantiate(m_Portal, hitPose.position, hitPose.rotation);

            }
            else
            {
                spawnObject.transform.position = hitPose.position;
            }
        }
    }
}
