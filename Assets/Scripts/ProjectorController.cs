using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ProjectorController : MonoBehaviour
{
    public RenderTexture BGSubOutputRTex;  // after background subtraction human sprite output
    public RenderTexture BGSubInputRTex;  // for video having human
    public Material bgSubMaterial;
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
        if (humanSpriteFrame == null)
        {
            Debug.Log($"Human sprite texture can not be null!");
            return;
        }

        // the first paramenter is the _MainTex for shader input
        Graphics.Blit(humanSpriteFrame, BGSubOutputRTex, bgSubMaterial);
    }

    public void SetRenderTexture(ARContorller.UserStudyType type)
    {
        if (type == ARContorller.UserStudyType.TYPE_XRAY || type == ARContorller.UserStudyType.TYPE_OCCLUDED)
            Graphics.Blit(BGSubInputRTex, BGSubOutputRTex, bgSubMaterial);
        else if (type == ARContorller.UserStudyType.TYPE_TEXTURED)
            Graphics.Blit(BGSubInputRTex, BGSubOutputRTex);
    }    
}