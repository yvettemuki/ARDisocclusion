using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ProjectorController : MonoBehaviour
{
    public RenderTexture BGSubOutputRTex;  // after background subtraction human sprite output
    public RenderTexture BGSubInputRTex;  // for video having human
    public Material bgSubMaterial;
    public Material mainCorridorProjectorMaterial;
    public Material sideCorridorProjectorMaterial;
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

    public void SetHumanRenderTexture(ARController.UserStudyType type)
    {
        //if (type == ARContorller.UserStudyType.TYPE_CUTAWAY)
        //    Graphics.Blit(BGSubInputRTex, BGSubOutputRTex, bgSubMaterial);
        //else if (type == ARContorller.UserStudyType.TYPE_MULTIPERSPECTIVE)
        //    Graphics.Blit(BGSubInputRTex, BGSubOutputRTex);
        Graphics.Blit(BGSubInputRTex, BGSubOutputRTex, bgSubMaterial);
    }

    public void SetMainCorridorProjectorMaterial(int srcMode, int dstMode)
    {
        mainCorridorProjectorMaterial.SetInt("_MySrcMode", srcMode);
        mainCorridorProjectorMaterial.SetInt("_MyDstMode", dstMode);
    }

    public void SetSideCorridorProjectorMaterial(int dstMode)
    {
        sideCorridorProjectorMaterial.SetInt("_MyDstMode", dstMode);
    }

}