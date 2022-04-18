using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class CameraImageController : MonoBehaviour
{
    [SerializeField]
    ARCameraBackground m_ARCameraBackground;

    public RenderTexture m_CameraARTex;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ProjectCameraA()
    {
        // output camera A capture to the render texture
        Graphics.Blit(null, m_CameraARTex, m_ARCameraBackground.material);
    }

    
}
