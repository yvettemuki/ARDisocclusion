using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class ARController : MonoBehaviour
{
    [SerializeField]
    ARSession m_Session;
    [SerializeField]
    ARRaycastManager m_ARRaycastManager;

    // TODO: Serialize Field
    public AnchorController m_AnchorController;
    public Camera m_ARCamera;
    public CameraImageController m_CameraImageController;
    public ProjectorController m_ProjectorController;
    public UserStudyController m_UserStudyController;
    public UserStudyFlowController m_FormalStudy;
    public UserStudyTrainingFlow m_TrainingStudy;

    public Dropdown m_DropDownAnchorType;
    public Dropdown m_DropDownUserStudyType;
    public Text m_TextCameraPos;
    public Text m_TextPortalPos;
    public Text m_TextAttentionInfo;
    public Text m_HumanPosInfo;
    public GameObject m_HumanSpritePrefab;
    public GameObject m_CameraBPrefab;
    public GameObject m_PortalPlanePrefab;
    public GameObject m_MirrorPrefab;
    public Texture2D m_Frame0;
    public Texture2D m_Frame1;
    public GameObject m_ProjectorPrefabBG; // background projector for side corridor
    public GameObject m_ProjectorPrefabHM; // human projector
    public GameObject m_ProjectorPrefabMULTI; // multiperspective projector
    public GameObject m_ProjectorPrefabLeftMAINCORD; // main corridor projector left
    public GameObject m_ProjectorPrefabRightMAINCORD; // main corridor projector right
    public GameObject m_StencilMaskPortalAreaPrefab;

    public RawImage m_RawImagePicInPicInSetupCanvas;  // pic in pic render image
    public RawImage m_RawImagePicInPicInUserCanvas;
    public RawImage m_RawImagePicInPicInTrainCanvas;

    private List<Vector3> m_4PortalCornerPositions;
    private Vector2 m_HumanLowestUV;
    private Vector3 m_HumanLowestPointDirFromCamB;
    private GameObject m_HumanSprite;
    private GameObject m_PortalPlane;
    private GameObject m_Mirror;
    private GameObject m_CameraB;
    private GameObject m_ProjectorBG;
    private GameObject m_ProjectorHM;
    private GameObject m_ProjectorMULTI;
    private GameObject m_ProjectorLeftMAINCORD;
    private GameObject m_ProjectorRightMAINCORD;
    private GameObject m_StencilMaskPortalArea;
    private bool m_IsCameraBRegisterd = false;
    private bool m_IsPlaybackSegment = false;
    private bool m_IsPlaybackHumanSprite = false;
    private Texture2D m_HumanSpriteTex;
    private Camera m_MirrorCamera;
    private int m_CameraACullingMask = 0;

    // Camera B data in world space
    Vector3 camera_b_pos = Vector3.zero;
    Vector3 forward = Vector3.zero;
    Vector3 up = Vector3.zero;
    Vector3 right = Vector3.zero;
    float portal_depth = 0f;

    // Portal coordinate system data (generate in the ar world != precomputed)
    Vector3 portal_origin = Vector3.zero;
    Vector3 portal_x_axis = Vector3.zero;
    Vector3 portal_y_axis = Vector3.zero;
    Vector3 portal_z_axis = Vector3.zero;
    Vector3 camera_a_pos_in_portal = Vector3.zero;
    float portal_x_lower_bound = 0f;
    float portal_x_upper_bound = 0f;

    // Video stuff
    float playbackTime = 0f;
    //public List<Texture2D> m_Frames;


    struct CameraBFrame
    {
        public Texture2D tex;
        public Vector2 footUV;

        public CameraBFrame(Texture2D tex, Vector2 footUV)
        {
            this.tex = tex;
            this.footUV = footUV;
        }
    }
    //private List<CameraBFrame> m_CameraBFrames = new List<CameraBFrame>();

    // human video frame for user study
    private List<CameraBFrame> m_UserStudyCamBFrames = new List<CameraBFrame>();
    public List<Texture2D> m_UserStudyFrames;
    private List<CameraBFrame> m_TrainCamBFrames = new List<CameraBFrame>();
    public List<Texture2D> m_TrainFrames;


    public enum ControlObjectType
    {
        OBJ_NONE,
        OBJ_CORRIDOR,
        OBJ_PORTAL,
        OBJ_POINT,
    };

    public enum UserStudyType
    {
        TYPE_CUTAWAY,
        TYPE_MULTIPERSPECTIVE,
        TYPE_PICINPIC,
        TYPE_XRAY,
        TYPE_REFLECTION,
        TYPE_NONE
    };

    public static ControlObjectType currentObjectType = ControlObjectType.OBJ_NONE;
    public static UserStudyType currentUserStudyType = UserStudyType.TYPE_NONE;

    // Start is called before the first frame update
    void Start()
    {
        if (ARSession.state == ARSessionState.Unsupported)
        {
            TextLogger.Log("This devide is not supported for the ARCore!");
            return;
        }

        m_CameraACullingMask = m_ARCamera.cullingMask;

        InitializeHumanVideoFrames();
    }

    // Update is called once per frame/
    void Update()
    {
        UpdateCameraPosition();

        //if (m_IsPlaybackSegment)
        //{
        //    PlaybackCameraBSegment();
        //}

        if (m_IsPlaybackHumanSprite)
        {
            PlaybackHumanSpriteInSideCorridor();
        }

        if (currentUserStudyType == UserStudyType.TYPE_CUTAWAY)
            CutawayDisocclusion();
        else if (currentUserStudyType == UserStudyType.TYPE_MULTIPERSPECTIVE)
            MultiperspDisocclusion();
        else if (currentUserStudyType == UserStudyType.TYPE_PICINPIC)
            PicInPicDisocclusion();
        else if (currentUserStudyType == UserStudyType.TYPE_XRAY)
            XRayDisocclusion();
        else if (currentUserStudyType == UserStudyType.TYPE_REFLECTION)
            ReflectionDisocclusion();

        // methods to see around the corner (disocclusion)
        //if (currentUserStudyType == UserStudyType.TYPE_CUTAWAY)
        //    PlaybackHumanSpriteInSideCorridor();
        //else if (currentUserStudyType == UserStudyType.TYPE_MULTIPERSPECTIVE)
        //    PlaybackCameraBVideoClipInMultiPersp();
        //else if (currentUserStudyType == UserStudyType.TYPE_PICINPIC)
        //    PlaybackCameraBVideoClipInPicture();

    }

    public void Reset()
    {
        //m_Session.Reset();

        m_AnchorController.Reset();
        m_UserStudyController.Reset();

        if (m_HumanSprite) Destroy(m_HumanSprite);
        if (m_CameraB) Destroy(m_CameraB);
        if (m_ProjectorBG) Destroy(m_ProjectorBG);
        if (m_ProjectorHM) Destroy(m_ProjectorHM);
        if (m_ProjectorMULTI) Destroy(m_ProjectorMULTI);
        if (m_PortalPlane) Destroy(m_PortalPlane);
        //if (m_Mirror) Destroy(m_Mirror);

        m_IsPlaybackSegment = false;
        m_IsPlaybackHumanSprite = false;
        m_IsCameraBRegisterd = false;

    }

    public void CleanUpScene()
    {
        // let user study controlle to control it
        if (m_HumanSprite) Destroy(m_HumanSprite);
        if (m_PortalPlane) Destroy(m_PortalPlane);
        //if (m_Mirror) Destroy(m_Mirror);

        //if (m_PortalPlane && m_PortalPlane.activeSelf) m_PortalPlane.SetActive(false);
        if (m_ProjectorMULTI.gameObject.activeSelf) m_ProjectorMULTI.gameObject.SetActive(false);
        if (m_RawImagePicInPicInUserCanvas.gameObject.activeSelf) m_RawImagePicInPicInUserCanvas.gameObject.SetActive(false);
        if (m_RawImagePicInPicInSetupCanvas.gameObject.activeSelf) m_RawImagePicInPicInSetupCanvas.gameObject.SetActive(false);
        if (m_RawImagePicInPicInTrainCanvas.gameObject.activeSelf) m_RawImagePicInPicInTrainCanvas.gameObject.SetActive(false);
        if (m_ProjectorLeftMAINCORD.gameObject.activeSelf) m_ProjectorLeftMAINCORD.gameObject.SetActive(false);
        if (m_StencilMaskPortalArea) Destroy(m_StencilMaskPortalArea);
        
        // reset the near clip plane of camera B
        m_CameraB.GetComponent<Camera>().nearClipPlane = 0.1f;

        // reset the side corridor projector blend mode
        m_ProjectorController.SetSideCorridorProjectorMaterial(0);
        m_ProjectorController.SetSideCorridorProjectorColor(new Color(1f, 1f, 1f, 1f));
    }

    public void CreateStencilMaskArea()
    {
        // Create the Stencil Mask Portal Area
        Transform portal_transform = GetPortalTransform();
        m_StencilMaskPortalArea = Instantiate(m_StencilMaskPortalAreaPrefab, portal_transform.position, portal_transform.rotation);
    }

    public void OnControlObjectChanged()
    {
        var selectedValue = m_DropDownAnchorType.options[m_DropDownAnchorType.value].text;

        switch (selectedValue)
        {
            case "None":
                currentObjectType = ControlObjectType.OBJ_NONE;
                break;
            case "Corridor":
                currentObjectType = ControlObjectType.OBJ_CORRIDOR;
                break;
            case "Portal":
                currentObjectType = ControlObjectType.OBJ_PORTAL;
                break;
            case "Point":
                currentObjectType = ControlObjectType.OBJ_POINT;
                break;
            default: 
                break;
        }
    }

    public void OnUserStudyTypeChanged()
    {
        var selected = m_DropDownUserStudyType.options[m_DropDownUserStudyType.value].text;

        switch (selected)
        {
            case "None":
                currentUserStudyType = UserStudyType.TYPE_NONE;
                CleanUpScene();
                m_AnchorController.m_Corridor.SetActive(false);
                break;

            case "Cut-Away":
                currentUserStudyType = UserStudyType.TYPE_CUTAWAY;
                CleanUpScene();
                m_UserStudyController.SetUserStudyObjectsActive(true);
                m_AnchorController.m_Corridor.SetActive(true);
                break;

            case "Multipersp":
                currentUserStudyType = UserStudyType.TYPE_MULTIPERSPECTIVE;
                CleanUpScene();
                m_AnchorController.m_Corridor.SetActive(false);
                m_CameraB.GetComponent<Camera>().nearClipPlane = portal_depth;
                m_ProjectorMULTI.gameObject.SetActive(true);
                break;

            case "PicInPic":
                currentUserStudyType = UserStudyType.TYPE_PICINPIC;
                CleanUpScene();
                m_AnchorController.m_Corridor.SetActive(false);
                m_UserStudyController.SetUserStudyObjectsActive(true);
                m_RawImagePicInPicInUserCanvas.gameObject.SetActive(true);
                CreateStencilMaskArea();
                break;

            case "X-Ray":
                currentUserStudyType = UserStudyType.TYPE_XRAY;
                CleanUpScene();
                m_AnchorController.m_Corridor.SetActive(true);
                // dynamic sphere view
                m_UserStudyController.SetUserStudyObjectsActive(true);
                m_ProjectorLeftMAINCORD.gameObject.SetActive(true);
                m_ProjectorController.SetMainCorridorProjectorMaterial(1, 5);
                //m_ProjectorRightMAINCORD.gameObject.SetActive(true);
                break;

            case "Reflection":
                currentUserStudyType = UserStudyType.TYPE_REFLECTION;
                CleanUpScene();
                m_AnchorController.m_Corridor.SetActive(false);
                m_UserStudyController.SetUserStudyObjectsActive(true);
                //m_ProjectorLeftMAINCORD.gameObject.SetActive(true);
                //m_ProjectorController.SetMainCorridorProjectorMaterial(1, 0);
                CreateStencilMaskArea();
                break;

            default:
                break;
        }
    }

    public void UpdateCameraPosition()
    {
        m_TextCameraPos.text = $"CameraA Position:\n" +
            $"{m_ARCamera.transform.position.ToString("F")}\n";

        // calculate cameraA position in portal coord system
        if (m_IsCameraBRegisterd)
        {
            m_TextCameraPos.text = $"CamA World Position:\n" +
            $"{m_ARCamera.transform.position.ToString("F")}\n" +
            $"CamB Pos:\n {camera_b_pos.ToString("F")}\n";

            if (currentUserStudyType == UserStudyType.TYPE_CUTAWAY || currentUserStudyType == UserStudyType.TYPE_XRAY)
            {
                CalCameraAPositionInPortal();

                // set the visibility of the corridor
                if (camera_a_pos_in_portal.x < portal_x_lower_bound)
                {
                    m_TextCameraPos.text += $"left side {portal_x_lower_bound}:\n{camera_a_pos_in_portal.ToString()}\n";
                    m_AnchorController.m_Corridor.transform.GetChild(0).Find("Geo Wall Right Side").gameObject.SetActive(true);
                    m_AnchorController.m_Corridor.transform.GetChild(0).Find("Geo Wall Left Front").gameObject.SetActive(true);
                    m_AnchorController.m_Corridor.transform.GetChild(0).Find("Geo Wall Right Front").gameObject.SetActive(false);
                    if (currentUserStudyType == UserStudyType.TYPE_CUTAWAY)
                    {
                        m_AnchorController.m_Corridor.transform.GetChild(0).Find("Geo Wall Left Side").gameObject.SetActive(false);
                        m_AnchorController.m_Corridor.transform.GetChild(0).Find("Auxiliary Plane Left").gameObject.SetActive(true);
                        m_AnchorController.m_Corridor.transform.GetChild(0).Find("Auxiliary Plane Right").gameObject.SetActive(false);
                    }
                    else 
                    {
                        m_AnchorController.m_Corridor.transform.GetChild(0).Find("Geo Wall Left Side").gameObject.SetActive(true);
                        m_AnchorController.m_Corridor.transform.GetChild(0).Find("Auxiliary Plane Left").gameObject.SetActive(false);
                        m_AnchorController.m_Corridor.transform.GetChild(0).Find("Auxiliary Plane Right").gameObject.SetActive(false);
                    }

                    m_AnchorController.m_Corridor.SetActive(true);

                }
                else if (camera_a_pos_in_portal.x > portal_x_upper_bound)
                {
                    m_TextCameraPos.text += $"right side {portal_x_upper_bound}:\n{camera_a_pos_in_portal.ToString()}\n ";
                    m_AnchorController.m_Corridor.transform.GetChild(0).Find("Geo Wall Left Side").gameObject.SetActive(true);
                    m_AnchorController.m_Corridor.transform.GetChild(0).Find("Geo Wall Right Front").gameObject.SetActive(true);
                    m_AnchorController.m_Corridor.transform.GetChild(0).Find("Geo Wall Left Front").gameObject.SetActive(false);
                    if (currentUserStudyType == UserStudyType.TYPE_CUTAWAY)
                    {
                        m_AnchorController.m_Corridor.transform.GetChild(0).Find("Geo Wall Right Side").gameObject.SetActive(false);
                        m_AnchorController.m_Corridor.transform.GetChild(0).Find("Auxiliary Plane Right").gameObject.SetActive(true);
                        m_AnchorController.m_Corridor.transform.GetChild(0).Find("Auxiliary Plane Left").gameObject.SetActive(false);
                    }
                    else 
                    {
                        m_AnchorController.m_Corridor.transform.GetChild(0).Find("Geo Wall Right Side").gameObject.SetActive(true);
                        m_AnchorController.m_Corridor.transform.GetChild(0).Find("Auxiliary Plane Left").gameObject.SetActive(false);
                        m_AnchorController.m_Corridor.transform.GetChild(0).Find("Auxiliary Plane Right").gameObject.SetActive(false);
                    }
                    m_AnchorController.m_Corridor.SetActive(true);

                }
                else
                {
                    m_TextCameraPos.text += $"center:\n{camera_a_pos_in_portal.ToString()}\n ";
                    m_AnchorController.m_Corridor.SetActive(false);
                }
            }
            
        }


    }

    public void SetSideCorridorViewActive(in bool isActive)
    {
        // side corridor self view
        if (m_AnchorController.m_Corridor)
        {
            m_AnchorController.m_Corridor.SetActive(isActive);
            m_AnchorController.m_Corridor.transform.GetChild(0).Find("Geo Wall Right Side").gameObject.SetActive(isActive);
            m_AnchorController.m_Corridor.transform.GetChild(0).Find("Geo Wall Left Side").gameObject.SetActive(isActive);
            m_AnchorController.m_Corridor.transform.GetChild(0).Find("Auxiliary Plane Right").gameObject.SetActive(false);
            m_AnchorController.m_Corridor.transform.GetChild(0).Find("Auxiliary Plane Left").gameObject.SetActive(false);

            // mirror effect
            if (currentUserStudyType == UserStudyType.TYPE_REFLECTION)
            {
                //m_AnchorController.m_CorridorAnchor.gameObject.transform.GetChild(0).Find("Geo Wall Left Front").gameObject.SetActive(isActive);
                //m_AnchorController.m_CorridorAnchor.gameObject.transform.GetChild(0).Find("Geo Wall Right Front").gameObject.SetActive(isActive);
                //m_AnchorController.m_CorridorAnchor.gameObject.transform.GetChild(0).Find("Geo Ceil Main").gameObject.SetActive(isActive);
                //m_AnchorController.m_CorridorAnchor.gameObject.transform.GetChild(0).Find("Geo Floor Main").gameObject.SetActive(isActive);
            }
            else
            {
                m_AnchorController.m_Corridor.transform.GetChild(0).Find("Geo Wall Left Front").gameObject.SetActive(false);
                m_AnchorController.m_Corridor.transform.GetChild(0).Find("Geo Wall Right Front").gameObject.SetActive(false);
            }
        }

        // set user study objects view
        if (ARController.currentUserStudyType == ARController.UserStudyType.TYPE_PICINPIC ||
            ARController.currentUserStudyType == ARController.UserStudyType.TYPE_REFLECTION)
        {
            if (UserStudyController.currentTaskMode == UserStudyController.TaskMode.DIRECT_INDICATOR_EASY
            || UserStudyController.currentTaskMode == UserStudyController.TaskMode.DIRECT_INDICATOR_MEDIUM
            || UserStudyController.currentTaskMode == UserStudyController.TaskMode.DIRECT_INDICATOR_HARD)
            {
                // human sprite view
                if (m_HumanSprite)
                    m_HumanSprite.SetActive(isActive);
            }
            return;
        }

        // dynamic sphere view
        m_UserStudyController.SetUserStudyObjectsActive(isActive);
        
    }

    public void CalCameraAPositionInPortal()
    {
        if (!m_AnchorController.m_PortalAnchor)
        {
            Debug.Log("Portal should be exist for calculation!");
            return;
        }

        // calcualte the cameraA position with respect to the portal
        Vector3 cam_pos0 = m_ARCamera.transform.position - portal_origin;
        camera_a_pos_in_portal = new Vector3(
            Vector3.Dot(portal_x_axis, cam_pos0),
            Vector3.Dot(portal_y_axis, cam_pos0),
            Vector3.Dot(portal_z_axis, cam_pos0)
        );
    }

    public void RegisterSecondCamera()
    {
        
        m_4PortalCornerPositions = m_AnchorController.getPortal4CornerPositions();

        if (m_4PortalCornerPositions.Count != 4)
        {
            m_TextPortalPos.text = $"Portal corner position lacks!";
            return;
        }

        m_TextPortalPos.text = 
            $"Portal Corner Position:\n" +
            $"{m_4PortalCornerPositions[0].ToString("F")}\n" +
            $"{m_4PortalCornerPositions[1].ToString("F")}\n" +
            $"{m_4PortalCornerPositions[2].ToString("F")}\n" +
            $"{m_4PortalCornerPositions[3].ToString("F")}";

        // calculate portal coordinate system data
        Transform portal_transform = m_AnchorController.m_PortalAnchor.gameObject.transform;
        portal_origin = portal_transform.position;
        portal_x_axis = portal_transform.right;
        portal_y_axis = portal_transform.up;
        portal_z_axis = portal_transform.forward;
        float portal_width = m_AnchorController.m_PortalAnchor.gameObject.transform.GetChild(1).transform.localScale.x;
        portal_x_lower_bound = -0.5f * portal_width;
        portal_x_upper_bound = 0.5f * portal_width;

        /** Process the data we use to do the extrinsic calibration and set the camera 
         * (human sprite projector ans background projector) in the positon relative to 
         * the portal, then get the new world position of the cameraB base on the ar core 
         * renew coordinate system.
         * */
        ProcessCameraBProjectorsPos();

        if (m_IsCameraBRegisterd)
            m_TextAttentionInfo.gameObject.SetActive(true);

    }

    // Deprecated: cut-away disocclusion with human sprite
    public void CutAwayDisocclusionWithHuman()
    {
        if (!m_IsCameraBRegisterd)
        {
            Debug.Log($"Camera B should be registered before disocclusion!");
            return;
        }

        // set the human texture for background subtraction
        m_HumanSpriteTex = m_Frame1;

        // find the lowest point ****** TODO ******
        m_HumanLowestUV = new Vector2(ControllerStates.HUMAN_HAND_SET_LOWEST_U, ControllerStates.HUMAN_HAND_SET_LOWEST_V);

        // set the huamn sprite projector texture
        ApplyHumanCurrentTexture(ControllerStates.PlaybackMode.PLAY_BACK_SEGMENT);

        // transform uv to world space
        TransformFromUVToWorldPoint(in m_HumanLowestUV, out m_HumanLowestPointDirFromCamB);

        // generate ray
        Ray ray = new Ray(camera_b_pos, m_HumanLowestPointDirFromCamB);

        // ray cast
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (m_ARRaycastManager.Raycast(ray, hits, TrackableType.PlaneWithinPolygon))
        {
            ARRaycastHit hit = hits[0];
            Debug.Log($"+++++++++ human sprite position: {hit.pose.position.ToString()}+++++++");

            m_HumanSprite = Instantiate(m_HumanSpritePrefab, hit.pose.position, Quaternion.LookRotation(forward, up));

            // set corridor visable
            m_AnchorController.m_CorridorAnchor.gameObject.SetActive(true);
        }
        else
            Debug.Log($"Failed to visualize the human sprite! Please check the code!");
    }

    public void InitHumanSpriteForUserStudy(UserStudyController.TaskMode taskMode)
    {
        if (!m_IsCameraBRegisterd)
        {
            Debug.Log($"Camera B should be registered before disocclusion!");
            return;
        }

        if (taskMode == UserStudyController.TaskMode.DIRECT_INDICATOR_EASY)
        {
            m_HumanSpriteTex = m_UserStudyCamBFrames[0].tex;
            m_HumanLowestUV = new Vector2(ControllerStates.USER_STUDY_DIRECT_INDI_FONT_UVs[0].x, ControllerStates.USER_STUDY_DIRECT_INDI_FONT_UVs[0].y);
        }
        else if (taskMode == UserStudyController.TaskMode.DIRECT_INDICATOR_MEDIUM)
        {
            m_HumanSpriteTex = m_UserStudyCamBFrames[1].tex;
            m_HumanLowestUV = new Vector2(ControllerStates.USER_STUDY_DIRECT_INDI_FONT_UVs[1].x, ControllerStates.USER_STUDY_DIRECT_INDI_FONT_UVs[1].y);
        }
        else if (taskMode == UserStudyController.TaskMode.DIRECT_INDICATOR_HARD)
        {
            m_HumanSpriteTex = m_UserStudyCamBFrames[2].tex;
            m_HumanLowestUV = new Vector2(ControllerStates.USER_STUDY_DIRECT_INDI_FONT_UVs[2].x, ControllerStates.USER_STUDY_DIRECT_INDI_FONT_UVs[2].y);
        }
        else
        {
            Debug.Log($"Should be used in the human direction indicator taks!");
            return;
        }

        m_ProjectorController.SetRenderTexture(m_HumanSpriteTex);

        TransformFromUVToWorldPoint(in m_HumanLowestUV, out m_HumanLowestPointDirFromCamB);

        Ray ray = new Ray(camera_b_pos, m_HumanLowestPointDirFromCamB);

        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (m_ARRaycastManager.Raycast(ray, hits, TrackableType.PlaneWithinPolygon))
        {
            ARRaycastHit hit = hits[0];

            m_HumanSprite = Instantiate(m_HumanSpritePrefab, hit.pose.position, Quaternion.LookRotation(forward, up));
        }
        else
            Debug.Log($"filed to place the human sprite, the plane is not large enough!");
    }

    public void InitHumanSpriteForUserStudy(int humanDataIndex, bool isStudyMode)
    {
        if (!m_IsCameraBRegisterd)
        {
            Debug.Log($"Camera B should be registered before disocclusion!");
            return;
        }

        if (isStudyMode == ControllerStates.STUDY_MODE)
        {
            m_HumanSpriteTex = m_UserStudyCamBFrames[humanDataIndex].tex;
            m_HumanLowestUV = new Vector2(ControllerStates.USER_STUDY_DIRECT_INDI_FONT_UVs[humanDataIndex].x, ControllerStates.USER_STUDY_DIRECT_INDI_FONT_UVs[humanDataIndex].y);
        }
        else
        {
            m_HumanSpriteTex = m_TrainCamBFrames[humanDataIndex].tex;
            m_HumanLowestUV = new Vector2(ControllerStates.TRAIN_DIRECT_INDI_FONT_UVs[humanDataIndex].x, ControllerStates.TRAIN_DIRECT_INDI_FONT_UVs[humanDataIndex].y);
        }
        
        m_ProjectorController.SetRenderTexture(m_HumanSpriteTex);

        TransformFromUVToWorldPoint(in m_HumanLowestUV, out m_HumanLowestPointDirFromCamB);

        Ray ray = new Ray(m_CameraB.transform.position, m_HumanLowestPointDirFromCamB);

        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (m_ARRaycastManager.Raycast(ray, hits, TrackableType.PlaneWithinPolygon))
        {
            ARRaycastHit hit = hits[0];

            m_HumanSprite = Instantiate(m_HumanSpritePrefab, hit.pose.position, Quaternion.LookRotation(m_CameraB.transform.forward, m_CameraB.transform.up));
        }
        else
            Debug.Log($"filed to place the human sprite, the plane is not large enough!");
    }

    public void InitializeHumanVideoFrames()
    {
        for (int i = 0; i < ControllerStates.USER_STUDY_DIRECT_INDI_FONT_UVs.Length; i++)
        {
            Vector2 uv = ControllerStates.USER_STUDY_DIRECT_INDI_FONT_UVs[i];

            if (m_UserStudyFrames[i] != null && uv != Vector2.zero)
            {
                CameraBFrame frame = new CameraBFrame(m_UserStudyFrames[i], uv);
                m_UserStudyCamBFrames.Add(frame);
            }
        }

        for (int i = 0; i < ControllerStates.TRAIN_DIRECT_INDI_FONT_UVs.Length; i++)
        {
            Vector2 uv = ControllerStates.TRAIN_DIRECT_INDI_FONT_UVs[i];

            if (m_TrainFrames[i] != null && uv != Vector2.zero)
            {
                CameraBFrame frame = new CameraBFrame(m_TrainFrames[i], uv);
                m_TrainCamBFrames.Add(frame);
            }
        }
    }

    // Textures disocclusion (Multiperspetive)
    public void MultiperspDisocclusion()
    {
        if (!m_IsCameraBRegisterd)
        {
            Debug.Log($"Camera B should be registered before disocclusion!");
            return;
        }

        if (!m_AnchorController.m_PortalAnchor)
        {
            Debug.Log($"Portal should be created before disocclusion!");
            return;
        }

        if (!m_PortalPlane)
        {
            Vector3 position = GetPortalTransform().position;
            Quaternion rotation = GetPortalTransform().rotation;
            m_PortalPlane = Instantiate(m_PortalPlanePrefab, position, rotation);
        }
        else
        {
            m_PortalPlane.SetActive(true);
        }

        SetSideCorridorViewActive(true);
        m_CameraB.GetComponent<Camera>().Render();
        SetSideCorridorViewActive(false);
    }

    public void PicInPicDisocclusion()
    {
        if (!m_IsCameraBRegisterd)
        {
            Debug.Log($"Camera B should be registered before disocclusion!");
            return;
        }

        if (!m_AnchorController.m_PortalAnchor)
        {
            Debug.Log($"Portal should be created before disocclusion!");
            return;
        }

        if (!m_StencilMaskPortalArea)
        {
            CreateStencilMaskArea();
        }

        SetSideCorridorViewActive(true);
        m_CameraB.GetComponent<Camera>().Render();
        SetSideCorridorViewActive(false);
    }

    public void ReflectionDisocclusion()
    {
        if (!m_IsCameraBRegisterd)
        {
            Debug.Log($"Camera B should be registered before disocclusion!");
            return;
        }

        if (!m_Mirror)
        {
            // create mirror
            Vector3 mirror_position = Vector3.zero;
            Quaternion mirror_rotation = Quaternion.identity;

            PortalObjectPos2World(in ControllerStates.MIRROR_POS_IN_PORTAL, out mirror_position);
            PortalObjectRot2World(in ControllerStates.MIRROR_ROT_IN_PORTAL, out mirror_rotation);
            m_Mirror = Instantiate(m_MirrorPrefab, mirror_position, mirror_rotation);

            // set cam not enable
            m_MirrorCamera = m_Mirror.transform.GetChild(2).gameObject.GetComponent<Camera>();
            m_MirrorCamera.enabled = false;
            
        }

        if (!m_StencilMaskPortalArea)
        {
            CreateStencilMaskArea();
        }

        SetSideCorridorViewActive(true);
        m_MirrorCamera.Render();
        //Texture render_texture = m_MirrorCamera.targetTexture;
        //m_Mirror.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.mainTexture = render_texture;
        SetSideCorridorViewActive(false);
    }

    public void XRayDisocclusion()
    {
        if (!m_IsCameraBRegisterd)
        {
            Debug.Log($"Camera B should be registered before disocclusion!");
            return;
        }

        if (!m_CameraImageController)
        {
            Debug.Log("Make sure camera image controller exist!");
            return;
        }

        m_ProjectorController.SetSideCorridorProjectorMaterial(5);
        m_ProjectorController.SetSideCorridorProjectorColor(new Color(1f, 1f, 1f, 0.33f));
    }

    public void CutawayDisocclusion()
    {
        if (!m_IsCameraBRegisterd)
        {
            Debug.Log($"Camera B should be registered before disocclusion!");
            return;
        }

        m_UserStudyController.SetUserStudyObjectsActive(true);
    }

    public void CaptureCamAView()
    {
        if (!m_IsCameraBRegisterd)
        {
            Debug.Log($"Camera B should be registered before disocclusion!");
            return;
        }

        if (!m_CameraImageController)
        {
            Debug.Log("Make sure camera image controller exist!");
            return;
        }

        // set projector position based on the current ar camera position
        m_ProjectorLeftMAINCORD = Instantiate(m_ProjectorPrefabLeftMAINCORD, m_ARCamera.transform.position, m_ARCamera.transform.rotation);
        m_ProjectorLeftMAINCORD.gameObject.SetActive(false);
        m_CameraImageController.ProjectCameraA();

        Vector3 pos_in_portal = Vector3.zero;
        Quaternion rot_in_portal = Quaternion.identity;
        WorldObjectPos2Portal(m_ARCamera.transform.position, out pos_in_portal);
        WorldObjectRot2Portal(m_ARCamera.transform, out rot_in_portal);

        m_TextPortalPos.text = $"Cam A Pos in portal: \n" +
            $"{pos_in_portal.ToString("F")} \n" +
            $"Cam A rot in portal: \n " +
            $"{rot_in_portal.ToString("F")}";
    }

    public void ProcessCameraBProjectorsPos()
    {
        Vector3 cam_pos = ControllerStates.CAM_B_EYE_POS;
        Vector3 look_at_point = ControllerStates.CAM_B_LOOK_AT_POINT;
        Vector3 up_at_point = cam_pos + ControllerStates.CAM_B_UP;

        // calculate portal coordinate system (attention: this is the precomputed portal data)
        Vector3 portal_bottom_left_0 = ControllerStates.PORTAL_BOTTOM_LEFT;
        Vector3 portal_bottom_right_0 = ControllerStates.PORTAL_BOTTOM_RIGHT;
        Vector3 portal_top_left_0 = ControllerStates.PORTAL_TOP_LEFT;
        Vector3 portal_origin_0 = 0.5f * (portal_bottom_left_0 + portal_bottom_right_0);

        Vector3 portal_x_axis_0 = (portal_bottom_right_0 - portal_bottom_left_0).normalized;
        Vector3 portal_y_axis_0 = (portal_top_left_0 - portal_bottom_left_0).normalized;
        Vector3 portal_z_axis_0 = Vector3.Cross(portal_x_axis_0, portal_y_axis_0).normalized;

        // transform camB position to portal coord system
        Vector3 cam_pos_0 = cam_pos - portal_origin_0;
        Vector3 cam_pos_in_portal_coord = new Vector3(
            Vector3.Dot(portal_x_axis_0, cam_pos_0),
            Vector3.Dot(portal_y_axis_0, cam_pos_0),
            Vector3.Dot(portal_z_axis_0, cam_pos_0)
        );

        // transform look at position to portal coord system
        Vector3 look_at_0 = look_at_point - portal_origin_0;
        Vector3 look_at_in_portal_coord = new Vector3(
            Vector3.Dot(portal_x_axis_0, look_at_0),
            Vector3.Dot(portal_y_axis_0, look_at_0),
            Vector3.Dot(portal_z_axis_0, look_at_0)
        );

        // transform up point position to portal coord system
        Vector3 up_at_0 = up_at_point - portal_origin_0;
        Vector3 up_at_in_portal_coord = new Vector3(
            Vector3.Dot(portal_x_axis_0, up_at_0),
            Vector3.Dot(portal_y_axis_0, up_at_0),
            Vector3.Dot(portal_z_axis_0, up_at_0)
        );

        // calculate the rotation of the camera in portal coord system
        Vector3 forward_in_portal_coord = (look_at_in_portal_coord - cam_pos_in_portal_coord).normalized;
        Vector3 up_in_portal_coord = (up_at_in_portal_coord - cam_pos_in_portal_coord).normalized;
        Quaternion rotation_in_portal_coord = Quaternion.LookRotation(forward_in_portal_coord, up_in_portal_coord);

        // Instantiate the camera B in the portal local coordinate
        if (m_AnchorController.m_PortalAnchor != null)
        {
            // create camera
            m_CameraB = Instantiate(m_CameraBPrefab);
            m_CameraB.transform.parent = m_AnchorController.m_PortalAnchor.transform;
            m_CameraB.transform.localPosition = cam_pos_in_portal_coord;
            m_CameraB.transform.localRotation = rotation_in_portal_coord;

            // we get the positon of the camera without instantiate but use the m_PortalPrefab.TransformPoint(c) 
            camera_b_pos = m_CameraB.transform.position;
            forward = m_CameraB.transform.forward.normalized;
            up = m_CameraB.transform.up.normalized;
            right = m_CameraB.transform.right.normalized;

            // set cameraB status
            m_CameraB.GetComponent<Camera>().enabled = false;
            m_IsCameraBRegisterd = true;

            // create projector for side corridor
            m_ProjectorBG = Instantiate(m_ProjectorPrefabBG);
            m_ProjectorBG.transform.parent = m_AnchorController.m_PortalAnchor.gameObject.transform;
            m_ProjectorBG.transform.localPosition = cam_pos_in_portal_coord;
            m_ProjectorBG.transform.localRotation = rotation_in_portal_coord;
            m_ProjectorBG.gameObject.SetActive(true);

            // create projector for human sprite
            m_ProjectorHM = Instantiate(m_ProjectorPrefabHM);
            m_ProjectorHM.transform.parent = m_AnchorController.m_PortalAnchor.gameObject.transform;
            m_ProjectorHM.transform.localPosition = cam_pos_in_portal_coord;
            m_ProjectorHM.transform.localRotation = rotation_in_portal_coord;
            m_ProjectorHM.gameObject.SetActive(true);

            // create projector for multiperspective portal plane
            m_ProjectorMULTI = Instantiate(m_ProjectorPrefabMULTI);
            m_ProjectorMULTI.transform.parent = m_AnchorController.m_PortalAnchor.gameObject.transform;
            m_ProjectorMULTI.transform.localPosition = cam_pos_in_portal_coord;
            m_ProjectorMULTI.transform.localRotation = rotation_in_portal_coord;
            m_ProjectorMULTI.gameObject.SetActive(false);

            // create projector for left main corridor
            m_ProjectorLeftMAINCORD = Instantiate(m_ProjectorPrefabLeftMAINCORD);
            m_ProjectorLeftMAINCORD.transform.parent = m_AnchorController.m_PortalAnchor.gameObject.transform;
            m_ProjectorLeftMAINCORD.transform.localPosition = ControllerStates.PROJECTOR_MAIN_LEFT_CORD_POS_IN_PORTAL;
            m_ProjectorLeftMAINCORD.transform.localRotation = ControllerStates.PROJECTOR_MAIN_LEFT_CORD_ROT_IN_PORTAL;
            m_ProjectorLeftMAINCORD.gameObject.SetActive(false);

            // create projector for main right corridor
            //m_ProjectorRightMAINCORD = Instantiate(m_ProjectorPrefabRightMAINCORD);
            //m_ProjectorRightMAINCORD.transform.parent = m_AnchorController.m_PortalAnchor.transform;
            //m_ProjectorRightMAINCORD.transform.localPosition = ControllerStates.PROJECTOR_MAIN_RIGHT_CORD_POS_IN_PORTAL;
            //m_ProjectorRightMAINCORD.transform.localRotation = ControllerStates.PROJECTOR_MAIN_RIGHT_CORD_ROT_IN_PORTAL;
            //m_ProjectorRightMAINCORD.gameObject.SetActive(false);

            // get depth of the side corridor (portal) from the cameraB
            portal_depth = Mathf.Abs(cam_pos_in_portal_coord.z);
        }
        else
        {
            Debug.Log($"-------- Attention: the portal position should not be empty! -----");
        }
        
    }

    public void TransformFromUVToWorldPoint(in Vector2 uv, out Vector3 pointDir)
    {
        pointDir = Vector3.zero;

        if (uv == Vector2.zero)
        {
            Debug.Log("The uv should be assigned!");
            return;
        }

        // calculate near clip plane point
        float tex_width = m_Frame1.width;
        float tex_height = m_Frame1.height;
        float aspect = (float)tex_height / (float)tex_width;
        float half_h_fov = ControllerStates.CAM_B_HFOV * 0.5f;
        float clip_half_width = ControllerStates.Z_NEAR * Mathf.Tan(Mathf.Deg2Rad * half_h_fov);
        float clip_half_height = clip_half_width * aspect;

        // get the start(bottom left) point of the clip plane
        Vector3 clip_center_point = m_CameraB.transform.position + m_CameraB.transform.forward * ControllerStates.Z_NEAR;
        Debug.Log($"&&& clip center point is : {clip_center_point.ToString()}"); // shoulbe be z = z_near 0.1
        Vector3 clip_start_point = clip_center_point - m_CameraB.transform.right * clip_half_width - m_CameraB.transform.up * clip_half_height;  // start from bottom left

        Vector3 cam_pos_to_clip_start_dir = clip_start_point - m_CameraB.transform.position;  // attenction: here we don't need to normalize the vector

        // calcualte the uv 3D point
        float uu = uv.x * (clip_half_width * 2 / tex_width);
        float vv = uv.y * (clip_half_height * 2 / tex_height);


        pointDir = cam_pos_to_clip_start_dir + uu * m_CameraB.transform.right + vv * m_CameraB.transform.up;

    }

    public void RaycastHumanSpritePosition(Ray ray)
    {
        // ray cast for human position
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (m_ARRaycastManager.Raycast(ray, hits, TrackableType.PlaneWithinPolygon))
        {
            ARRaycastHit hit = hits[0];
            Debug.Log($"+++++++++ human sprite position: {hit.pose.position.ToString()}+++++++");

            if (!m_HumanSprite)
            {
                m_HumanSprite = Instantiate(m_HumanSpritePrefab, hit.pose.position, Quaternion.LookRotation(forward, up));
            }
            else
            {
                m_HumanSprite.transform.position = hit.pose.position;
            }

        }
        else
            Debug.Log($"filed to place the human sprite, the plane is not large enough!");
    }

    //public void PlaybackCameraBSegment()
    //{
    //    if (m_CameraBFrames.Count <= 0)
    //    { 
    //        Debug.Log($"Video frames should not be empty!");
    //        return;
    //    }

    //    if (!m_IsCameraBRegisterd)
    //    {
    //        Debug.Log($"Please first register the camera B!");
    //        return;
    //    }

    //    int index = (int)Mathf.Floor(playbackTime) % ControllerStates.SEGMENT_FRAME_NUM;
    //    CameraBFrame frame = m_CameraBFrames[index];

    //    if (!frame.Equals(default(CameraBFrame)))
    //    {
    //        Vector2 uv = frame.footUV;
    //        Texture2D tex = frame.tex;
    //        Vector3 dirFromCamBToUV = Vector3.zero;

    //        // apply projector texture to the human sprite
    //        m_HumanSpriteTex = tex;

    //        // apply human projector texture
    //        ApplyHumanCurrentTexture(ControllerStates.PlaybackMode.PLAY_BACK_SEGMENT);

    //        // transform uv to world space
    //        TransformFromUVToWorldPoint(in uv, out dirFromCamBToUV);

    //        // generate ray
    //        Ray ray = new Ray(camera_b_pos, dirFromCamBToUV);

    //        // update the human position
    //        RaycastHumanSpritePosition(ray);
    //    }

    //    playbackTime += Time.deltaTime;
    //}

    public void PlaybackHumanSpriteInSideCorridor()
    {
        if (!m_IsCameraBRegisterd)
        {
            Debug.Log($"Please first register the camera B!");
            return;
        }

        Vector3 dirFromCamBToUV = Vector3.zero;
        Vector2 uv = Vector2.zero;

        // get the time data for position clculation
        int time_0 = (int)m_ProjectorController.videoPlayer.time;

        if (time_0 < ControllerStates.VIDEO_CLIP_FRAME_NUM - 1)
        {
            int time_1 = (int)m_ProjectorController.videoPlayer.time + 1;
            //float current_time = (int)(m_ProjectorController.videoPlayer.time * 1000f) / 1000f;
            float current_time = (float)m_ProjectorController.videoPlayer.time;

            Vector2 uv_min = ControllerStates.VIDEO_FOOT_UVs[time_0];
            Vector2 uv_max = ControllerStates.VIDEO_FOOT_UVs[time_1];

            // linear interpolation the position
            float ratio = (current_time - time_0) / (time_1 - time_0);
            uv = uv_min + ratio * (uv_max - uv_min);
        }
        else
        {
            uv = ControllerStates.VIDEO_FOOT_UVs[time_0];
        }

        ApplyHumanCurrentTexture(ControllerStates.PlaybackMode.PLAY_BACK_VIDEO_CLIP);

        TransformFromUVToWorldPoint(in uv, out dirFromCamBToUV);

        Ray ray = new Ray(camera_b_pos, dirFromCamBToUV);

        RaycastHumanSpritePosition(ray);
        
    }

    public void PlaybackCameraBVideoClipInMultiPersp()
    {
        // play back human sprite in side corridor
        //PlaybackHumanSpriteInSideCorridor();

        // disocclusion
        MultiperspDisocclusion();

    }

    public void PlaybackCameraBVideoClipInPicture()
    {
        // play back human sprite in side corridor
        //PlaybackHumanSpriteInSideCorridor();

        // disocclusion
        PicInPicDisocclusion();
    }

    public void PlayBackCameraBVideoClipInMirror()
    {
        //PlaybackHumanSpriteInSideCorridor();
        ReflectionDisocclusion();
    }

    public void SetPlayBackSegment()
    {
        if (m_IsPlaybackSegment)
            playbackTime = 0;

        m_IsPlaybackSegment = !m_IsPlaybackSegment;
    }

    public void SetPlayBackHumanSprite()
    {
        m_IsPlaybackHumanSprite = !m_IsPlaybackHumanSprite;

        if (m_IsPlaybackHumanSprite)
        {
            m_ProjectorController.videoPlayer.Play();
        }
        else
        {
            m_ProjectorController.videoPlayer.Stop();

            if (m_HumanSprite)
                Destroy(m_HumanSprite);
        }
    }

    //public void InitializeCameraBVideoFrames()
    //{
    //   for (int i = 0; i < ControllerStates.FOOT_UVs.Length; i++)
    //   {
    //        Vector2 uv = ControllerStates.FOOT_UVs[i];

    //        if (m_Frames[i] != null && uv != Vector2.zero)
    //        {
    //            CameraBFrame frame = new CameraBFrame(m_Frames[i], uv);
    //            m_CameraBFrames.Add(frame);
    //        }
    //    }
    //}

    private void ApplyHumanCurrentTexture(ControllerStates.PlaybackMode mode)
    {
        if (mode == ControllerStates.PlaybackMode.PLAY_BACK_SEGMENT)
            m_ProjectorController.SetRenderTexture(m_HumanSpriteTex);
        else if (mode == ControllerStates.PlaybackMode.PLAY_BACK_VIDEO_CLIP)
            m_ProjectorController.SetHumanRenderTexture(currentUserStudyType);
    }

    public GameObject GetHumanSprite()
    {
        if (m_HumanSprite)
            return m_HumanSprite;
        else
            return null;
    }

    // transform object position from portal coord to world
    public void PortalObjectPos2World(in Vector3 pos_in_portal, out Vector3 pos_in_world)
    {
        pos_in_world = Vector3.zero;
        ARAnchor portal_anchor = m_AnchorController.m_PortalAnchor;

        if (!portal_anchor)
        {
            Debug.Log("Must have portal for the transform!");
            return;
        }

        pos_in_world = portal_anchor.gameObject.transform.TransformPoint(pos_in_portal);
    }

    // transform object rotation from portal coord to world
    public void PortalObjectRot2World(in Quaternion rot_in_portal, out Quaternion rot_in_world)
    {
        rot_in_world = Quaternion.identity;

        ARAnchor portal_anchor = m_AnchorController.m_PortalAnchor;

        if (!portal_anchor)
        {
            Debug.Log("Must have portal for the transform!");
            return;
        }

        Vector3 forward_in_portal = ControllerStates.MIRROR_ROT_IN_PORTAL * Vector3.forward;
        Vector3 up_in_portal = ControllerStates.MIRROR_ROT_IN_PORTAL * Vector3.up;
        Vector3 forward_in_world = portal_anchor.gameObject.transform.TransformDirection(forward_in_portal);
        Vector3 up_in_world = portal_anchor.gameObject.transform.TransformDirection(up_in_portal);
        rot_in_world = Quaternion.LookRotation(forward_in_world, up_in_world);
    }

    // the transfrom of the portal
    public Transform GetPortalTransform()
    {
        if (!m_AnchorController.m_PortalAnchor)
        {
            return null;
        }

        return m_AnchorController.m_PortalAnchor.gameObject.transform;
    }

    public void WorldObjectPos2Portal(in Vector3 pos_in_world, out Vector3 pos_in_portal)
    {
        pos_in_portal = Vector3.zero;

        if (!m_AnchorController.m_PortalAnchor)
        {
            Debug.Log("Portal should be exist for calculation!");
            return;
        }

        // calcualte the cameraA position with respect to the portal
        Vector3 pos_in_portal_0 = pos_in_world - GetPortalTransform().position;
        pos_in_portal = new Vector3(
            Vector3.Dot(GetPortalTransform().right, pos_in_portal_0),
            Vector3.Dot(GetPortalTransform().up, pos_in_portal_0),
            Vector3.Dot(GetPortalTransform().forward, pos_in_portal_0)
        );

    }

    public void WorldObjectRot2Portal(in Transform transform_in_world, out Quaternion rot_in_portal)
    {
        rot_in_portal = Quaternion.identity;

        if (!m_AnchorController.m_PortalAnchor)
        {
            Debug.Log("Portal should be exist for calculation!");
            return;
        }

        Transform portal_transform = m_AnchorController.m_PortalAnchor.gameObject.transform;
        Vector3 up_in_portal = portal_transform.InverseTransformDirection(transform_in_world.up);
        Vector3 forward_in_portal = portal_transform.InverseTransformDirection(transform_in_world.forward);

        rot_in_portal = Quaternion.LookRotation(forward_in_portal, up_in_portal);
    }

    public void SetCameraBNearClipToPortalDepth()
    {
        if (!m_IsCameraBRegisterd)
        {
            Debug.Log("This method should used after the camera B register!");
            return;
        }

        m_CameraB.GetComponent<Camera>().nearClipPlane = portal_depth;
    }

    public void SetCameraBNearClipToDefaultDepth()
    {
        if (!m_IsCameraBRegisterd)
        {
            Debug.Log("This method should used after the camera B register!");
            return;
        }

        m_CameraB.GetComponent<Camera>().nearClipPlane = 0.1f;
    }

    public void SetProjectorMULTIActive(bool isActive)
    {
        if (!m_IsCameraBRegisterd)
        {
            Debug.Log("This method should used after the camera B register!");
            return;
        }

        m_ProjectorMULTI.gameObject.SetActive(isActive);
    }

    public void SetProjectorMAINCORDActive(bool isActive)
    {
        if (!m_IsCameraBRegisterd)
        {
            Debug.Log("This method should used after the camera B register!");
            return;
        }

        m_ProjectorLeftMAINCORD.gameObject.SetActive(isActive);
    }

}
