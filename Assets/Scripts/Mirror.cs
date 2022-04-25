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
            CalculateMirrorCameraPos();
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

    public void CalculateMirrorCameraPos()
    {
        // flip the camera coordinate in z-axis in mirror coordinate
        Vector3 _player_cam_pos_in_mirror = transform.InverseTransformPoint(m_CameraA.position);
        _player_cam_pos_in_mirror.z = -_player_cam_pos_in_mirror.z;
        Vector3 _new_mirror_cam_pos = transform.TransformPoint(_player_cam_pos_in_mirror);

        // flip the camera up direction in z-axis in mirror coordinate
        Vector3 _player_cam_up_in_mirror = transform.InverseTransformDirection(m_CameraA.up);
        _player_cam_up_in_mirror.z = -_player_cam_up_in_mirror.z;
        Vector3 _new_mirror_cam_up = transform.TransformDirection(_player_cam_up_in_mirror);

        // flip the camera foward direction in z-axis in mirror coordinate
        Vector3 _player_cam_forward_in_mirror = transform.InverseTransformDirection(m_CameraA.forward);
        _player_cam_forward_in_mirror.z = -_player_cam_forward_in_mirror.z;
        Vector3 _new_mirror_cam_forward = transform.TransformDirection(_player_cam_forward_in_mirror);

        // set new mirror
        MirrorCam.position = _new_mirror_cam_pos;
        MirrorCam.rotation = Quaternion.LookRotation(_new_mirror_cam_forward, _new_mirror_cam_up);
        //// calculate the view direction on the mirror
        //Vector3 _view_direction = m_CameraA.forward;
        //Vector3 _view_origin = m_CameraA.position;
        //Vector3 _mirror_center = transform.position;
        //Vector3 _mirror_normal = transform.forward;
        //// ray cast to the mirror
        //float t = Vector3.Dot(_mirror_center - _view_origin, _mirror_normal) / Vector3.Dot(_view_direction, _mirror_normal);
        //if (t >= 0)
        //{
        //    // get intersection point
        //    Vector3 _intersection_point = _view_origin + _view_direction * t;

        //    // get the look at direction of the mirror camera
        //    Vector3 _new_mirror_forward = _intersection_point - _new_mirror_cam_pos;

        //    // set the new position of mirror cam (wrold space)
        //    MirrorCam.position = _new_mirror_cam_pos;
        //    // set the new rotation of mirror cam (world space)
        //    MirrorCam.rotation = Quaternion.LookRotation(_new_mirror_forward, MirrorCam.up);
        //}

    }
}
