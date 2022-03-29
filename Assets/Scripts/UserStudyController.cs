using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ARContorller))]
public class UserStudyController : MonoBehaviour
{
    ARContorller m_ARController;

    void Start()
    {
        
    }

    void Awake()
    {
        m_ARController = GetComponent<ARContorller>();
    }

    void Update()
    {
        // use to instruct the user study
        
    }

    
}
