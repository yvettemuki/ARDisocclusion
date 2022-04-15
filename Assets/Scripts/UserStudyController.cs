using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserStudyController : MonoBehaviour
{
    [SerializeField]
    private ARContorller m_ARController;

    // Tasks Vairables 
    TaskMode m_CurrentTaskMode = TaskMode.COUNTING_DYNAMIC_SPHERE;
    
    // Sphere
    int m_DynamicSphereNum = 3;
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

    // UI

    public enum CaptureDegree
    {
        HIGHLY_SUCCESS,
        COMMON_SUCCESS,
        FILED,
        HIGHLY_FAILD
    };

    public enum TaskMode
    {
        COUNTING_DYNAMIC_SPHERE,

    };

    void Start()
    {
        
    }

    void Update()
    {
        UpdateDynamicSpheres();
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

    public void PlaceDynamicSphere()
    {
        //Vector3 pos_in_world = Vector3.zero;
        //m_ARController.PortalObjectPos2World(in ControllerStates.SPHERE_POS_IN_PORTAL_1_MIN, out pos_in_world);

        // design a array of gameobjects with specific position
        InitDynamicSpheres();
    }

    public void InitDynamicSpheres()
    {
        for (int i = 0; i < m_DynamicSphereNum; i++)
        {
            // get portal coordinate system direction as set it as the sphere direction
            Quaternion _rotation = m_ARController.GetPortalTransform().rotation;
            Vector3 _position_start = Vector3.zero;
            m_ARController.PortalObjectPos2World(in ControllerStates.SPHERES_IN_PORTAL_POS_START[i], out _position_start);
            float longest_dist = Random.Range(1.5f, 2.5f);

            GameObject _sphere = Instantiate(m_SpherePrefab, _position_start, _rotation);

            float _range_scale = Random.Range(0.2f, 0.8f);
            _sphere.transform.localScale = new Vector3(_range_scale, _range_scale, _range_scale);

            DynamicSphere _dy_sphere = new DynamicSphere(_sphere, _position_start, longest_dist, Random.Range(2f, 8f));
            m_DynamicSpheres.Add(_dy_sphere);
        }
    }

    public void UpdateDynamicSpheres()
    {
        foreach (DynamicSphere obj in m_DynamicSpheres)
        {
            obj.sphere.transform.Translate(obj.speed * Time.deltaTime * obj.sphere.transform.forward, Space.World);

            // detect the bouding
            float dist = Vector3.Distance(obj.sphere.transform.position, obj.startPos);
            if (dist > obj.longest_dist)
            {
                obj.speed = -obj.speed;
            }
        }
    }

    public void SetUserStudyObjectsActive(bool isActive)
    {
        if (m_CurrentTaskMode == TaskMode.COUNTING_DYNAMIC_SPHERE)
        {
            foreach (DynamicSphere obj in m_DynamicSpheres)
            {
                Vector3 _world_position = obj.sphere.transform.position;
                Vector3 _portal_position = Vector3.zero;
                m_ARController.WorldObjectPos2Portal(in _world_position, out _portal_position);

                if (_portal_position.z > 0f)
                    obj.sphere.SetActive(isActive);
                else
                    obj.sphere.SetActive(true);
            }
        }
    }
}
