using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class ARContorller : MonoBehaviour
{
    [SerializeField]
    ARSession m_Session;
    [SerializeField]
    ARRaycastManager m_ARRaycastManager;

    public AnchorController m_AnchorController;
    public Camera m_ARCamera;
    public ARCameraController m_ARCameraController;
    public ProjectorController m_ProjectorController;

    public Dropdown m_DropDown;
    public Text m_TextCameraPos;
    public Text m_TextPortalPos;
    //public Text m_TextHumanPos;
    //public Camera m_CameraB;
    public GameObject m_HumanSpritePrefab;
    public GameObject m_CameraBPrefab;
    public Texture2D m_Frame0;
    public Texture2D m_Frame1;
    public GameObject m_ProjectorPrefabBG; // background projector
    public GameObject m_ProjectorPrefabHM; // human projector

    private List<Vector3> m_4PortalCornerPositions;
    private Vector2 m_HumanLowestUV;
    private Vector3 m_HumanLowestPointDirFromCamB;
    private GameObject m_HumanSprite;
    private GameObject m_CameraB;
    private GameObject m_ProjectorBG;
    private GameObject m_ProjectorHM;
    private bool m_IsCameraBRegisterd = false;
    private bool m_IsPlaybackSegment = false;
    private bool m_IsPlaybackVideoClip = false;
    private Texture2D m_HumanSpriteTex;

    // Camera B data as the child of Portal
    Vector3 camera_b_pos = Vector3.zero;
    Vector3 forward = Vector3.zero;
    Vector3 up = Vector3.zero;
    Vector3 right = Vector3.zero;

    // Portal coordinate system data
    Vector3 portal_origin = Vector3.zero;
    Vector3 portal_x_axis = Vector3.zero;
    Vector3 portal_y_axis = Vector3.zero;
    Vector3 portal_z_axis = Vector3.zero;
    Vector3 camera_a_pos_in_portal = Vector3.zero;
    float portal_x_lower_bound = 0f;
    float portal_x_upper_bound = 0f;

    // Video stuff
    float playbackTime = 0f;
    public List<Texture2D> m_Frames;

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
    private List<CameraBFrame> m_CameraBFrames = new List<CameraBFrame>();


    public enum ControlObjectType
    {
        OBJ_NONE,
        OBJ_CORRIDOR,
        OBJ_PORTAL,
        OBJ_POINT,
    };

    public static ControlObjectType currentObjectType = ControlObjectType.OBJ_NONE;

    private List<ARPlane> m_DetectedPlanes = new List<ARPlane> ();

    // Start is called before the first frame update
    void Start()
    {
        if (ARSession.state == ARSessionState.Unsupported)
        {
            TextLogger.Log("This devide is not supported for the ARCore!");
            return;
        }

        // Start the detected
        TextLogger.Log("Start the AR session!");

        // Start set up the camera
        //m_ARCamera.enabled = true;
        //m_CameraB.enabled = false;

        InitializeCameraBVideoFrames();
    }

    // Update is called once per frame/
    void Update()
    {
        UpdateCameraPosition();

        if (m_IsPlaybackSegment)
        {
            PlaybackCameraBSegment();
        }

        if (m_IsPlaybackVideoClip)
        {
            PlaybackCameraBVideoClip();
        }

        
    }

    public void Reset()
    {
        //m_Session.Reset();

        m_AnchorController.Reset();

        Destroy(m_HumanSprite);
        Destroy(m_CameraB);
        Destroy(m_ProjectorBG);
        Destroy(m_ProjectorHM);

        m_IsPlaybackSegment = false;
        m_IsCameraBRegisterd = false;
    }

    public void onControlObjectChanged()
    {
        var selectedValue = m_DropDown.options[m_DropDown.value].text;

        Debug.Log("++++ " + selectedValue);
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

    public void UpdateCameraPosition()
    {
        m_TextCameraPos.text = $"CameraA Position:\n" +
            $"{m_ARCamera.transform.position.ToString()}\n";

        // calculate cameraA position in portal coord system
        if (m_IsCameraBRegisterd)
        {
            m_TextCameraPos.text = $"CamA World Position:\n" +
            $"{m_ARCamera.transform.position.ToString()}\n + " +
            $"CamB Pos:\n {camera_b_pos.ToString()}\n";

            // calcualte the cameraA position with respective to the portal
            Vector3 cam_pos0 = m_ARCamera.transform.position - portal_origin;
            camera_a_pos_in_portal = new Vector3(
                Vector3.Dot(portal_x_axis, cam_pos0),
                Vector3.Dot(portal_y_axis, cam_pos0),
                Vector3.Dot(portal_z_axis, cam_pos0)
            );

            if (camera_a_pos_in_portal.x < portal_x_lower_bound)
            {
                m_TextCameraPos.text += $"left side {portal_x_lower_bound}:\n{camera_a_pos_in_portal.ToString()}\n";
                m_AnchorController.m_CorridorAnchor.gameObject.transform.GetChild(0).Find("Geo Wall Left Side").gameObject.SetActive(false);
                m_AnchorController.m_CorridorAnchor.gameObject.transform.GetChild(0).Find("Geo Wall Right Side").gameObject.SetActive(true);
                m_AnchorController.m_CorridorAnchor.gameObject.transform.GetChild(0).Find("Auxiliary Plane Left").gameObject.SetActive(true);
                m_AnchorController.m_CorridorAnchor.gameObject.transform.GetChild(0).Find("Auxiliary Plane Right").gameObject.SetActive(false);
                m_AnchorController.m_CorridorAnchor.gameObject.SetActive(true);
                
            }
            else if (camera_a_pos_in_portal.x > portal_x_upper_bound)
            {
                m_TextCameraPos.text += $"right side {portal_x_upper_bound}:\n{camera_a_pos_in_portal.ToString()}\n ";
                m_AnchorController.m_CorridorAnchor.gameObject.transform.GetChild(0).Find("Geo Wall Right Side").gameObject.SetActive(false);
                m_AnchorController.m_CorridorAnchor.gameObject.transform.GetChild(0).Find("Geo Wall Left Side").gameObject.SetActive(true);
                m_AnchorController.m_CorridorAnchor.gameObject.transform.GetChild(0).Find("Auxiliary Plane Right").gameObject.SetActive(true);
                m_AnchorController.m_CorridorAnchor.gameObject.transform.GetChild(0).Find("Auxiliary Plane Left").gameObject.SetActive(false);
                m_AnchorController.m_CorridorAnchor.gameObject.SetActive(true);
                
            }
            else
            {
                m_TextCameraPos.text += $"center:\n{camera_a_pos_in_portal.ToString()}\n ";
                m_AnchorController.m_CorridorAnchor.gameObject.SetActive(false);
            }

        }


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
            $"{m_4PortalCornerPositions[0].ToString()}\n" +
            $"{m_4PortalCornerPositions[1].ToString()}\n" +
            $"{m_4PortalCornerPositions[2].ToString()}\n" +
            $"{m_4PortalCornerPositions[3].ToString()}";

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

        // set the camera B status
        m_IsCameraBRegisterd = true;

    }

    public void XRayDisocclusion()
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

    public void ProcessCameraBProjectorsPos()
    {
        Vector3 cam_pos = ControllerStates.CAM_B_EYE_POS;
        Vector3 look_at_point = ControllerStates.CAM_B_LOOK_AT_POINT;
        Vector3 up_at_point = cam_pos + ControllerStates.CAM_B_UP;

        // calculate portal coordinate system (attention: this is the precomputed portal data)
        Vector3 portal_bottom_left = ControllerStates.PORTAL_BOTTOM_LEFT;
        Vector3 portal_bottom_right = ControllerStates.PORTAL_BOTTOM_RIGHT;
        Vector3 portal_top_left = ControllerStates.PORTAL_TOP_LEFT;
        Vector3 portal_origin = 0.5f * (portal_bottom_left + portal_bottom_right);

        Vector3 portal_x_axis = (portal_bottom_right - portal_bottom_left).normalized;
        Vector3 portal_y_axis = (portal_top_left - portal_bottom_left).normalized;
        Vector3 portal_z_axis = Vector3.Cross(portal_x_axis, portal_y_axis).normalized;

        // transform eye position to portal coord system
        Vector3 cam_pos_0 = cam_pos - portal_origin;
        Vector3 cam_pos_in_portal_coord = new Vector3(
            Vector3.Dot(portal_x_axis, cam_pos_0),
            Vector3.Dot(portal_y_axis, cam_pos_0),
            Vector3.Dot(portal_z_axis, cam_pos_0)
        );

        // transform look at position to portal coord system
        Vector3 look_at_0 = look_at_point - portal_origin;
        Vector3 look_at_in_portal_coord = new Vector3(
            Vector3.Dot(portal_x_axis, look_at_0),
            Vector3.Dot(portal_y_axis, look_at_0),
            Vector3.Dot(portal_z_axis, look_at_0)
        );

        // transform up point position to portal coord system
        Vector3 up_at_0 = up_at_point - portal_origin;
        Vector3 up_at_in_portal_coord = new Vector3(
            Vector3.Dot(portal_x_axis, up_at_0),
            Vector3.Dot(portal_y_axis, up_at_0),
            Vector3.Dot(portal_z_axis, up_at_0)
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
            m_CameraB.gameObject.SetActive(false);

            // create projector for side corridor
            m_ProjectorBG = Instantiate(m_ProjectorPrefabBG);
            m_ProjectorBG.transform.parent = m_AnchorController.m_PortalAnchor.transform;
            m_ProjectorBG.transform.localPosition = cam_pos_in_portal_coord;
            m_ProjectorBG.transform.localRotation = rotation_in_portal_coord;
            m_ProjectorBG.gameObject.SetActive(true);

            // create projector for human sprite
            m_ProjectorHM = Instantiate(m_ProjectorPrefabHM);
            m_ProjectorHM.transform.parent = m_AnchorController.m_PortalAnchor.transform;
            m_ProjectorHM.transform.localPosition = cam_pos_in_portal_coord;
            m_ProjectorHM.transform.localRotation = rotation_in_portal_coord;
            m_ProjectorHM.gameObject.SetActive(true);
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
        Vector3 clip_center_point = camera_b_pos + forward * ControllerStates.Z_NEAR;
        Debug.Log($"&&& clip center point is : {clip_center_point.ToString()}"); // shoulbe be z = z_near 0.1
        Vector3 clip_start_point = clip_center_point - right * clip_half_width - up * clip_half_height;  // start from bottom left

        Vector3 cam_pos_to_clip_start_dir = clip_start_point - camera_b_pos;  // attenction: here we don't need to normalize the vector

        // calcualte the uv 3D point
        float uu = uv.x * (clip_half_width * 2 / tex_width);
        float vv = uv.y * (clip_half_height * 2 / tex_height);


        pointDir = cam_pos_to_clip_start_dir + uu * right + vv * up;
        Debug.Log($"------ {pointDir.ToString()} ------");

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
                // to do: also change the orientation of human
            }

        }
        else
            Debug.Log($"Failed to register camera B! Please check the code!");
    }

    public void PlaybackCameraBSegment()
    {
        if (m_CameraBFrames.Count <= 0)
        { 
            Debug.Log($"Video frames should not be empty!");
            return;
        }

        if (!m_IsCameraBRegisterd)
        {
            Debug.Log($"Please first register the camera B!");
            return;
        }

        int index = (int)Mathf.Floor(playbackTime) % ControllerStates.SEGMENT_FRAME_NUM;
        CameraBFrame frame = m_CameraBFrames[index];

        if (!frame.Equals(default(CameraBFrame)))
        {
            Vector2 uv = frame.footUV;
            Texture2D tex = frame.tex;
            Vector3 dirFromCamBToUV = Vector3.zero;

            // apply projector texture to the human sprite
            m_HumanSpriteTex = tex;

            // apply human projector texture
            ApplyHumanCurrentTexture(ControllerStates.PlaybackMode.PLAY_BACK_SEGMENT);

            // transform uv to world space
            TransformFromUVToWorldPoint(in uv, out dirFromCamBToUV);

            // generate ray
            Ray ray = new Ray(camera_b_pos, dirFromCamBToUV);

            // update the human position
            RaycastHumanSpritePosition(ray);
        }

        playbackTime += Time.deltaTime;
    }

    public void PlaybackCameraBVideoClip()
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

    public void SetPlayBackSegment()
    {
        if (m_IsPlaybackSegment)
            playbackTime = 0;

        m_IsPlaybackSegment = !m_IsPlaybackSegment;
    }

    public void SetPlayBackVideoClip()
    {
        m_IsPlaybackVideoClip = !m_IsPlaybackVideoClip;

        if (m_IsPlaybackVideoClip)
            m_ProjectorController.videoPlayer.Play();
        else
            m_ProjectorController.videoPlayer.Stop();
    }

    public void InitializeCameraBVideoFrames()
    {
       for (int i = 0; i < ControllerStates.FOOT_UVs.Length; i++)
       {
            Vector2 uv = ControllerStates.FOOT_UVs[i];

            if (m_Frames[i] != null && uv != Vector2.zero)
            {
                CameraBFrame frame = new CameraBFrame(m_Frames[i], uv);
                m_CameraBFrames.Add(frame);
            }
        }
    }

    private void ApplyHumanCurrentTexture(ControllerStates.PlaybackMode mode)
    {
        if (m_HumanSpriteTex == null)
        {
            Debug.Log($"Human sprite texture can not be null!");
            return;
        }

        if (mode == ControllerStates.PlaybackMode.PLAY_BACK_SEGMENT)
            m_ProjectorController.SetRenderTexture(m_HumanSpriteTex);
        else if (mode == ControllerStates.PlaybackMode.PLAY_BACK_VIDEO_CLIP)
            m_ProjectorController.SetRenderTexture();

    }

}
