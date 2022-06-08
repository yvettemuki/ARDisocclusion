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

    public static Vector3? mainLightDirection = Vector3.zero;

    public static Color? mainLightColor = Color.white;

    void OnEnable()
    {
        if (m_CameraManager != null)
            m_CameraManager.frameReceived += FrameChanged;

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
            m_DymSphereMat.SetVector("_EstimateLightDir", new Vector4(mainLightDirection.Value.x, mainLightDirection.Value.y, mainLightDirection.Value.z, 0));
        }
        else
        {
            mainLightDirection = null;
            m_DymSphereMat.SetVector("_EstimateLightDir", new Vector4(0, 0, 0, 0));
        }

        if (args.lightEstimation.mainLightColor.HasValue)
        {
            mainLightColor = args.lightEstimation.mainLightColor;
            m_DymSphereMat.SetColor("_EstimateLightColor", mainLightColor.Value);
        }
        else
        {
            mainLightColor = null;
            m_DymSphereMat.SetColor("_EstimateLightColor", new Color(0.1f, 0.1f, 0.1f, 1));
        }
    }
}
