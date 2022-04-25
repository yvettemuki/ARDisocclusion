using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UserStudyController))]
public class UserStudyAPIs : MonoBehaviour
{
    UserStudyController m_UserStudyController;
    int m_DynamicSphereNum = 0;

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
        // Clean up the Object
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


}
