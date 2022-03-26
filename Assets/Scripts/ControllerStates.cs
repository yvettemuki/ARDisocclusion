using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerStates : MonoBehaviour
{
    public static float PORTAL_HEIGHT = 2.717906f;
    public static float PORTAL_WIDTH = 1.6f;          //1.519696f;

    public static float CAM_B_HFOV = 67.6615f;  // horizontal field of view!!!
    public static float CAM_B_VFOV = 94.007f;
    public static float CAM_RATIO = 0.624866f;
    public static Vector3 CAM_B_EYE_POS = new Vector3(-0.0303495f, 0.183799f, -0.113018f); //-z
    public static Vector3 CAM_B_LOOK_AT_POINT = new Vector3(-0.0439344f, 0.190594f, 0.886866f); //-z
    public static Vector3 CAM_B_UP = new Vector3(0.00495004f, 0.999965f, -0.0067288f); //-z
    public static float Z_NEAR = 0.10f;
    public static float Z_FAR = 100f;
    public static Vector3 PORTAL_BOTTOM_LEFT = new Vector3(-0.9f, -1.2f, 1.8f);
    public static Vector3 PORTAL_BOTTOM_RIGHT = new Vector3(0.7f, -1.2f, 1.8f);
    public static Vector3 PORTAL_TOP_LEFT = new Vector3(-0.9f, 1.5f, 1.8f);
    public static Vector3 PORTAL_TOP_RIGHT = new Vector3(0.7f, 1.5f, 1.8f);

    public static float HUMAN_HEIGHT = 1.62f;
    public static int HUMAN_HAND_SET_LOWEST_U = 290; // 282 280  290(1*1) 297 285
    public static int HUMAN_HAND_SET_LOWEST_V = 317; // 316 318  211(1*1) 166 192

    public enum PlaybackMode
    {
        PLAY_BACK_SEGMENT,
        PLAY_BACK_VIDEO_CLIP
    };

    public static int FRAME_NUM = 5;
    public static Vector2[] FOOTUVs= {
       new Vector2(290, 317),
       new Vector2(294, 369),
       new Vector2(290, 395),
       new Vector2(285, 346),
       new Vector2(281, 247)
    };

    // data old
    //public static Vector2[] FOOTUVs = {
    //   new Vector2(284, 246),
    //   new Vector2(280, 318),
    //   new Vector2(280, 373),
    //   new Vector2(273, 335),
    //   new Vector2(280, 238)
    //};
}

/** Data of Camera B by Extrinsic Calibration (Right Coord System)**/
// hfov: 67.6615
// eye: -0.0303495 0.183799 0.113018
// look at point: -0.0439344 0.190594 -0.886866
// up vector: 0.00495004 0.999965 0.0067288

/** Portal Data of Camera B by Extrinsic Calibration in Unity World Space For the test time **/
//bottom left(-0.9, -1.2, 1.8)
//bottom right(0.7, -1.2, 1.8)
//top left(-0.9, 1.5, 1.8)
//top right(0.7, 1.5, 1.8)

