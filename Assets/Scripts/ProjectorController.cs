using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ProjectorController : MonoBehaviour
{
    public RenderTexture BGSubOutputRTex;  // after background subtraction human sprite output
    public Material material;
    public RenderTexture BGSubInputRTex;  // for video having human
    public VideoPlayer videoPlayer;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = BGSubInputRTex;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetRenderTexture(Texture2D humanSpriteFrame)
    {
        // the first paramenter is the _MainTex for shader input
        Graphics.Blit(humanSpriteFrame, BGSubOutputRTex, material);
    }

    public void SetRenderTexture()
    {
        Graphics.Blit(BGSubInputRTex, BGSubOutputRTex, material);
    }    
}