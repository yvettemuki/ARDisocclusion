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

    public void InitSphereByMethodAndMode()
    {
        int currMethod = (int)ARController.currentUserStudyType;
        int currTaskMode = (int)UserStudyController.currentTaskMode;
        int currTaskIndex = currMethod * 3 + currTaskMode;

        m_UserStudyController.InitDynamicSpheres(currTaskIndex);
    }

    public void SetUserStudyTask(UserStudyController.TaskMode taskMode)
    {
        m_UserStudyController.Reset();

        // should be set before the determination
        UserStudyController.currentTaskMode = taskMode;

        if (taskMode == UserStudyController.TaskMode.COUNTING_DYNAMIC_SPHERE_EASY
            || taskMode == UserStudyController.TaskMode.COUNTING_DYNAMIC_SPHERE_MEDIUM
            || taskMode == UserStudyController.TaskMode.COUNTING_DYNAMIC_SPHERE_HARD
        )
        {
            InitSphereByMethodAndMode();
        }

        switch (taskMode)
        {
            case UserStudyController.TaskMode.DIRECT_INDICATOR_EASY:
                m_UserStudyController.InitDigitsNumbersForHumanDir();
                m_ARController.InitHumanSpriteForUserStudy(UserStudyController.TaskMode.DIRECT_INDICATOR_EASY);
                break;

            case UserStudyController.TaskMode.DIRECT_INDICATOR_MEDIUM:
                m_UserStudyController.InitDigitsNumbersForHumanDir();
                m_ARController.InitHumanSpriteForUserStudy(UserStudyController.TaskMode.DIRECT_INDICATOR_MEDIUM);
                break;

            case UserStudyController.TaskMode.DIRECT_INDICATOR_HARD:
                m_UserStudyController.InitDigitsNumbersForHumanDir();
                m_ARController.InitHumanSpriteForUserStudy(UserStudyController.TaskMode.DIRECT_INDICATOR_HARD);
                break;

            case UserStudyController.TaskMode.ClOSEST_SPHERE_GROUP_EASY:
                m_UserStudyController.InitClosestSphereGroup(UserStudyController.TaskMode.ClOSEST_SPHERE_GROUP_EASY);
                break;

            case UserStudyController.TaskMode.ClOSEST_SPHERE_GROUP_MEDIUM:
                m_UserStudyController.InitClosestSphereGroup(UserStudyController.TaskMode.ClOSEST_SPHERE_GROUP_MEDIUM);
                break;

            case UserStudyController.TaskMode.ClOSEST_SPHERE_GROUP_HARD:
                m_UserStudyController.InitClosestSphereGroup(UserStudyController.TaskMode.ClOSEST_SPHERE_GROUP_HARD);
                break;

            case UserStudyController.TaskMode.SIMILAR_GROUP_EASY:
                m_UserStudyController.InitSimilarGroup(UserStudyController.TaskMode.SIMILAR_GROUP_EASY);
                break;

            case UserStudyController.TaskMode.SIMILAR_GROUP_MEDIUM:
                m_UserStudyController.InitSimilarGroup(UserStudyController.TaskMode.SIMILAR_GROUP_MEDIUM);
                break;

            case UserStudyController.TaskMode.SIMILAR_GROUP_HARD:
                m_UserStudyController.InitSimilarGroup(UserStudyController.TaskMode.SIMILAR_GROUP_HARD);
                break;

            default:
                break;
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
                // add user study task method disable?
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


}
