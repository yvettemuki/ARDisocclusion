using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARAnchorManager))]
[RequireComponent(typeof(PlaneController))]
public class AnchorController : MonoBehaviour
{
    ARRaycastManager m_RaycastManager;
    ARAnchorManager m_AnchorManager;
    PlaneController m_PlaneController;

    List<ARAnchor> m_Anchors = new List<ARAnchor>();
    const TrackableType trackableTypes = TrackableType.PlaneWithinPolygon; //TrackableType.FeaturePoint | 
    static List<ARRaycastHit> portalRayCastHits = new List<ARRaycastHit>(); // pivots for portal
    static List<ARRaycastHit> corridorRayCastHits = new List<ARRaycastHit>(); // pivots for corridor
    
    [SerializeField]
    GameObject m_PrefabCorrider;
    public GameObject prefabCorrider
    {
        get { return m_PrefabCorrider; }
        set { m_PrefabCorrider = value; }
    }

    [SerializeField]
    GameObject m_PrefabPortal;
    public GameObject prefabPortal
    {
        get { return m_PrefabPortal; }
        set { m_PrefabPortal = value; }
    }

    [SerializeField]
    GameObject m_PrefabPoint;
    public GameObject prefabPoint
    {
        get { return m_PrefabPoint; }
        set { m_PrefabPoint = value; }
    }

    private GameObject m_CurrentPrefab;
    private bool m_IsCorridorExist = false;
    private bool m_IsPortalExist = false;

    public ARAnchor m_PortalAnchor;
    public ARAnchor m_CorridorAnchor;

    //public Text m_TextAnchorPostiion;

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_AnchorManager = GetComponent<ARAnchorManager>();
        m_PlaneController = GetComponent<PlaneController>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // add the restriction for anchor creation
        //if (m_Anchors.Count >= 1)
        //    return;

        if (Input.touchCount == 0)
            return;

        // Determine create type
        if (!m_IsCorridorExist && ARContorller.currentObjectType == ARContorller.ControlObjectType.OBJ_CORRIDOR)
        {
            CreateMainGeometry(prefabCorrider, corridorRayCastHits);
        }
        else if (!m_IsPortalExist && ARContorller.currentObjectType == ARContorller.ControlObjectType.OBJ_PORTAL)
        {
            CreateMainGeometry(prefabPortal, portalRayCastHits);
        }
        else if (ARContorller.currentObjectType == ARContorller.ControlObjectType.OBJ_POINT)
        {
            m_CurrentPrefab = prefabPoint;

            // get the touch
            var touch = Input.GetTouch(0);
            if (touch.phase != TouchPhase.Began)
                return;

            // Perform the raycast
            List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
            if (m_RaycastManager.Raycast(touch.position, s_Hits, trackableTypes))
            {
                // Raycast hit is stored by distace so the first one will be the closet one
                ARRaycastHit hit = s_Hits[0];

                TextLogger.Log($"Point Position: {hit.pose.position.ToString()}");

                // Create a new anchor
                var anchor = CreateAnchor(hit.pose, hit.trackable, m_CurrentPrefab);
                if (anchor)
                {
                    // Add the anchor to our anchor list to manage
                    SetAnchorText(anchor, $"{hit.pose.position.ToString()}");
                    m_Anchors.Add(anchor);
                }
                else
                {
                    TextLogger.Log("Failed to create point anchor!");
                }

            }
        }
        else if (ARContorller.currentObjectType == ARContorller.ControlObjectType.OBJ_NONE)
            return;

    }

    public Pose CalculatePlacementPose(List<ARRaycastHit> pivotHits)
    {
        Pose pose = new Pose();
        Vector3 position;
        Vector3 forward;
        Vector3 right;
        Vector3 up;
        Quaternion rotation;

        if (pivotHits.Count == 2)
        {
            position = 0.5f * (pivotHits[0].pose.position + pivotHits[1].pose.position);
            //position = pivotHits[0].pose.position;

            right = pivotHits[1].pose.position - pivotHits[0].pose.position;
            up = pivotHits[0].pose.up;
            forward = Vector3.Cross(right, up);  // !!make sure the correct direction
            // calculate the rotation
            Vector3 portalRightDir = pivotHits[1].pose.position - pivotHits[0].pose.position;
            rotation = Quaternion.LookRotation(forward, up); // make sure the right later

            pose.position = position;
            pose.rotation = rotation;
            
        }

        return pose;
    }

    public ARAnchor CreateAnchor(in Pose pose, in ARTrackable trackable, GameObject prefab)
    {
        ARAnchor anchor = null;

        // if hit a plane
        if (trackable is ARPlane plane)
        {
            if (m_PlaneController.GetTargetPlane() == null)
            {
                m_PlaneController.SetTargetPlane(plane);
                m_PlaneController.SetPlaneDetection(false);
                m_PlaneController.SetOtherPlaneActive(false);
            }
            
            var planeManager = GetComponent<ARPlaneManager>();

            if (planeManager)
            {
                TextLogger.Log($"Creating anchor attachment at the position {pose.position.ToString()}");
                var oldPrefab = m_AnchorManager.anchorPrefab; //?? why needs this step
                m_AnchorManager.anchorPrefab = prefab;
                anchor = m_AnchorManager.AttachAnchor(plane, pose);
                m_AnchorManager.anchorPrefab = oldPrefab;
            }
        }
        else
        {
            // else, give the user some alert information for not adding to the plane
            TextLogger.Log("Please place the model onto the horizontal plane");
        }

        return anchor;
    }

    public void Reset()
    {
        if (m_Anchors.Count == 0)
        {
            TextLogger.Log("Anchors list is empty!");
            return;
        }

        // clear the anchors of corridor and portal
        foreach (var anchor in m_Anchors)
        {
            Destroy(anchor.gameObject);
        }

        m_IsCorridorExist = false;
        m_IsPortalExist = false;

        m_PortalAnchor = null;
        m_CorridorAnchor = null;

        m_Anchors.Clear();
        portalRayCastHits.Clear();
        corridorRayCastHits.Clear();
        TextLogger.Log("Anchors has been cleared!");

        m_PlaneController.SetPlaneDetection(true);
        m_PlaneController.SetOtherPlaneActive(true);
        m_PlaneController.SetTargetPlane(null);
    }

    public List<Vector3> getPortal4CornerPositions()
    {
        if (!m_IsPortalExist)
        {
            TextLogger.Log($"Portal should be created first!");
            return null;
        }

        List<Vector3> _fourCorners = new List<Vector3>();
        // add the point of the portal: 1.bottom left, 2.bottom right, 3.top left, 4.top right
        // attention: should not use pivots[] because it may not equal to the real one
        Vector3 right = m_PortalAnchor.gameObject.transform.right;
        Vector3 up = m_PortalAnchor.gameObject.transform.up;
        Vector3 pos = m_PortalAnchor.gameObject.transform.position;

        var _position_1 = pos - right * ControllerStates.PORTAL_WIDTH * 0.5f;
        var _position_2 = pos + right * ControllerStates.PORTAL_WIDTH * 0.5f;
        var _position_3 = pos + up * ControllerStates.PORTAL_HEIGHT - right * ControllerStates.PORTAL_WIDTH * 0.5f;
        var _position_4 = pos + up * ControllerStates.PORTAL_HEIGHT + right * ControllerStates.PORTAL_WIDTH * 0.5f;

        _fourCorners.Add(_position_1);
        _fourCorners.Add(_position_2);
        _fourCorners.Add(_position_3);
        _fourCorners.Add(_position_4);

        return _fourCorners;
    }

    public void SetAnchorText(ARAnchor anchor, string text)
    {
        var canvasTextManager = anchor.GetComponent<CanvasTextManager>();
        if (canvasTextManager)
        {
            canvasTextManager.text = text;
        }
    }

    public void CreateMainGeometry(GameObject prefab, List<ARRaycastHit> pivots)
    {
        m_CurrentPrefab = prefab;

        if (pivots.Count < 2)
        {
            // get two pivots for the portal
            var touch = Input.GetTouch(0);
            if (touch.phase != TouchPhase.Began)
                return;

            List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
            if (m_RaycastManager.Raycast(touch.position, s_Hits, trackableTypes))
            {
                ARRaycastHit hit = s_Hits[0];

                TextLogger.Log($"[{prefab.name}] Hit point {pivots.Count}: {hit.pose.position.ToString()}");
                pivots.Add(hit);
            }
            else
            {
                TextLogger.Log($"[{prefab.name}] hit point create failed");
            }
        }
        else if (pivots.Count == 2)
        {
            // create portal anchor
            Pose pose = CalculatePlacementPose(pivots);

            // add for the reset
            pivots.Add(new ARRaycastHit());

            // Create a new anchor
            var anchor = CreateAnchor(pose, pivots[0].trackable, m_CurrentPrefab);
            if (anchor)
            {
                // Add the anchor to our anchor list to manage
                m_Anchors.Add(anchor);

                if (ARContorller.currentObjectType == ARContorller.ControlObjectType.OBJ_PORTAL)
                {
                    m_PortalAnchor = anchor;
                    m_IsPortalExist = true;

                    Debug.Log($"the calculate pose : {pose.position.ToString()}");
                    Debug.Log($"the portal posi: {m_PortalAnchor.gameObject.transform.position.ToString()} ");

                    // attach the corridor geometry to the portal
                    AttachGeoToPortal(prefabCorrider, pose);
                }
                else if (ARContorller.currentObjectType == ARContorller.ControlObjectType.OBJ_CORRIDOR)
                {
                    m_CorridorAnchor = anchor;
                    m_IsCorridorExist = true;

                    // set corridor to unvisable
                    m_CorridorAnchor.gameObject.SetActive(false);
                }
                
            }
            else
            {
                TextLogger.Log($"Failed to create [{prefab.name}] anchor!");
            }
        }
        else
            return;
    }


    // Create and attach the geometry prefab (corridor) to the portal anchor
    private void AttachGeoToPortal(GameObject geoPrefab, Pose pose)
    {
        if (!m_IsPortalExist)
            return;

        if (geoPrefab == null || pose == null)
            return;

        Debug.Log("--- in the attach function of the geometry! ----");

        ARTrackable plane = m_PlaneController.GetTargetPlane();
        if (plane != null)
        {
            var anchor = CreateAnchor(pose, plane, geoPrefab);

            if (anchor)
            {
                // TODO: current we only have one geometry model (corridor)
                m_CorridorAnchor = anchor;
                m_Anchors.Add(anchor);
                m_IsCorridorExist = true;

                m_CorridorAnchor.gameObject.SetActive(false);
            }
        }

    }
}
