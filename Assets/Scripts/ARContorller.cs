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
    private List<Color> m_ForegroundColors = new List<Color>();
    private Vector2 m_HumanLowestUV;
    private Vector3 m_HumanLowestPointDirFromCamB;
    private GameObject m_HumanSprite;
    private GameObject m_CameraB;
    private GameObject m_ProjectorBG;
    private GameObject m_ProjectorHM;
    private bool m_IsCameraBRegisterd = false;
    private bool m_IsPlayback = false;

    // Camera B data as the child of Portal
    Vector3 camera_pos = Vector3.zero;
    Vector3 forward = Vector3.zero;
    Vector3 up = Vector3.zero;
    Vector3 right = Vector3.zero;

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
        OBJ_QUAD
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
        updateCameraPosition();

        if (m_IsPlayback)
        {
            PlaybackCameraB();
        }
    }

    public void Reset()
    {
        //m_Session.Reset();

        m_AnchorController.Reset();

        Destroy(m_HumanSprite);
        Destroy(m_CameraB);
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
            case "Quad":
                currentObjectType = ControlObjectType.OBJ_QUAD;
                break;
            default: 
                break;
        }
    }

    public void updateCameraPosition()
    {
        m_TextCameraPos.text = $"CameraA Position:\n" +
            $"{m_ARCamera.transform.position.ToString()}\n" +
            $"CameraB Positoin:\n{camera_pos.ToString()}\n" +
            $"Projector Position:\n{m_ProjectorBG.transform.position.ToString()}";
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

        TextLogger.Log($"position 1: {m_4PortalCornerPositions[0].ToString()}");
        TextLogger.Log($"position 2: {m_4PortalCornerPositions[1].ToString()}");
        TextLogger.Log($"position 3: {m_4PortalCornerPositions[2].ToString()}");
        TextLogger.Log($"position 4: {m_4PortalCornerPositions[3].ToString()}");

        /** Process the data we use to do the extrinsic calibration and set the camera is 
         * positon relative to the portal, then get the new world position of the cameraB 
         * base on the ar core renew coordinate system
         * */
        ProcessCameraBPosition();

        // background subtraction
        BackgourndSubtraction(m_Frame0, m_Frame1, out m_HumanLowestUV);

        // transform uv to world space
        TransformFromUVToWorldPoint(in m_HumanLowestUV, out m_HumanLowestPointDirFromCamB);

        // generate ray
        Ray ray = new Ray(camera_pos, m_HumanLowestPointDirFromCamB);

        // ray cast
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (m_ARRaycastManager.Raycast(ray, hits, TrackableType.PlaneWithinPolygon))
        {
            ARRaycastHit hit = hits[0];
            Debug.Log($"+++++++++ human sprite position: {hit.pose.position.ToString()}+++++++");

            m_HumanSprite = Instantiate(m_HumanSpritePrefab, hit.pose.position, Quaternion.LookRotation(forward, up));
            m_IsCameraBRegisterd = true;
        }
        else
            Debug.Log($"Failed to register camera B! Please check the code!");

        // data for test
        //Vector3 test_point_0 = Vector3.zero;
        //Vector2 test_uv_0 = new Vector2(110, 151);
        //TransformFromUVToWorldPoint(in test_uv_0, out test_point_0);
        //Vector3 test_point_1 = Vector3.zero;
        //Vector2 test_uv_1 = new Vector2(472, 160);
        //TransformFromUVToWorldPoint(in test_uv_1, out test_point_1);

        //Ray test_ray_0 = new Ray(camera_pos, test_point_0);
        //Ray test_ray_1 = new Ray(camera_pos, test_point_1);

        //Vector3 plane_norm = (m_4PortalCornerPositions[2] - m_4PortalCornerPositions[0]).normalized;
        //Vector3 p0 = m_4PortalCornerPositions[0];
        //float test_t_0 = Vector3.Dot(p0 - test_ray_0.origin, plane_norm) / Vector3.Dot(test_ray_0.direction, plane_norm);
        //if (test_t_0 > 0)
        //{
        //    Vector3 test_hit_0 = test_ray_0.GetPoint(test_t_0);
        //    Instantiate(m_HumanSpritePrefab, test_hit_0, Quaternion.identity);
        //}
        //float test_t_1 = Vector3.Dot(p0 - test_ray_1.origin, plane_norm) / Vector3.Dot(test_ray_1.direction, plane_norm);
        //if (test_t_1 > 0)
        //{
        //    Vector3 test_hit_1 = test_ray_1.GetPoint(test_t_1);
        //    Instantiate(m_HumanSpritePrefab, test_hit_1, Quaternion.identity);
        //}

    }

    public void ProcessCameraBPosition()
    {
        Vector3 cam_pos = ControllerStates.CAM_B_EYE_POS;
        Vector3 look_at_point = ControllerStates.CAM_B_LOOK_AT_POINT;
        Vector3 up_at_point = cam_pos + ControllerStates.CAM_B_UP;

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
            camera_pos = m_CameraB.transform.position;
            forward = m_CameraB.transform.forward.normalized;
            up = m_CameraB.transform.up.normalized;
            right = m_CameraB.transform.right.normalized;

            // set cameraB enable
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
        Vector3 clip_center_point = camera_pos + forward * ControllerStates.Z_NEAR;
        Debug.Log($"&&& clip center point is : {clip_center_point.ToString()}"); // shoulbe be z = z_near 0.1
        Vector3 clip_start_point = clip_center_point - right * clip_half_width - up * clip_half_height;  // start from bottom left

        Vector3 cam_pos_to_clip_start_dir = clip_start_point - camera_pos;  // attenction: here we don't need to normalize the vector

        // calcualte the uv 3D point
        float uu = uv.x * (clip_half_width * 2 / tex_width);
        float vv = uv.y * (clip_half_height * 2 / tex_height);


        pointDir = cam_pos_to_clip_start_dir + uu * right + vv * up;
        Debug.Log($"------ {pointDir.ToString()} ------");

    }

    public Texture2D BackgourndSubtraction(Texture2D frame0, Texture2D frame1, out Vector2 lowestUV)
    {
        int _tex_width = frame0.width;
        int _tex_height = frame0.height;

        Texture2D _subtract_frame = new Texture2D(_tex_width, _tex_height);
        lowestUV = new Vector2(ControllerStates.HUMAN_HAND_SET_LOWEST_U, ControllerStates.HUMAN_HAND_SET_LOWEST_V);

        if (frame0 == null || frame1 == null)
        {
            Debug.Log("make sure the frame0 and frame1 both imported");
            return null;
        }

        if (frame0.isReadable && frame1.isReadable)
        {
            for (int v = 0; v < _tex_height; v++)
            {
                for (int u = 0; u < _tex_width; u++)
                {
                    Color color0 = frame0.GetPixel(u, v);
                    Color color1 = frame1.GetPixel(u, v);

                    float diff_0 = Mathf.Pow(color0.r - color1.r, 2);
                    float diff_1 = Mathf.Pow(color0.g - color1.g, 2);
                    float diff_2 = Mathf.Pow(color0.b - color1.b, 2);

                    if (diff_0 > 0.008 || diff_1 > 0.008 || diff_2 > 0.008)
                    {
                        // add the object pixel into the list
                        m_ForegroundColors.Add(color1);
                        _subtract_frame.SetPixel(u, v, color1);
                    }
                    else
                    {
                        _subtract_frame.SetPixel(u, v, new Color(1f, 1f, 1f, 0f));
                    }
                    
                    
                }
            }


            //FindHumanFootUV(_subtract_frame, out m_HumanLowestUV);

            //frame1.SetPixel((int)m_HumanLowestUV.x, (int)m_HumanLowestUV.y, new Color(1f, 0f, 0f, 1f));
            //frame1.SetPixel((int)m_HumanLowestUV.x + 1, (int)m_HumanLowestUV.y, new Color(1f, 0f, 0f, 1f));
            //frame1.SetPixel((int)m_HumanLowestUV.x - 1, (int)m_HumanLowestUV.y, new Color(1f, 0f, 0f, 1f));
            //frame1.SetPixel((int)m_HumanLowestUV.x, (int)m_HumanLowestUV.y + 1, new Color(1f, 0f, 0f, 1f));
            //frame1.SetPixel((int)m_HumanLowestUV.x, (int)m_HumanLowestUV.y - 1, new Color(1f, 0f, 0f, 1f));

            //_subtract_frame.Apply();
            //if (m_AnchorController.m_QuadAnchor != null)
            //{
            //    m_AnchorController.m_QuadAnchor.transform.GetChild(0).GetComponent<Renderer>().material.mainTexture = _subtract_frame;
            //}
        }
        else
        {
            Debug.Log($"----- false to read texture, please check texture setting ------");
            return null;
        }

        return _subtract_frame;
    }

    public void FindHumanFootUV(Texture2D frame, out Vector2 uv)
    {
        uv = Vector2.zero;
        int _tex_height = frame.height;
        int _tex_width = frame.width;

        for (int v = 0; v < _tex_height; v++)
        {
            for (int u = 0; u < _tex_width; u++)
            {
                if (frame.GetPixel(u, v) != new Color(1f, 1f, 1f, 0f) && v > uv.y)
                {
                    bool isArea = ConfirmArea(new Vector2(u, v), frame, 3);

                    if (isArea)
                    {
                        uv.x = u;
                        uv.y = v;
                        Debug.Log($"--highest point------- ({u},{v}) ----------");
                    }
                }
            }
        }
    }

    public bool ConfirmArea(Vector2 uv, Texture2D frame, int range)
    {
        int _white_area_num = 0;
        int _sum_num = (1 + 2 * range) * (1 + range);

        for (int v = (int)uv.y - range; v <= (int)uv.y; v++)
        {
            for (int u = (int)uv.x - range; u <= (int)uv.x + range; u++)
            {
                if (uv.x == u && uv.y == v)
                    continue;

                if (frame.GetPixel(u, v) == new Color(1f, 1f, 1f, 0f))
                {
                    _white_area_num++;
                }
            }
        }

        //Debug.Log($"--highest point--({uv.x},{uv.y})----- {frame.GetPixel((int)uv.x, (int)uv.y).ToString()} ----------");
        //Debug.Log($"--highest point--{_white_area_num}---");

        if ((float)_white_area_num / (float)_sum_num < 0.15)
            return true;
        else
            return false;
    }

    public void PlaybackCameraB()
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

        int index = (int)Mathf.Floor(playbackTime) % ControllerStates.FRAME_NUM;
        CameraBFrame frame = m_CameraBFrames[index];

        if (!frame.Equals(default(CameraBFrame)))
        {
            Vector2 uv = frame.footUV;
            Texture2D tex = frame.tex;
            Vector3 dirFromCamBToUV = Vector3.zero;

            // transform uv to world space
            TransformFromUVToWorldPoint(in uv, out dirFromCamBToUV);

            // generate ray
            Ray ray = new Ray(camera_pos, dirFromCamBToUV);

            // ray cast
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

        playbackTime += Time.deltaTime;
    }

    public void SetPlayBack()
    {
        if (m_IsPlayback)
            playbackTime = 0;

        m_IsPlayback = !m_IsPlayback;
    }

    public void InitializeCameraBVideoFrames()
    {
       for (int i = 0; i < ControllerStates.FOOTUVs.Length; i++)
       {
            Vector2 uv = ControllerStates.FOOTUVs[i];

            if (m_Frames[i] != null && uv != Vector2.zero)
            {
                CameraBFrame frame = new CameraBFrame(m_Frames[i], uv);
                m_CameraBFrames.Add(frame);
            }
        }
    }

}
