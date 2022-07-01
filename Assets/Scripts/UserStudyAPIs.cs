using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserStudyAPIs : MonoBehaviour
{
    [SerializeField]
    private UserStudyController m_UserStudyController;
    [SerializeField]
    private ARController m_ARController;

    void Start()
    {
        
    }

    void Awake()
    {
        //m_UserStudyController = GetComponent<UserStudyController>();
    }

    void Update()
    {
        
    }

    private void InitDynamicSphereByMethodAndTask(bool isStudyMode)
    {
        int currMethod = (int)ARController.currentUserStudyType;
        int currTaskMode = (int)UserStudyController.currentTaskMode;
        int currTaskIndex = currMethod * 3 + currTaskMode;  // currTaskMode % 3

        m_UserStudyController.InitDynamicSpheres(currTaskIndex, isStudyMode);
    }

    private void InitHumanByMethodAndTask(bool isStudyMode)
    {
        int currMethod = (int)ARController.currentUserStudyType;
        int currTaskMode = (int)UserStudyController.currentTaskMode;
        int currTaskIndex = currMethod * 3 + (currTaskMode % 3);

        m_ARController.InitHumanSpriteForUserStudy(currTaskIndex, isStudyMode);
    }

    private void InitClosestPatchByMethodAndTask(bool isStudyMode)
    {
        int currMethod = (int)ARController.currentUserStudyType;
        int currTaskMode = (int)UserStudyController.currentTaskMode;
        int currTaskIndex = 8 * (currMethod * 3 + (currTaskMode % 3));

        m_UserStudyController.InitClosestPatchGroup(currTaskIndex, isStudyMode);
    }

    private void InitSimilarDigitByMethodAndTask(bool isStudyMode)
    {
        int currMethod = (int)ARController.currentUserStudyType;
        int currTaskMode = (int)UserStudyController.currentTaskMode;
        int currTaskIndex = currMethod * 3 + (currTaskMode % 3);

        m_UserStudyController.InitSimilarGroup(currTaskIndex, isStudyMode);
    }

    public void SetUserStudyTask(UserStudyController.TaskMode taskMode)
    {
        m_UserStudyController.Reset();

        // should be SET BEFORE the determination
        UserStudyController.currentTaskMode = taskMode;

        if (taskMode == UserStudyController.TaskMode.COUNTING_DYNAMIC_SPHERE_EASY
            || taskMode == UserStudyController.TaskMode.COUNTING_DYNAMIC_SPHERE_MEDIUM
            || taskMode == UserStudyController.TaskMode.COUNTING_DYNAMIC_SPHERE_HARD
        )
        {
            InitDynamicSphereByMethodAndTask(ControllerStates.STUDY_MODE);
        }

        if (taskMode == UserStudyController.TaskMode.DIRECT_INDICATOR_EASY
            || taskMode == UserStudyController.TaskMode.DIRECT_INDICATOR_MEDIUM
            || taskMode == UserStudyController.TaskMode.DIRECT_INDICATOR_HARD
        )
        {
            //m_UserStudyController.InitDigitsNumbersForHumanDir();
            InitHumanByMethodAndTask(ControllerStates.STUDY_MODE);
        }

        if (taskMode == UserStudyController.TaskMode.ClOSEST_PATCH_GROUP_EASY
            || taskMode == UserStudyController.TaskMode.ClOSEST_PATCH_GROUP_MEDIUM
            || taskMode == UserStudyController.TaskMode.ClOSEST_PATCH_GROUP_HARD
        )
        {
            InitClosestPatchByMethodAndTask(ControllerStates.STUDY_MODE);
        }

        if (taskMode == UserStudyController.TaskMode.SIMILAR_GROUP_EASY
            || taskMode == UserStudyController.TaskMode.SIMILAR_GROUP_MEDIUM
            || taskMode == UserStudyController.TaskMode.SIMILAR_GROUP_HARD
        )
        {
            InitSimilarDigitByMethodAndTask(ControllerStates.STUDY_MODE);
        }
    }

    public void SetUserStudyMethod(ARController.UserStudyType methodMode)
    {
        m_ARController.CleanUpScene();
        ARController.currentUserStudyType = methodMode;

        switch (methodMode)
        {
            case ARController.UserStudyType.TYPE_NONE:
                m_ARController.m_AnchorController.m_Corridor.SetActive(false);
                break;

            case ARController.UserStudyType.TYPE_CUTAWAY:
                m_ARController.m_AnchorController.m_Corridor.SetActive(true);
                m_UserStudyController.SetUserStudyObjectsActive(true);
                break;

            case ARController.UserStudyType.TYPE_MULTIPERSPECTIVE:
                m_ARController.m_AnchorController.m_Corridor.SetActive(false);
                m_ARController.SetCameraBNearClipToPortalDepth();
                m_ARController.SetProjectorMULTIActive(true);
                break;

            case ARController.UserStudyType.TYPE_PICINPIC:
                m_ARController.SetCameraBNearClipToDefaultDepth();
                m_ARController.m_AnchorController.m_Corridor.SetActive(false);
                m_ARController.m_RawImagePicInPicInUserCanvas.gameObject.SetActive(true);
                m_ARController.m_RawImagePicInPicInTrainCanvas.gameObject.SetActive(true);
                m_ARController.CreateStencilMaskArea();
                break;

            case ARController.UserStudyType.TYPE_XRAY:
                m_ARController.m_AnchorController.m_Corridor.SetActive(true);
                // dynamic sphere view
                m_UserStudyController.SetUserStudyObjectsActive(true);
                m_ARController.SetProjectorMAINCORDActive(true);
                m_ARController.m_ProjectorController.SetMainCorridorProjectorMaterial(1, 5);
                //m_ProjectorRightMAINCORD.gameObject.SetActive(true);
                break;

            case ARController.UserStudyType.TYPE_REFLECTION:
                m_ARController.m_AnchorController.m_Corridor.SetActive(false);
                m_ARController.SetProjectorMAINCORDActive(true);
                m_ARController.m_ProjectorController.SetMainCorridorProjectorMaterial(1, 0);
                m_ARController.CreateStencilMaskArea();
                break;

            default:
                break;
        }
    }

    public void SetTrainTask(UserStudyController.TaskMode taskMode)
    {
        m_UserStudyController.Reset();

        // should be SET BEFORE the determination
        UserStudyController.currentTaskMode = taskMode;

        if (taskMode == UserStudyController.TaskMode.COUNTING_DYNAMIC_SPHERE_EASY
            || taskMode == UserStudyController.TaskMode.COUNTING_DYNAMIC_SPHERE_MEDIUM
            || taskMode == UserStudyController.TaskMode.COUNTING_DYNAMIC_SPHERE_HARD
        )
        {
            InitDynamicSphereByMethodAndTask(ControllerStates.TRAIN_MODE);
        }

        if (taskMode == UserStudyController.TaskMode.DIRECT_INDICATOR_EASY
            || taskMode == UserStudyController.TaskMode.DIRECT_INDICATOR_MEDIUM
            || taskMode == UserStudyController.TaskMode.DIRECT_INDICATOR_HARD
        )
        {
            //m_UserStudyController.InitDigitsNumbersForHumanDir();
            InitHumanByMethodAndTask(ControllerStates.TRAIN_MODE);
        }

        if (taskMode == UserStudyController.TaskMode.ClOSEST_PATCH_GROUP_EASY
            || taskMode == UserStudyController.TaskMode.ClOSEST_PATCH_GROUP_MEDIUM
            || taskMode == UserStudyController.TaskMode.ClOSEST_PATCH_GROUP_HARD
        )
        {
            InitClosestPatchByMethodAndTask(ControllerStates.TRAIN_MODE);
        }

        if (taskMode == UserStudyController.TaskMode.SIMILAR_GROUP_EASY
            || taskMode == UserStudyController.TaskMode.SIMILAR_GROUP_MEDIUM
            || taskMode == UserStudyController.TaskMode.SIMILAR_GROUP_HARD
        )
        {
            InitSimilarDigitByMethodAndTask(ControllerStates.TRAIN_MODE);
        }
    }

    public string GetDirectIndicateAccuracy()
    {
        string dataset = null;
        //Vector3 correct_dir = (m_UserStudyController.GetCurrHumanPos() - m_ARController.m_ARCamera.transform.position).normalized;
        //Vector3 user_dir = m_ARController.m_ARCamera.transform.forward;
        //float deviation_angle = Mathf.Abs(Vector3.Angle(correct_dir, user_dir));
        //float diff_angle = (90f - Mathf.Abs(Vector3.Angle(correct_dir, user_dir))) / 90f * 100f;
        //dataset = $"{correct_dir.x.ToString("0.00")}#{correct_dir.y.ToString("0.00")}#{correct_dir.z.ToString("0.00")}," +
        //    $"{user_dir.x.ToString("0.00")}#{user_dir.y.ToString("0.00")}#{user_dir.z.ToString("0.00")}," +
        //    $"{deviation_angle.ToString("0.00")}";

        // User position with respect to the portal
        Vector3 user_position_in_portal = Vector3.zero;
        Vector3 user_position_in_world = m_ARController.m_ARCamera.transform.position;
        m_ARController.WorldObjectPos2Portal(in user_position_in_world, out user_position_in_portal);

        // User direction with respect to the portal
        Quaternion user_rotation_in_portal = Quaternion.identity;
        Transform user_transform_in_world = m_ARController.m_ARCamera.transform;
        m_ARController.WorldObjectRot2Portal(in user_transform_in_world, out user_rotation_in_portal);
        Vector3 user_direction_in_portal = user_rotation_in_portal * Vector3.forward;

        // Human sprite depth(z) with respect to the portal
        Vector3 human_sprite_position_in_portal = Vector3.zero;
        Vector3 human_sprite_position_in_world = m_UserStudyController.GetCurrHumanPos();
        m_ARController.WorldObjectPos2Portal(in human_sprite_position_in_world, out human_sprite_position_in_portal);
        float human_sprite_depth_in_portal = human_sprite_position_in_portal.z;

        dataset = $"{user_position_in_portal.x.ToString("0.000")}#{user_position_in_portal.y.ToString("0.000")}#{user_position_in_portal.z.ToString("0.000")}," +
            $"{user_direction_in_portal.x.ToString("0.000")}#{user_direction_in_portal.y.ToString("0.000")}#{user_direction_in_portal.z.ToString("0.000")}," +
            $"{human_sprite_position_in_portal.x.ToString("0.000")}#{human_sprite_position_in_portal.y.ToString("0.000")}#{human_sprite_position_in_portal.z.ToString("0.000")}";
        //Debug.Log(dataset);
        
        return dataset;
    }

    public void SetNoneDisocclusinWithCrosshair()
    {
        UserStudyController.currentTaskMode = UserStudyController.TaskMode.NONE;
        ARController.currentUserStudyType = ARController.UserStudyType.TYPE_NONE;
        m_ARController.m_AnchorController.m_Corridor.SetActive(false);
        m_ARController.CleanUpScene();
        m_UserStudyController.Reset();

        m_UserStudyController.m_CrosshairStudy.gameObject.SetActive(true);
        m_UserStudyController.m_CrosshairTrain.gameObject.SetActive(true);
    }


}
