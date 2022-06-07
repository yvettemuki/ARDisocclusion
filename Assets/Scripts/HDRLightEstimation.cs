using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;

public class HDRLightEstimation : MonoBehaviour
{
    [SerializeField]
    ARCameraManager m_CameraManager;

    public Material m_DymSphereMat;

    //[SerializeField]
    //Transform m_Arrow;

    //public Transform arrow
    //{
    //    get => m_Arrow;
    //    set => m_Arrow = value;
    //}

    public ARCameraManager cameraManager
    {
        get { return m_CameraManager; }
        set
        {
            if (m_CameraManager == value)
                return;

            if (m_CameraManager != null)
                m_CameraManager.frameReceived -= FrameChanged;

            m_CameraManager = value;

            if (m_CameraManager != null & enabled)
                m_CameraManager.frameReceived += FrameChanged;
        }
    }

    public static Vector3? mainLightDirection { get; private set; }

    public static Color? mainLightColor { get; private set; }

    void OnEnable()
    {
        if (m_CameraManager != null)
            m_CameraManager.frameReceived += FrameChanged;

        //// Disable the arrow to start; enable it later if we get directional light info
        //if (arrow)
        //{
        //    arrow.gameObject.SetActive(false);
        //}
        Application.onBeforeRender += OnBeforeRender;
    }

    void OnDisable()
    {
        Application.onBeforeRender -= OnBeforeRender;

        if (m_CameraManager != null)
            m_CameraManager.frameReceived -= FrameChanged;
    }

    void OnBeforeRender()
    {
        if (m_CameraManager)
        {
            var cameraTransform = m_CameraManager.GetComponent<Camera>().transform;
            //arrow.position = cameraTransform.position + cameraTransform.forward * .25f;
        }
    }

    void FrameChanged(ARCameraFrameEventArgs args)
    {
        if (args.lightEstimation.mainLightDirection.HasValue)
        {
            mainLightDirection = args.lightEstimation.mainLightDirection;
            //m_DymSphereMat.SetVector("_EstimateLightDir", new Vector4(mainLightDirection.Value.x, mainLightDirection.Value.y, mainLightDirection.Value.z, 0));
            //if (arrow)
            //{
            //    arrow.gameObject.SetActive(true);
            //    arrow.rotation = Quaternion.LookRotation(mainLightDirection.Value);
            //}
        }
        //else if (arrow)
        //{
        //    arrow.gameObject.SetActive(false);
        //    mainLightDirection = null;
        //}

        if (args.lightEstimation.mainLightColor.HasValue)
        {
            mainLightColor = args.lightEstimation.mainLightColor;
            m_DymSphereMat.SetColor("_EstimateLightColor", mainLightColor.Value);
            //Debug.Log($"-------------: {mainLightColor.Value.ToString()}");
        }
        else
        {
            mainLightColor = null;
        }
    }
}
