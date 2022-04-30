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
    public void SetUserStudyTask(UserStudyController.TaskMode taskMode)
    {
        m_UserStudyController.Reset();
        UserStudyController.currentTaskMode = taskMode;

        switch (taskMode)
        {
            case UserStudyController.TaskMode.COUNTING_DYNAMIC_SPHERE_3:
                m_UserStudyController.m_DynamicSphereNum = 3;
                m_UserStudyController.InitDynamicSpheres();
                break;

            case UserStudyController.TaskMode.COUNTING_DYNAMIC_SPHERE_5:
                m_UserStudyController.m_DynamicSphereNum = 5;
                m_UserStudyController.InitDynamicSpheres();
                break;

            case UserStudyController.TaskMode.COUNTING_DYNAMIC_SPHERE_7:
                m_UserStudyController.m_DynamicSphereNum = 7;
                m_UserStudyController.InitDynamicSpheres();
                break;

            case UserStudyController.TaskMode.DIRECT_INDICATOR_23:
                m_UserStudyController.InitDigitsNumbersForHumanDir();
                m_ARController.InitHumanSpriteForUserStudy(UserStudyController.TaskMode.DIRECT_INDICATOR_23);
                break;

            case UserStudyController.TaskMode.DIRECT_INDICATOR_56:
                m_UserStudyController.InitDigitsNumbersForHumanDir();
                m_ARController.InitHumanSpriteForUserStudy(UserStudyController.TaskMode.DIRECT_INDICATOR_56);
                break;

            case UserStudyController.TaskMode.DIRECT_INDICATOR_12:
                m_UserStudyController.InitDigitsNumbersForHumanDir();
                m_ARController.InitHumanSpriteForUserStudy(UserStudyController.TaskMode.DIRECT_INDICATOR_12);
                break;

            case UserStudyController.TaskMode.ClOSEST_SPHERE_GROUP_1:
                m_UserStudyController.InitClosestSphereGroup(UserStudyController.TaskMode.ClOSEST_SPHERE_GROUP_1);
                break;

            case UserStudyController.TaskMode.ClOSEST_SPHERE_GROUP_2:
                m_UserStudyController.InitClosestSphereGroup(UserStudyController.TaskMode.ClOSEST_SPHERE_GROUP_2);
                break;

            case UserStudyController.TaskMode.ClOSEST_SPHERE_GROUP_3:
                m_UserStudyController.InitClosestSphereGroup(UserStudyController.TaskMode.ClOSEST_SPHERE_GROUP_3);
                break;

            case UserStudyController.TaskMode.SIMILAR_GROUP_1:
                m_UserStudyController.InitSimilarGroup(UserStudyController.TaskMode.SIMILAR_GROUP_1);
                break;

            case UserStudyController.TaskMode.SIMILAR_GROUP_2:
                m_UserStudyController.InitSimilarGroup(UserStudyController.TaskMode.SIMILAR_GROUP_2);
                break;

            case UserStudyController.TaskMode.SIMILAR_GROUP_3:
                m_UserStudyController.InitSimilarGroup(UserStudyController.TaskMode.SIMILAR_GROUP_3);
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
                m_ARController.m_RawImagePicInPicInSetupCanvas.gameObject.SetActive(true);
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
