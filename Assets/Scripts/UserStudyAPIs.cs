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

    private void InitClosestSphereByMethodAndTask(bool isStudyMode)
    {
        int currMethod = (int)ARController.currentUserStudyType;
        int currTaskMode = (int)UserStudyController.currentTaskMode;
        int currTaskIndex = 3 * (currMethod * 3 + (currTaskMode % 3));

        m_UserStudyController.InitClosestSphereGroup(currTaskIndex, isStudyMode);
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

        if (taskMode == UserStudyController.TaskMode.ClOSEST_SPHERE_GROUP_EASY
            || taskMode == UserStudyController.TaskMode.ClOSEST_SPHERE_GROUP_MEDIUM
            || taskMode == UserStudyController.TaskMode.ClOSEST_SPHERE_GROUP_HARD
        )
        {
            InitClosestSphereByMethodAndTask(ControllerStates.STUDY_MODE);
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
                m_ARController.m_AnchorController.m_CorridorAnchor.gameObject.SetActive(false);
                break;

            case ARController.UserStudyType.TYPE_CUTAWAY:
                m_ARController.m_AnchorController.m_CorridorAnchor.gameObject.SetActive(true);
                m_UserStudyController.SetUserStudyObjectsActive(true);
                break;

            case ARController.UserStudyType.TYPE_MULTIPERSPECTIVE:
                m_ARController.m_AnchorController.m_CorridorAnchor.gameObject.SetActive(false);
                m_ARController.SetCameraBNearClipToPortalDepth();
                m_ARController.SetProjectorMULTIActive(true);
                break;

            case ARController.UserStudyType.TYPE_PICINPIC:
                m_ARController.SetCameraBNearClipToDefaultDepth();
                m_ARController.m_AnchorController.m_CorridorAnchor.gameObject.SetActive(false);
                m_ARController.m_RawImagePicInPicInUserCanvas.gameObject.SetActive(true);
                m_ARController.m_RawImagePicInPicInTrainCanvas.gameObject.SetActive(true);
                m_ARController.CreateStencilMaskArea();
                break;

            case ARController.UserStudyType.TYPE_XRAY:
                m_ARController.m_AnchorController.m_CorridorAnchor.gameObject.SetActive(true);
                // dynamic sphere view
                m_UserStudyController.SetUserStudyObjectsActive(true);
                m_ARController.SetProjectorMAINCORDActive(true);
                m_ARController.m_ProjectorController.SetMainCorridorProjectorMaterial(1, 5);
                //m_ProjectorRightMAINCORD.gameObject.SetActive(true);
                break;

            case ARController.UserStudyType.TYPE_REFLECTION:
                m_ARController.m_AnchorController.m_CorridorAnchor.gameObject.SetActive(false);
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

        if (taskMode == UserStudyController.TaskMode.ClOSEST_SPHERE_GROUP_EASY
            || taskMode == UserStudyController.TaskMode.ClOSEST_SPHERE_GROUP_MEDIUM
            || taskMode == UserStudyController.TaskMode.ClOSEST_SPHERE_GROUP_HARD
        )
        {
            InitClosestSphereByMethodAndTask(ControllerStates.TRAIN_MODE);
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
        Vector3 correct_dir = (m_UserStudyController.GetCurrHumanPos() - m_ARController.m_ARCamera.transform.position).normalized;
        Vector3 user_dir = m_ARController.m_ARCamera.transform.forward;
        float diviation_angle = Mathf.Abs(Vector3.Angle(correct_dir, user_dir));
        //float diff_angle = (90f - Mathf.Abs(Vector3.Angle(correct_dir, user_dir))) / 90f * 100f;
        dataset = $"{correct_dir.ToString("0.00")}, {user_dir.ToString("0.00")}, {diviation_angle.ToString("0.00")}";
        //Debug.Log(dataset);
        
        return dataset;
    }

    public void SetNoneDisocclusinWithCrosshair()
    {
        UserStudyController.currentTaskMode = UserStudyController.TaskMode.NONE;
        ARController.currentUserStudyType = ARController.UserStudyType.TYPE_NONE;
        m_ARController.m_AnchorController.m_CorridorAnchor.gameObject.SetActive(false);
        m_ARController.CleanUpScene();
        m_UserStudyController.Reset();

        m_UserStudyController.m_CrosshairStudy.gameObject.SetActive(true);
        m_UserStudyController.m_CrosshairTrain.gameObject.SetActive(true);
    }


}
