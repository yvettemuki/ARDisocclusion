using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserStudyController : MonoBehaviour
{
    [SerializeField]
    private ARContorller m_ARController;

    // Other
    int currentMatType = -1;

    // UI
    public Dropdown m_DropdownTaskMode;
    
    // Sphere
    public int m_DynamicSphereNum = 3;
    List<DynamicSphere> m_DynamicSpheres = new List<DynamicSphere>();
    public class DynamicSphere
    {
        public DynamicSphere(GameObject sphere_0, Vector3 startPos_0, float longest_dist_0, float speed_0)
        {
            sphere = sphere_0;
            startPos = startPos_0;
            longest_dist = longest_dist_0;
            speed = speed_0;
        }

        public GameObject sphere { get; set; }
        public Vector3 startPos { get; set; }
        public float longest_dist { get; set; }
        public float speed { get; set; }

    }
    public GameObject m_SpherePrefab;

    // Other
    public Material m_CircleMaterial;
    public Material m_SphereStandardMat;
    public Material m_SphereStencilMat;

    public enum CaptureDegree
    {
        HIGHLY_SUCCESS,
        COMMON_SUCCESS,
        FILED,
        HIGHLY_FAILD
    };

    public enum TaskMode
    {
        NONE,
        COUNTING_DYNAMIC_SPHERE_3,
        COUNTING_DYNAMIC_SPHERE_5,
        COUNTING_DYNAMIC_SPHERE_7
    };

    // Tasks Vairables 
    public static TaskMode currentTaskMode = TaskMode.NONE;

    // Constants
    // Object Material Mode
    public static int MAT_STANDARD = 0;
    public static int MAT_STENCIL = 1;

    void Start()
    {
        
    }

    void Update()
    {
        if (currentTaskMode == TaskMode.COUNTING_DYNAMIC_SPHERE_3
            || currentTaskMode == TaskMode.COUNTING_DYNAMIC_SPHERE_5
            || currentTaskMode == TaskMode.COUNTING_DYNAMIC_SPHERE_7)
        {
            UpdateDynamicSpheres();
        }

    }

    public void Reset()
    {
        DestroyCurrentObjects();
    }

    public void CreateCircle(Vector3 position)
    {
        var circle = new GameObject { name = "Circle" };
        circle.AddComponent<MeshRenderer>();
        circle.GetComponent<MeshRenderer>().material = m_CircleMaterial;
        circle.DrawCircle(0.8f, 0.05f);
        circle.transform.position = position;
        circle.transform.rotation = Quaternion.identity;
    }

    public void DropCircle(in int drop_time)  // the drop time should send by the arcontroller
    {
        GameObject human_sprite = m_ARController.GetHumanSprite();
        Vector3 human_position = human_sprite.transform.position;
        Vector3 world_pos = Vector3.zero;
        
        switch (drop_time)
        {
            // set position one
            case 1:
                {
                    Vector3 portal_pos = ControllerStates.CIRCLE_POS_IN_PORTAL;
                    m_ARController.PortalObjectPos2World(in portal_pos, out world_pos);
                    CreateCircle(world_pos);
                    // consider where to drop circle
                    break;
                }
            case 2:
                {
                    break;
                }
            case 3:
                {
                    break;
                }
            default: 
                break;
        }
        

        // set position two

        // set position three
    }

    public void ResetCircle()
    {

    }

    public void InitDynamicSpheres()
    {
        for (int i = 0; i < m_DynamicSphereNum; i++)
        {
            // get portal coordinate system direction as set it as the sphere direction
            Quaternion _rotation = m_ARController.GetPortalTransform().rotation;
            Vector3 _position_start = Vector3.zero;
            m_ARController.PortalObjectPos2World(in ControllerStates.SPHERES_IN_PORTAL_POS_START[i], out _position_start);
            float longest_dist = Random.Range(1.5f, 3.5f);

            GameObject _sphere = Instantiate(m_SpherePrefab, _position_start, _rotation);

            float _range_scale = Random.Range(0.1f, 0.4f);
            _sphere.transform.localScale = new Vector3(_range_scale, _range_scale, _range_scale);

            DynamicSphere _dy_sphere = new DynamicSphere(_sphere, _position_start, longest_dist, Random.Range(1f, 2.5f));
            m_DynamicSpheres.Add(_dy_sphere);
        }
    }

    public void DestroyCurrentObjects()
    {
        // Dynamic Spheres
        if (currentTaskMode == TaskMode.COUNTING_DYNAMIC_SPHERE_3 
            || currentTaskMode == TaskMode.COUNTING_DYNAMIC_SPHERE_5 
            || currentTaskMode == TaskMode.COUNTING_DYNAMIC_SPHERE_7)
        {
            foreach (DynamicSphere obj in m_DynamicSpheres)
            {
                Destroy(obj.sphere);
            }

            m_DynamicSpheres.Clear();
        }
        
    }

    public void UpdateDynamicSpheres()
    {
        if (ARContorller.currentUserStudyType == ARContorller.UserStudyType.TYPE_PICINPIC
            || ARContorller.currentUserStudyType == ARContorller.UserStudyType.TYPE_REFLECTION)
        {
            ChangeDynamicSphereMaterial(MAT_STENCIL);
        }
        else
        {
            ChangeDynamicSphereMaterial(MAT_STANDARD);
        }

        foreach (DynamicSphere obj in m_DynamicSpheres)
        {
            obj.sphere.transform.Translate(obj.speed * Time.deltaTime * obj.sphere.transform.forward, Space.World);

            // detect the bouding
            float dist = Vector3.Distance(obj.sphere.transform.position, obj.startPos);
            if (dist > obj.longest_dist)
            {
                obj.sphere.transform.position = obj.startPos + obj.sphere.transform.forward * obj.longest_dist * (obj.speed / Mathf.Abs(obj.speed));
                obj.speed = -obj.speed;
            }
        }
    }

    public void ChangeDynamicSphereMaterial(int type)
    {
        if (m_DynamicSpheres.Count > 0 && currentMatType == type)
            return;

        if (type == MAT_STANDARD)
        {
            // type == 0, standard color material
            foreach (DynamicSphere obj in m_DynamicSpheres)
            {
                obj.sphere.GetComponent<MeshRenderer>().material = m_SphereStandardMat;
            }

            currentMatType = MAT_STANDARD;
        }
        else if (type == MAT_STENCIL)
        {
            // type == 1, stencil shader material
            foreach (DynamicSphere obj in m_DynamicSpheres)
            {
                obj.sphere.GetComponent<MeshRenderer>().material = m_SphereStencilMat;
            }

            currentMatType = MAT_STENCIL;
        }
    }

    public void SetUserStudyObjectsActive(bool isActive)
    {
        if (ARContorller.currentUserStudyType == ARContorller.UserStudyType.TYPE_PICINPIC || 
            ARContorller.currentUserStudyType == ARContorller.UserStudyType.TYPE_REFLECTION)
            return;

        if (currentTaskMode == TaskMode.COUNTING_DYNAMIC_SPHERE_3 
            || currentTaskMode == TaskMode.COUNTING_DYNAMIC_SPHERE_5 
            || currentTaskMode == TaskMode.COUNTING_DYNAMIC_SPHERE_7)
        {
            foreach (DynamicSphere obj in m_DynamicSpheres)
            {
                // calculate the radius of the spheres
                Vector3 scale = obj.sphere.transform.lossyScale;
                float absoluteRadius = Mathf.Abs(Mathf.Max(Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y)), Mathf.Abs(scale.z)) * 0.5f);
                absoluteRadius = Mathf.Max(absoluteRadius, 0.00001f);

                Vector3 _world_position = obj.sphere.transform.position;
                Vector3 _portal_position = Vector3.zero;
                m_ARController.WorldObjectPos2Portal(in _world_position, out _portal_position);

                float dist_from_portal = Mathf.Abs(_portal_position.z);
                if (dist_from_portal > absoluteRadius)
                {
                    if (_portal_position.z < 0f)
                        obj.sphere.SetActive(true);
                    else
                        obj.sphere.SetActive(isActive);
                }
                else
                {
                    obj.sphere.SetActive(true);
                }
            }
        }
    }

    public void OnUserStudyTaskModeChange()
    {
        var selected = m_DropdownTaskMode.options[m_DropdownTaskMode.value].text;

        DestroyCurrentObjects();

        switch (selected)
        {
            case "None":
                currentTaskMode = TaskMode.NONE;
                break;

            case "Dynm Sphere 3":
                m_DynamicSphereNum = 3;
                InitDynamicSpheres();
                currentTaskMode = TaskMode.COUNTING_DYNAMIC_SPHERE_3;
                break;

            case "Dynm Sphere 5":
                m_DynamicSphereNum = 5;
                InitDynamicSpheres();
                currentTaskMode = TaskMode.COUNTING_DYNAMIC_SPHERE_5;
                break;

            case "Dynm Sphere 7":
                m_DynamicSphereNum = 7;
                InitDynamicSpheres();
                currentTaskMode = TaskMode.COUNTING_DYNAMIC_SPHERE_7;
                break;

            default:
                break;
        }
    }
}
