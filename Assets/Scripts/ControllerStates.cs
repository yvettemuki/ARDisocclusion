using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerStates : MonoBehaviour
{
    public static float PORTAL_HEIGHT = 2.717906f;
    public static float PORTAL_WIDTH = 1.6f;          //1.519696f;

    // Camera B Calibration Data
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

    // Camera A Projector Data
    public static Vector3 PROJECTOR_MAIN_LEFT_CORD_POS_IN_PORTAL = new Vector3(-6.65f, 1.53f, -2.27f);  //-6.13f, 1.49f, -1.65f
    public static Quaternion PROJECTOR_MAIN_LEFT_CORD_ROT_IN_PORTAL = new Quaternion(0.05f, 0.54f, -0.05f, 0.84f);  //0.07f, 0.56f, -0.06f, 0.83f
    public static Vector3 PROJECTOR_MAIN_RIGHT_CORD_POS_IN_PORTAL = new Vector3(4.17f, 1.46f, -1.72f);
    public static Quaternion PROJECTOR_MAIN_RIGHT_CORD_ROT_IN_PORTAL = new Quaternion(0.09f, -0.48f, 0.02f, 0.87f);

    public static float HUMAN_HEIGHT = 1.62f;
    public static int HUMAN_HAND_SET_LOWEST_U = 290; // 282 280  290(1*1) 297 285
    public static int HUMAN_HAND_SET_LOWEST_V = 317; // 316 318  211(1*1) 166 192

    // Commons
    public static float GRAVITY_ACC = 9.8f;

    // User Study Preset Datas
    public static Vector3 CIRCLE_POS_IN_PORTAL = new Vector3(0f, 0f, 2.06f);
    public static Vector3 MIRROR_POS_IN_PORTAL = new Vector3(0f, 1.43f, -2.38f);
    public static Quaternion MIRROR_ROT_IN_PORTAL = Quaternion.Euler(0f, 360f, 0f);
    

    // Sphere Data
    // 3 Group 
    public static Vector3[] SPHERES_IN_PORTAL_POS_START =
    {
        new Vector3(-0.1f, 1.1f, 1.5f),
        new Vector3(0.13f, 0.5f, 3.4f),
        new Vector3(0.4f, 0.6f, 2f),
        new Vector3(-0.364f, 1.5f, 3.8f),
        new Vector3(0.51f, 0.388f, 1.97f),
        new Vector3(0.122f, 0.711f, 5.4f),
        new Vector3(0.194f, 1.55f, 5.08f)
    };
    public static float[] SPHERES_SPEED =
    {
        10f,
        5f,
        2f
    };
    
    public static Vector3 SPHERE_POS_IN_PORTAL_3 = new Vector3(-0.4f, 0.45f, 3.6f);

    // Human Direction Indicator
    public static Vector3 HUMAN_DIRECT_DIGIT_NUMBER = new Vector3(0.7f, 1.62f, 1.78f);
    public static Vector3[] USER_STUDY_DIRECT_INDI_FONT_UVs =
    {
        new Vector2(277, 334),
        new Vector2(277, 369),
        new Vector2(269, 257)
    };

    // Closest Sphere
    public static Vector3[] CLOSEST_SPHERE_GROUP_1 =
    {
        // Order: up, down, inner
        new Vector3(-0.941f, 1.578f, 0.1f),
        new Vector3(-0.941f, 1.031f, 0.1f),
        new Vector3(-0.65f, 1.4f, 1.484f)
    };

    public static Vector3[] CLOSEST_SPHERE_GROUP_2 =
    {
        new Vector3(-0.941f, 1.578f, 0.1f),
        new Vector3(-0.941f, 1.031f, 0.1f),
        new Vector3(-0.676f, 1.4f, 0.1f)
    };

    public static Vector3[] CLOSEST_SPHERE_GROUP_3 =
    {
        new Vector3(-0.941f, 1.578f, 0.1f),
        new Vector3(-0.941f, 1.266f, 0.1f),
        new Vector3(-0.676f, 1.4f, 0.1f)
    };

    // Find similar 
    public static Vector3[] FIND_SIMILAR_GROUPs =
    {
        new Vector3(-1.5f, 1.4f, -0.1f),
        new Vector3(-1.5f, 1.4f, -0.1f),
        new Vector3(-1.5f, 1.4f, -0.1f)
    };

    public enum PlaybackMode
    {
        PLAY_BACK_SEGMENT,
        PLAY_BACK_VIDEO_CLIP
    };

    public static int SEGMENT_FRAME_NUM = 5;
    public static Vector2[] FOOT_UVs= {
       new Vector2(290, 317),
       new Vector2(294, 369),
       new Vector2(290, 395),
       new Vector2(285, 346),
       new Vector2(281, 247)
    };


    // data old for segment
    //public static Vector2[] FOOTUVs = {
    //   new Vector2(284, 246),
    //   new Vector2(280, 318),
    //   new Vector2(280, 373),
    //   new Vector2(273, 335),
    //   new Vector2(280, 238)
    //};

    //public static int VIDEO_CLIP_FRAME_NUM = 14;
    //public static Vector2[] VIDEO_FOOT_UVs = {
    //   new Vector2(283, 266),
    //   new Vector2(287, 317),
    //   new Vector2(292, 349),
    //   new Vector2(292, 371),
    //   new Vector2(290, 384),
    //   new Vector2(290, 390),
    //   new Vector2(289, 398),
    //   new Vector2(291, 399),
    //   new Vector2(290, 395),
    //   new Vector2(289, 385),
    //   new Vector2(288, 373),
    //   new Vector2(288, 353),
    //   new Vector2(291, 328),
    //   new Vector2(285, 305)
    //};

    public static int VIDEO_CLIP_FRAME_NUM = 14;
    public static Vector2[] VIDEO_FOOT_UVs = {
        new Vector2(270, 232),
        new Vector2(281, 301),
        new Vector2(284, 339),
        new Vector2(276, 361),
        new Vector2(273, 377),
        new Vector2(272, 386),
        new Vector2(272, 392),
        new Vector2(280, 390),
        new Vector2(278, 383),
        new Vector2(276, 369),
        new Vector2(276, 356),
        new Vector2(276, 334),
        new Vector2(274, 303),
        new Vector2(266, 259)
    };

    // user study flow
    public static int MAX_TASK_NUM = 4;
    public static int MAX_METHOD_NUM = 5;
    public static int MAX_TRIAL_NUM = 3;
    
    public static int DYNAMIC_BALL = 0;
    public static int MATCH_SQUARE = 1;
    public static int DIRECTION_INDICATION = 2;
    public static int TWO_DOTS = 3;

    public static int P_IN_P = 0;
    public static int CUT_AWAY = 1;
    public static int X_RAY = 2;
    public static int MULTIVIEW = 3;
    public static int MIRROR = 4;

    public static string[] QUESTIONS = new string[]
    {
        "How many balls are moving in the corridor?",
        "Between which two stickers is the person standing?",
        "Which ball is closer to the pink one?",
        "Which two-digit number is the target number on the left wall?"
    };

    public static string[,,] CHOICES = new string[,,] 
    {
        {{"1", "2", "3", "4", "5"}, {"3", "4", "5", "6", "7" }, {"5", "6", "7", "8", "9" } },
        {{"1-2", "2-3", "3-4", "4-5", "5-6" }, {"1-2", "2-3", "3-4", "4-5", "5-6" }, {"1-2", "2-3", "3-4", "4-5", "5-6" } },
        {{"ph", "Blue", "ph", "Yellow", "ph" }, {"ph", "Blue", "ph", "Yellow", "ph" }, {"ph", "Blue", "ph", "Yellow", "ph" } },
        {{"Yellow", "Blue", "Green", "Pink", "Black" }, {"Yellow", "Blue", "Green", "Pink", "Black"}, {"Yellow", "Blue", "Green", "Pink", "Black"} } 
    };

    public static int[,,] CORRECT_ANSWERS = new int[,,]
    {
        { {2, 0, 3 }, {3, 4, 1 }, {1, 3, 2 }, {2, 1, 0 }, {0, 4, 4} },
        { {2, 0, 3 }, {3, 4, 1 }, {1, 3, 2 }, {2, 1, 0 }, {0, 4, 4} },
        { {2, 0, 3 }, {3, 4, 1 }, {1, 3, 2 }, {2, 1, 0 }, {0, 4, 4} },
        { {2, 0, 3 }, {3, 4, 1 }, {1, 3, 2 }, {2, 1, 0 }, {0, 4, 4} }
    };
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

