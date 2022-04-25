using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UserStudyController))]
public class UserStudyAPIs : MonoBehaviour
{
    UserStudyController m_UserStudyController;
    ARContorller m_ARController;

    void Start()
    {
        
    }

    void Awake()
    {
        m_UserStudyController = GetComponent<UserStudyController>();
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

            default:
                break;
        }
    }

    public void SetUserStudyMethod(ARContorller.UserStudyType methodMode)
    {
        m_ARController.CleanUpScene();
        ARContorller.currentUserStudyType = methodMode;

        switch (methodMode)
        {
            case ARContorller.UserStudyType.TYPE_NONE:
                m_ARController.m_AnchorController.m_CorridorAnchor.gameObject.SetActive(false);
                // add user study task method disable?
                break;

            case ARContorller.UserStudyType.TYPE_CUTAWAY:
                m_ARController.m_AnchorController.m_CorridorAnchor.gameObject.SetActive(true);
                m_UserStudyController.SetUserStudyObjectsActive(true);
                break;

            case ARContorller.UserStudyType.TYPE_MULTIPERSPECTIVE:
                m_ARController.m_AnchorController.m_CorridorAnchor.gameObject.SetActive(false);
                m_ARController.SetCameraBNearClipToPortalDepth();
                m_ARController.SetProjectorMULTIActive(true);
                break;

            case ARContorller.UserStudyType.TYPE_PICINPIC:
                m_ARController.SetCameraBNearClipToDefaultDepth();
                m_ARController.m_AnchorController.m_CorridorAnchor.gameObject.SetActive(false);
                m_ARController.m_RawImagePicInPic.gameObject.SetActive(true);
                m_ARController.CreateStencilMaskArea();
                break;

            case ARContorller.UserStudyType.TYPE_XRAY:
                m_ARController.m_AnchorController.m_CorridorAnchor.gameObject.SetActive(true);
                // dynamic sphere view
                m_UserStudyController.SetUserStudyObjectsActive(true);
                m_ARController.SetProjectorMAINCORDActive(true);
                m_ARController.m_ProjectorController.SetMainCorridorProjectorMaterial(1, 5);
                //m_ProjectorRightMAINCORD.gameObject.SetActive(true);
                break;

            case ARContorller.UserStudyType.TYPE_REFLECTION:
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
