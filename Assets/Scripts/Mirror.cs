using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    public Transform MirrorCam;
    
    Transform m_CameraA;  // Camera A position

    // Start is called before the first frame update
    void Start()
    {
        m_CameraA = Camera.main.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_CameraA)
            CalculateRotation();
        else
        {
            Debug.Log("Should get camera A postion!");
        }
    }

    public void CalculateRotation()
    {
        Vector3 view_dir = (m_CameraA.position - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(view_dir);

        rot.eulerAngles = transform.eulerAngles - rot.eulerAngles;

        MirrorCam.localRotation = rot;
    }
}
