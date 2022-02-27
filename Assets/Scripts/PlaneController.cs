using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARPlaneManager))]
public class PlaneController : MonoBehaviour
{
    ARPlaneManager m_ARPlaneManager;

    private ARPlane m_TargetPlane = null;

    // Start is called before the first frame update
    void Start()
    {
        m_TargetPlane = null;
    }

    void Awake()
    {
        m_ARPlaneManager = GetComponent<ARPlaneManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // !!!Some Problem: the lowest height plane is not equal to the target plane(floor)
    private void TrackTargetFloorPlane()
    {
        int index = 0;
        float _minHeight = float.MaxValue;
        TrackableId _min_height_plane_id = default(TrackableId);

        foreach (var plane in m_ARPlaneManager.trackables)
        {
            Vector3 _position = plane.transform.position;
            TrackableId _trackable_id = plane.trackableId;

            if (_position.y <= _minHeight)
            {
                _minHeight = _position.y;
                _min_height_plane_id = _trackable_id;
            }
        }

        foreach (var plane in m_ARPlaneManager.trackables)
        {
            if (plane.trackableId == _min_height_plane_id)
            {
                plane.gameObject.SetActive(true);
                Debug.Log($"++++---- : [{plane.transform.position.ToString()}]");
                Debug.Log($"+++++++++: [{plane.trackableId}]");
            }
            else
                plane.gameObject.SetActive(false);
        }
    }

    public void ToggleTargetPlane()
    {
        if (m_TargetPlane == null)
            return;

        m_TargetPlane.gameObject.SetActive(!m_TargetPlane.gameObject.activeInHierarchy);
    }

    public void SetPlaneDetection(bool isEnable)
    {
        m_ARPlaneManager.enabled = isEnable;
    }

    public void SetTargetPlane(in ARPlane targetPlane)
    {
        m_TargetPlane = targetPlane;
    }

    // set all the planes except target plane active true or false
    public void SetOtherPlaneActive(bool isEnable)
    {
        foreach (var plane in m_ARPlaneManager.trackables)
        {
            if (plane.trackableId != m_TargetPlane.trackableId)
                plane.gameObject.SetActive(isEnable);
        }    
    }
}
