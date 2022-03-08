using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class ARCameraController : MonoBehaviour
{
    public ARCameraManager m_ARCameraManaegr;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetCurrentCameraImage(out XRCpuImage image)
    {
        if (m_ARCameraManaegr != null)
        {
            m_ARCameraManaegr.TryAcquireLatestCpuImage(out image);
        }

        image = default(XRCpuImage); 
    }
}
