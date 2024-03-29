using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerStates : MonoBehaviour
{
    public static float PORTAL_HEIGHT = 2.717906f;
    public static float PORTAL_WIDTH = 1.6f;

    // Camera B Calibration Data
    public static float CAM_B_HFOV = 67.6615f;  // horizontal field of view!!!
    public static float CAM_B_VFOV = 95.00f;
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

    //public static float CAM_B_HFOV = 60.3043f;  // horizontal field of view!!!
    //public static float CAM_B_VFOV = 94.007f;
    //public static float CAM_RATIO = 0.624866f;
    //public static Vector3 CAM_B_EYE_POS = new Vector3(0.736091f, 1.57838f, -2.17946f); //-z
    //public static Vector3 CAM_B_LOOK_AT_POINT = new Vector3(0.76831f, 1.58019f, -1.17998f); //-z
    //public static Vector3 CAM_B_UP = new Vector3(0.0182915f, 0.99983f, -0.00240036f); //-z
    //public static float Z_NEAR = 0.10f;
    //public static float Z_FAR = 100f;
    //public static Vector3 PORTAL_BOTTOM_LEFT = new Vector3(0.0f, 0.0f, 0.0f);
    //public static Vector3 PORTAL_BOTTOM_RIGHT = new Vector3(1.6f, 0.0f, 0.0f);
    //public static Vector3 PORTAL_TOP_LEFT = new Vector3(0.0f, 2.72f, 0.0f);
    //public static Vector3 PORTAL_TOP_RIGHT = new Vector3(1.6f, 2.72f, 0.0f);

    // Camera A Projector Data
    public static Vector3 PROJECTOR_MAIN_LEFT_CORD_POS_IN_PORTAL = new Vector3(-6.576f, 1.602f, -1.981f);  //-7.61f, 1.49f, -1.96f
    public static Quaternion PROJECTOR_MAIN_LEFT_CORD_ROT_IN_PORTAL = new Quaternion(0.07f, 0.58f, -0.06f, 0.81f);  //0.06f, 0.59f, -0.05f, 0.81f
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
    // 2-6 spheres
    public static Vector3[] DYM_SPHERES_POS_IN_PORTAL =
    {
        new Vector3(0.332f, 1.559f, 0.771f),
        new Vector3(0.397f, 1.537f, 2.618f),
        new Vector3(-0.321f, 1.236f, 3.52f),
        new Vector3(-0.321f, 0.74f, 1.06f),
        new Vector3(-0.083f, 1.77f, 2.13f),
        new Vector3(0.166f, 0.939f, 0.611f),
        new Vector3(-0.104f, 0.59f, 2.89f),
        new Vector3(-0.321f, 1.7f, 4.61f),    
        new Vector3(-0.25f, 0.84f, 3.93f),       
        new Vector3(-0.321f, 2f, 2.95f),  
        new Vector3(-0.44f, 0.91f, 4.68f), 
        new Vector3(-0.097f, 1.236f, 2.09f), 
        new Vector3(0.44f, 0.85f, 1.87f),
        new Vector3(0.09f, 1.86f, 1.32f),
        new Vector3(0.03f, 0.83f, 3.08f), //15
        new Vector3(0.12f, 1.5f, 3.74f), //16
        new Vector3(0.69f, 1.236f, 4.78f), //17
        new Vector3(-0.037f, 0.57f, 0.93f), //18
        new Vector3(0.111f, 1.252f, 1.556f), //19
    };

    // Change
    //3, 7, 8,  // CA
    //    4, 6, 9,  // MP
    //    4, 5, 10,  // PP
    //    3, 5, 11,  // XR
    //    5, 6, 8,  // MI
    public static int[] DYN_SPHERES_NUM =
    {
        4, 6, 10,  // CA
        6, 5, 9,  // MP
        5, 4, 11,  // PP
        4, 6, 10,  // XR
        6, 4, 10,  // MI
    };
    public static int[] TRAIN_DYN_SPHERES_NUM =
    {
        6, 5, 11,  // CA
        6, 4, 10,  // MP
        4, 6, 10,  // PP
        5, 5, 11,  // XR
        6, 4, 10,  // MI
    };
    public static Color[] DYN_SPHERE_COLOR =
    {
        new Color(0.972f, 0.215f, 0.372f, 1.0f),
        new Color(0.329f, 0.509f, 0.960f, 1.0f),
        new Color(0.960f, 0.839f, 0, 1.0f)
    };

    // Human Direction Indicator
    public static Vector3 HUMAN_DIRECT_DIGIT_NUMBER = new Vector3(-1.318f, 0f, 0f);
    public static Vector3[] USER_STUDY_DIRECT_INDI_FONT_UVs =
    {
        // CA
        new Vector2(277, 334), // 11
        new Vector2(277, 369), // 9
        new Vector2(269, 257), // 13
        
        // MP // Change
        new Vector2(278, 359),  // 3
        //new Vector2(277, 334), // 11
        new Vector2(283, 298),  // 1
        new Vector2(277, 383),  // 8

        // PP // Change
        new Vector2(276, 356),  // 10
        //new Vector2(277, 334), // 11
        new Vector2(275, 303),  // 12
        new Vector2(272, 388),  // 5

        // XR // Change
        new Vector2(285, 339),  // 2
        //new Vector2(277, 334), // 11
        new Vector2(271, 392),  // 6
        new Vector2(269, 257),  // 13

        // MI
        new Vector2(277, 383),  // 8
        new Vector2(285, 339),  // 2
        new Vector2(283, 298),  // 1
    };
    public static Vector3[] TRAIN_DIRECT_INDI_FONT_UVs =
    {
        // CA
        new Vector2(277, 383),  // 8
        new Vector2(285, 339),  // 2
        new Vector2(272, 388),  // 5
        
        // MP
        new Vector2(269, 257),  // 13
        new Vector2(271, 392),  // 6
        new Vector2(283, 298),  // 1

        // PP
        new Vector2(277, 383),  // 8
        new Vector2(276, 356),  // 10
        new Vector2(283, 298),  // 1

        // XR
        new Vector2(277, 369), // 9
        new Vector2(277, 334), // 11
        new Vector2(277, 383),  // 8

        // MI
        new Vector2(272, 388),  // 5
        new Vector2(275, 303),  // 12
        new Vector2(276, 356),  // 10
    };

    // Closest Sphere (old)
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


    public static Vector3[] CLOSEST_PATCH_GROUPs =
    {
        // CA 
        // Red
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 2.051f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(0.676f, 0.903f, 2.348f),

        // Green
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 2.051f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(0.676f, 0.57f, 0.74f),

        // Yellow
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 2.051f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(0.676f, 0.748f, 0.3f),

        // MP
        // Green
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 2.051f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(0.676f, 0.66f, 1.154f),

        // Blue
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 2.051f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(0.676f, 0.903f, 2.85f),

        // Yellow
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 2.051f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(0.676f, 0.6f, 0.3f),

        // PP
        // Blue
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 2.051f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(0.676f, 0.72f, 2.8f),

        // Yellow
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 2.051f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(0.676f, 0.73f, 0.3f),

        // White
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 2.051f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(0.676f, 1.668f, 0.482f),

        // XR
        // Yellow
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 2.051f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(0.676f, 0.83f, 0.37f),

        // White
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 2.051f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(0.676f, 0.438f, 1.273f),

        // Red
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 2.051f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(0.676f, 1.18f, 2.67f),

        // MI
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 2.051f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(-0.676f, 0.992f, 0.655f),

        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 2.051f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(-0.676f, 1.589f, 0.655f),

        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 2.051f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(-0.676f, 1.276f, 0.655f),

    };

    public static Vector3[] TRAIN_CLOSEST_PATCH_GROUPs =
    {
        // CA 
        // Black
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 0.38f, 0.991f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(0.676f, 1.31f, 0.25f),

        // Red
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 0.38f, 0.991f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(0.676f, 1.14f, 2.348f),

        // Purple
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 0.38f, 0.991f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(0.676f, 1.42f, 0.74f),

        // MP
        // Blue
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 0.38f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(0.676f, 0.903f, 2.85f),

        // Black
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 0.38f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(0.676f, 1.35f, 0.25f),

        // Green
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 0.38f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(0.676f, 0.39f, 1.53f),

        // PP
        // Black
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 0.38f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(0.676f, 1.38f, 0.24f),

        // Purple
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 0.38f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(0.676f, 1.74f, 0.482f),

        // Blue
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 0.38f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(0.676f, 0.72f, 2.91f),

        // XR
        // White
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 0.38f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(0.676f, 0.36f, 1.08f),

        // Red
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 0.38f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(0.676f, 1.18f, 2.58f),

        // Black
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 0.38f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(0.676f, 1.37f, 0.25f),

        // MI
        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 0.38f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(-0.676f, 0.992f, 0.655f),

        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 0.38f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(-0.676f, 1.589f, 0.655f),

        new Vector3(-1.173f, 1.468f, -0.03f),
        new Vector3(-1.173f, 0.857f, -0.03f),
        new Vector3(-0.77f, 0.38f, 0.591f),
        new Vector3(-0.77f, 0.369f, 1.5f),
        new Vector3(-0.77f, 1.927f, 1.251f),
        new Vector3(0.676f, 1.734f, 2.583f),
        new Vector3(0.676f, 1.02f, 3.467f),
        new Vector3(-0.676f, 1.276f, 0.655f),

    };

    // Find similar 
    public static Vector3 SIMILAR_DIGIT_GROUP_POS = new Vector3(-1.5f, 1.4f, -0.1f);
    public static int SIMILAR_DIGIT_NUM_PER_GROUP = 18;  // including the target one (16 + 5 back face display for left side)
    public static int SIMILAR_DIGIT_START_INDEX = 3;  // including the target one
    public static int SIMILAR_DIGIT_NUM_LEFT_WALL = 13;
    public static Vector3[] FIND_SIMILAR_GROUPs =
    {
        // MI
        new Vector3(-1.5f, 1.4f, -0.1f),
        new Vector3(-1.5f, 1.4f, -0.1f),
        new Vector3(-1.5f, 1.4f, -0.1f),

        new Vector3(-0.941f, 1.578f, 0.1f),
        new Vector3(-0.941f, 1.031f, 0.1f),
        new Vector3(-0.676f, 1.4f, 0.1f),

        new Vector3(-0.941f, 1.578f, 0.1f),
        new Vector3(-0.941f, 1.266f, 0.1f),
        new Vector3(-0.676f, 1.4f, 0.1f),
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
    public static int MAX_METHOD_NUM = 4;
    public static int MAX_TRIAL_NUM = 3;
    
    public static int DYNAMIC_BALL = 0;
    public static int DIRECTION_INDICATION = 1;
    public static int FIND_CLOSEST = 2;
    public static int MATCH_NUM = 3;

    public static int P_IN_P = 0;
    public static int CUT_AWAY = 1;
    public static int X_RAY = 2;
    public static int MULTIVIEW = 3;
    public static int MIRROR = 4;

    public static string[] QUESTIONS = new string[]
    {
        "Count the floating spheres",
        "Between which two stickers is the person standing?",
        "Which square is closest to the pink disk?",
        "Which number appears on two different stickers?"
    };

    public static bool TRAIN_MODE = false;
    public static bool STUDY_MODE = true;

    public static string[,,] CHOICES = new string[,,] 
    {
        {{"3", "4", "5", "6", "7"}, {"3", "4", "5", "6", "7" }, {"8", "9", "10", "11", "12" } },
        {{"1-2", "2-3", "3-4", "4-5", "5-6" }, {"1-2", "2-3", "3-4", "4-5", "5-6" }, {"1-2", "2-3", "3-4", "4-5", "5-6" } },
        {{"Green", "ph", "Yellow", "ph", "None" }, {"Green", "ph", "Yellow", "ph", "None" }, {"Green", "ph", "Yellow", "ph", "None" } },
        {{"Yellow", "Blue", "Green", "Pink", "Black" }, {"Yellow", "Blue", "Green", "Pink", "Black"}, {"Yellow", "Blue", "Green", "Pink", "Black"} } 
    };

    public static string[] CHOICES_CLOSEST = new string[]
    {
        "Black", "Yellow", "White", "Green", "Purple", "Red", "Blue"
    };

    public static string[,] CHOICES_SIMILARITY = new string[,]
    {
        {"12", "14", "16", "17", "18", "22", "26", "31", "32", "88", "91", "99", "None" },
        {"11", "12", "14", "17", "18", "26", "31", "32", "45", "61", "91", "99", "None" },
        {"11", "12", "16", "17", "21", "26", "31", "32", "45", "61", "88", "99", "None" },
        {"12", "14", "16", "17", "21", "22", "31", "32", "45", "61", "91", "99", "None" },
        {"11", "12", "18", "21", "26", "31", "32", "45", "61", "88", "91", "99", "None" },
        {"11", "12", "14", "16", "17", "26", "31", "45", "61", "88", "91", "99", "None" },
        {"11", "14", "16", "17", "22", "26", "31", "32", "45", "61", "91", "99", "None" },
        {"12", "16", "17", "18", "21", "22", "31", "32", "45", "61", "91", "99", "None" },
        {"11", "12", "16", "17", "18", "21", "22", "26", "31", "32", "45", "91", "None" },
        {"11", "12", "14", "16", "18", "21", "22", "26", "31", "32", "88", "99", "None" },
        {"11", "12", "14", "16", "17", "21", "26", "32", "45", "61", "88", "91", "None" },
        {"11", "12", "14", "16", "17", "21", "26", "31", "32", "61", "88", "91", "None" },
        {"11", "12", "14", "16", "17", "18", "21", "22", "26", "31", "32", "45", "None" },
        {"11", "12", "14", "16", "17", "18", "21", "22", "26", "31", "32", "45", "None" },
        {"11", "12", "14", "16", "17", "18", "21", "22", "26", "31", "32", "45", "None" }
    };

    public static string[,] CHOICES_SIMILARITY_TRAIN = new string[,]
    {
        {"11", "12", "16", "17", "18", "21", "26", "31", "45", "88", "91", "99", "None" },
        {"11", "12", "14", "16", "17", "26", "31", "32", "45", "61", "91", "99", "None" },
        {"11", "12", "16", "17", "21", "26", "31", "32", "45", "61", "88", "99", "None" },
        {"11", "12", "14", "16", "17", "21", "31", "32", "45", "88", "91", "99", "None" },
        {"11", "12", "14", "16", "17", "21", "26", "31", "32", "88", "91", "99", "None" },
        {"11", "12", "14", "16", "17", "26", "31", "45", "61", "88", "91", "99", "None" },
        {"11", "12", "14", "16", "17", "26", "31", "32", "45", "61", "91", "99", "None" },
        {"12", "14", "16", "17", "21", "22", "31", "32", "45", "61", "91", "99", "None" },
        {"11", "12", "18", "21", "22", "26", "31", "32", "45", "61", "91", "99", "None" },
        {"11", "12", "16", "17", "18", "21", "22", "26", "31", "32", "45", "91", "None" },
        {"11", "14", "16", "17", "22", "26", "31", "32", "45", "61", "91", "99", "None" },
        {"12", "16", "17", "18", "21", "22", "31", "32", "45", "61", "91", "99", "None" },
        {"11", "12", "14", "16", "17", "18", "21", "22", "26", "31", "32", "45", "None" },
        {"11", "12", "14", "16", "17", "18", "21", "22", "26", "31", "32", "45", "None" },
        {"11", "12", "14", "16", "17", "18", "21", "22", "26", "31", "32", "45", "None" }
    };

    public static int[,,] CORRECT_ANSWERS = new int[,,]
    {
        { {2, 0, 3 }, {3, 4, 1 }, {1, 3, 2 }, {2, 1, 0 }, {0, 4, 4} },
        { {2, 0, 3 }, {3, 4, 1 }, {1, 3, 2 }, {2, 1, 0 }, {0, 4, 4} },
        { {2, 0, 3 }, {3, 4, 1 }, {1, 3, 2 }, {2, 1, 0 }, {0, 4, 4} },
        { {2, 3, 3 }, {1, 1, 3 }, {1, 3, 1 }, {2, 1, 2 }, {1, 2, 0} }
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

