using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserStudyController : MonoBehaviour
{
    [SerializeField]
    private ARController m_ARController;

    // Other
    int currentMatType = -1;

    // UI
    public Dropdown m_DropdownTaskMode;
    
    // Dynamic Spheres
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

    // Human Direction Indication
    GameObject m_DirectDigitIndicator;
    public GameObject m_DirectDigitIndicatorPrefab;

    // Closest Sphere
    List<GameObject> m_ClosestSpheres = new List<GameObject>();
    public GameObject m_ClosestSpherePrefab;
    public List<Material> m_ClosestSphereMat;

    // Similar Object
    GameObject m_SimilarGroup;
    public GameObject m_SimilarObjectPrefab;

    // Other
    public Material m_CircleMaterial;
    public Material m_SphereStandardMat; // 0
    public Material m_SphereStencilMat;  // 1
    public List<Material> m_IndicatorStandardMats; // 0
    public List<Material> m_IndicatorStencilMats;  // 1
    public List<Material> m_SimilarGroup1StandardMats; // 0
    public List<Material> m_SimilarGroup1StencilMats;  // 1

    public enum CaptureDegree
    {
        HIGHLY_SUCCESS,
        COMMON_SUCCESS,
        FILED,
        HIGHLY_FAILD
    };

    public enum TaskMode
    {
        COUNTING_DYNAMIC_SPHERE_EASY,
        COUNTING_DYNAMIC_SPHERE_MEDIUM,
        COUNTING_DYNAMIC_SPHERE_HARD,
        DIRECT_INDICATOR_EASY, 
        DIRECT_INDICATOR_MEDIUM,
        DIRECT_INDICATOR_HARD,
        ClOSEST_SPHERE_GROUP_EASY,
        ClOSEST_SPHERE_GROUP_MEDIUM,
        ClOSEST_SPHERE_GROUP_HARD,
        SIMILAR_GROUP_EASY,
        SIMILAR_GROUP_MEDIUM,
        SIMILAR_GROUP_HARD,
        NONE
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
        if (currentTaskMode == TaskMode.COUNTING_DYNAMIC_SPHERE_EASY
            || currentTaskMode == TaskMode.COUNTING_DYNAMIC_SPHERE_MEDIUM
            || currentTaskMode == TaskMode.COUNTING_DYNAMIC_SPHERE_HARD)
        {
            UpdateDynamicSpheres();
        }

        if (currentTaskMode == TaskMode.DIRECT_INDICATOR_EASY
            || currentTaskMode == TaskMode.DIRECT_INDICATOR_MEDIUM
            || currentTaskMode == TaskMode.DIRECT_INDICATOR_HARD)
        {
            UpdateDirectIndicator();
        }

        if (currentTaskMode == TaskMode.ClOSEST_SPHERE_GROUP_EASY
            || currentTaskMode == TaskMode.ClOSEST_SPHERE_GROUP_MEDIUM
            || currentTaskMode == TaskMode.ClOSEST_SPHERE_GROUP_HARD)
        {
            UpdateClosestSphere();
        }

        if (currentTaskMode == TaskMode.SIMILAR_GROUP_EASY
           || currentTaskMode == TaskMode.SIMILAR_GROUP_MEDIUM
           || currentTaskMode == TaskMode.SIMILAR_GROUP_HARD)
        {
            UpdateSimilarGroup();
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

    public void InitDynamicSpheres(int currTaskIndex)
    {
        int sphereNum = ControllerStates.DYN_SPHERES_NUM[currTaskIndex];

        for (int i = 0; i < sphereNum; i++)
        {
            // get portal coordinate system direction as set it as the sphere direction
            Quaternion _rotation = m_ARController.GetPortalTransform().rotation;
            Vector3 _position_start = Vector3.zero;
            m_ARController.PortalObjectPos2World(in ControllerStates.DYM_SPHERES_POS_IN_PORTAL[i], out _position_start);
            float longest_dist = Random.Range(1.5f, 3.5f);

            GameObject _sphere = Instantiate(m_SpherePrefab, _position_start, _rotation);

            float _range_scale = Random.Range(0.1f, 0.25f);
            _sphere.transform.localScale = new Vector3(_range_scale, _range_scale, _range_scale);

            DynamicSphere _dy_sphere = new DynamicSphere(_sphere, _position_start, longest_dist, Random.Range(1f, 2.5f));
            m_DynamicSpheres.Add(_dy_sphere);
        }
    }

    public void InitDigitsNumbersForHumanDir()
    {
        // get position and rotation based on the portal coordinate system
        Quaternion _rotation = m_ARController.GetPortalTransform().rotation;
        Vector3 _position = Vector3.zero;
        m_ARController.PortalObjectPos2World(in ControllerStates.HUMAN_DIRECT_DIGIT_NUMBER, out _position);

        m_DirectDigitIndicator = Instantiate(m_DirectDigitIndicatorPrefab, _position, _rotation);
    }

    public void InitClosestSphereGroup(TaskMode taskMode)
    {
        if (taskMode == TaskMode.ClOSEST_SPHERE_GROUP_EASY)
        {
            CreateClosestSphere(ControllerStates.CLOSEST_SPHERE_GROUP_1[0], m_ClosestSphereMat[0]);
            CreateClosestSphere(ControllerStates.CLOSEST_SPHERE_GROUP_1[1], m_ClosestSphereMat[1]);
            CreateClosestSphere(ControllerStates.CLOSEST_SPHERE_GROUP_1[2], m_ClosestSphereMat[2]);
        }
        else if (taskMode == TaskMode.ClOSEST_SPHERE_GROUP_MEDIUM)
        {
            CreateClosestSphere(ControllerStates.CLOSEST_SPHERE_GROUP_2[0], m_ClosestSphereMat[0]);
            CreateClosestSphere(ControllerStates.CLOSEST_SPHERE_GROUP_2[1], m_ClosestSphereMat[1]);
            CreateClosestSphere(ControllerStates.CLOSEST_SPHERE_GROUP_2[2], m_ClosestSphereMat[2]);
        }
        else if (taskMode == TaskMode.ClOSEST_SPHERE_GROUP_HARD)
        {
            CreateClosestSphere(ControllerStates.CLOSEST_SPHERE_GROUP_3[0], m_ClosestSphereMat[0]);
            CreateClosestSphere(ControllerStates.CLOSEST_SPHERE_GROUP_3[1], m_ClosestSphereMat[1]);
            CreateClosestSphere(ControllerStates.CLOSEST_SPHERE_GROUP_3[2], m_ClosestSphereMat[2]);
        }
        else
        {
            Debug.Log("Should not use the function about the closest sphere task!");
        }
    }

    public void InitSimilarGroup(TaskMode taskMode)
    {
        if (taskMode == TaskMode.SIMILAR_GROUP_EASY)
        {
            CreateSimilarGroup(ControllerStates.FIND_SIMILAR_GROUPs[0]);
        }
        else if (taskMode == TaskMode.SIMILAR_GROUP_MEDIUM)
        {
            CreateSimilarGroup(ControllerStates.FIND_SIMILAR_GROUPs[1]);
        }
        else if (taskMode == TaskMode.SIMILAR_GROUP_HARD)
        {
            CreateSimilarGroup(ControllerStates.FIND_SIMILAR_GROUPs[2]);
        }
        else
        {
            Debug.Log("Should not use the function about the similar object!");
        }
    }

    public void CreateSimilarGroup(Vector3 pos_in_portal)
    {
        Quaternion _rotation = m_ARController.GetPortalTransform().rotation;

        Vector3 _position = Vector3.zero;
        m_ARController.PortalObjectPos2World(in pos_in_portal, out _position);

        m_SimilarGroup = Instantiate(m_SimilarObjectPrefab, _position, _rotation);
    }

    public void CreateClosestSphere(Vector3 pos_in_portal, Material mat)
    {
        Quaternion _rotation = m_ARController.GetPortalTransform().rotation;

        Vector3 _position = Vector3.zero;
        m_ARController.PortalObjectPos2World(in pos_in_portal, out _position);

        GameObject sphere = Instantiate(m_ClosestSpherePrefab, _position, _rotation);
        sphere.GetComponent<MeshRenderer>().material = mat;

        m_ClosestSpheres.Add(sphere);
    }

    public void DestroyCurrentObjects()
    {
        // Dynamic Spheres
        if (currentTaskMode == TaskMode.COUNTING_DYNAMIC_SPHERE_EASY 
            || currentTaskMode == TaskMode.COUNTING_DYNAMIC_SPHERE_MEDIUM 
            || currentTaskMode == TaskMode.COUNTING_DYNAMIC_SPHERE_HARD)
        {
            foreach (DynamicSphere obj in m_DynamicSpheres)
            {
                Destroy(obj.sphere);
            }

            m_DynamicSpheres.Clear();
        }

        // human direction indicator
        if (m_DirectDigitIndicator) Destroy(m_DirectDigitIndicator);
        if (m_ARController.GetHumanSprite()) Destroy(m_ARController.GetHumanSprite());

        // closest sphere
        if (m_ClosestSpheres.Count > 0)
        {
            foreach (GameObject obj in m_ClosestSpheres)
            {
                Destroy(obj);
            }

            m_ClosestSpheres.Clear();
        }

        // similar object group
        if (m_SimilarGroup) Destroy(m_SimilarGroup);
    }

    public void UpdateDynamicSpheres()
    {
        if (ARController.currentUserStudyType == ARController.UserStudyType.TYPE_PICINPIC
            || ARController.currentUserStudyType == ARController.UserStudyType.TYPE_REFLECTION)
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

    public void UpdateDirectIndicator()
    {
        if (ARController.currentUserStudyType == ARController.UserStudyType.TYPE_PICINPIC
            || ARController.currentUserStudyType == ARController.UserStudyType.TYPE_REFLECTION)
        {
            ChangeDirectIndicatorMaterial(MAT_STENCIL);
        }
        else
        {
            ChangeDirectIndicatorMaterial(MAT_STANDARD);
        }
    }

    public void UpdateClosestSphere()
    {
        if (ARController.currentUserStudyType == ARController.UserStudyType.TYPE_PICINPIC
            || ARController.currentUserStudyType == ARController.UserStudyType.TYPE_REFLECTION)
        {
            ChangeClosestSphereMaterial(MAT_STENCIL);
        }
        else
        {
            ChangeClosestSphereMaterial(MAT_STANDARD);
        }
    }

    public void UpdateSimilarGroup()
    {
        if (ARController.currentUserStudyType == ARController.UserStudyType.TYPE_PICINPIC
            || ARController.currentUserStudyType == ARController.UserStudyType.TYPE_REFLECTION)
        {
            ChangeSimilarMaterial(MAT_STENCIL);
        }
        else
        {
            ChangeSimilarMaterial(MAT_STANDARD);
        }
    }

    public void ChangeDirectIndicatorMaterial(int type)
    {
        if (type == MAT_STANDARD)
        {
            // type == 0, standard color material
            m_DirectDigitIndicator.transform.GetChild(0).GetComponent<MeshRenderer>().material = m_IndicatorStandardMats[0];
            m_DirectDigitIndicator.transform.GetChild(1).GetComponent<MeshRenderer>().material = m_IndicatorStandardMats[1];
            m_DirectDigitIndicator.transform.GetChild(2).GetComponent<MeshRenderer>().material = m_IndicatorStandardMats[2];
            m_DirectDigitIndicator.transform.GetChild(3).GetComponent<MeshRenderer>().material = m_IndicatorStandardMats[3];
            m_DirectDigitIndicator.transform.GetChild(4).GetComponent<MeshRenderer>().material = m_IndicatorStandardMats[4];
            m_DirectDigitIndicator.transform.GetChild(5).GetComponent<MeshRenderer>().material = m_IndicatorStandardMats[5];
            
            currentMatType = MAT_STANDARD;
        }
        else if (type == MAT_STENCIL)
        {
            // type == 1, stencil shader material
            m_DirectDigitIndicator.transform.GetChild(0).GetComponent<MeshRenderer>().material = m_IndicatorStencilMats[0];
            m_DirectDigitIndicator.transform.GetChild(1).GetComponent<MeshRenderer>().material = m_IndicatorStencilMats[1];
            m_DirectDigitIndicator.transform.GetChild(2).GetComponent<MeshRenderer>().material = m_IndicatorStencilMats[2];
            m_DirectDigitIndicator.transform.GetChild(3).GetComponent<MeshRenderer>().material = m_IndicatorStencilMats[3];
            m_DirectDigitIndicator.transform.GetChild(4).GetComponent<MeshRenderer>().material = m_IndicatorStencilMats[4];
            m_DirectDigitIndicator.transform.GetChild(5).GetComponent<MeshRenderer>().material = m_IndicatorStencilMats[5];
            currentMatType = MAT_STENCIL;
        }
    }

    public void ChangeDynamicSphereMaterial(int type)
    {
        //if (m_DynamicSpheres.Count > 0 && currentMatType == type)
        //    return;

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

    public void ChangeClosestSphereMaterial(int type)
    {
        if (type == MAT_STANDARD)
        {
            // type == 0, standard color material
            m_ClosestSpheres[2].GetComponent<MeshRenderer>().material = m_SphereStandardMat;
            currentMatType = MAT_STANDARD;
        }
        else if (type == MAT_STENCIL)
        {
            // type == 1, stencil shader material
            m_ClosestSpheres[2].GetComponent<MeshRenderer>().material = m_SphereStencilMat;
            currentMatType = MAT_STENCIL;
        }
    }

    public void ChangeSimilarMaterial(int type)
    {
        if (type == MAT_STANDARD)
        {
            // type == 0, standard color material
            //m_SimilarGroup.transform.GetChild(0).GetComponent<MeshRenderer>().material = m_SimilarGroup1StandardMats[0];
            m_SimilarGroup.transform.GetChild(1).GetComponent<MeshRenderer>().material = m_SimilarGroup1StandardMats[1];
            m_SimilarGroup.transform.GetChild(2).GetComponent<MeshRenderer>().material = m_SimilarGroup1StandardMats[2];
            m_SimilarGroup.transform.GetChild(3).GetComponent<MeshRenderer>().material = m_SimilarGroup1StandardMats[3];
            m_SimilarGroup.transform.GetChild(4).GetComponent<MeshRenderer>().material = m_SimilarGroup1StandardMats[4];
            m_SimilarGroup.transform.GetChild(5).GetComponent<MeshRenderer>().material = m_SimilarGroup1StandardMats[5];
            currentMatType = MAT_STANDARD;
        }
        else if (type == MAT_STENCIL)
        {
            // type == 1, stencil shader material
            //m_SimilarGroup.transform.GetChild(0).GetComponent<MeshRenderer>().material = m_SimilarGroup1StencilMats[0];
            m_SimilarGroup.transform.GetChild(1).GetComponent<MeshRenderer>().material = m_SimilarGroup1StencilMats[1];
            m_SimilarGroup.transform.GetChild(2).GetComponent<MeshRenderer>().material = m_SimilarGroup1StencilMats[2];
            m_SimilarGroup.transform.GetChild(3).GetComponent<MeshRenderer>().material = m_SimilarGroup1StencilMats[3];
            m_SimilarGroup.transform.GetChild(4).GetComponent<MeshRenderer>().material = m_SimilarGroup1StencilMats[4];
            m_SimilarGroup.transform.GetChild(5).GetComponent<MeshRenderer>().material = m_SimilarGroup1StencilMats[5];
            currentMatType = MAT_STENCIL;
        }
    }

    public void SetUserStudyObjectsActive(bool isActive)
    {
        if (currentTaskMode == TaskMode.COUNTING_DYNAMIC_SPHERE_EASY 
            || currentTaskMode == TaskMode.COUNTING_DYNAMIC_SPHERE_MEDIUM 
            || currentTaskMode == TaskMode.COUNTING_DYNAMIC_SPHERE_HARD)
        {
            // dynamic spheres
            foreach (DynamicSphere obj in m_DynamicSpheres)
            {
                if (ARController.currentUserStudyType == ARController.UserStudyType.TYPE_MULTIPERSPECTIVE)
                {
                    // calculate the radius of the spheres
                    Vector3 scale = obj.sphere.transform.lossyScale;
                    float absoluteRadius = Mathf.Abs(Mathf.Max(Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y)), Mathf.Abs(scale.z)) * 0.5f);
                    absoluteRadius = Mathf.Max(absoluteRadius, 0.00001f);

                    // get the position of sphere in portal coordinate
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
                else
                {
                    obj.sphere.SetActive(isActive);
                }
            }
        }

        if (currentTaskMode == TaskMode.DIRECT_INDICATOR_EASY
            || currentTaskMode == TaskMode.DIRECT_INDICATOR_MEDIUM
            || currentTaskMode == TaskMode.DIRECT_INDICATOR_HARD)
        {
            // human sprite
            if (m_ARController.GetHumanSprite())
                m_ARController.GetHumanSprite().SetActive(isActive);

            // indicator
            if (m_DirectDigitIndicator)
                m_DirectDigitIndicator.SetActive(isActive);
        }

        if (currentTaskMode == TaskMode.ClOSEST_SPHERE_GROUP_EASY
            || currentTaskMode == TaskMode.ClOSEST_SPHERE_GROUP_MEDIUM
            || currentTaskMode == TaskMode.ClOSEST_SPHERE_GROUP_HARD)
        {
            // closest sphere
            if (ARController.currentUserStudyType == ARController.UserStudyType.TYPE_MULTIPERSPECTIVE)
                m_ClosestSpheres[2].SetActive(isActive);
            else
            {
                // if the spheres have many, can replace to foreach
                m_ClosestSpheres[0].SetActive(isActive);
                m_ClosestSpheres[1].SetActive(isActive);
                m_ClosestSpheres[2].SetActive(isActive);
            }
        }

        if (currentTaskMode == TaskMode.SIMILAR_GROUP_EASY
            || currentTaskMode == TaskMode.SIMILAR_GROUP_MEDIUM
            || currentTaskMode == TaskMode.SIMILAR_GROUP_HARD)
        {
            // similar group
            if (ARController.currentUserStudyType == ARController.UserStudyType.TYPE_MULTIPERSPECTIVE)
            {
                m_SimilarGroup.transform.GetChild(1).gameObject.SetActive(isActive);
                m_SimilarGroup.transform.GetChild(2).gameObject.SetActive(isActive);
                m_SimilarGroup.transform.GetChild(3).gameObject.SetActive(isActive);
                m_SimilarGroup.transform.GetChild(4).gameObject.SetActive(isActive);
                m_SimilarGroup.transform.GetChild(5).gameObject.SetActive(isActive);
            }
            else
            {
                // if the spheres have many, can replace to foreach
                m_SimilarGroup.transform.GetChild(0).gameObject.SetActive(isActive);
                m_SimilarGroup.transform.GetChild(1).gameObject.SetActive(isActive);
                m_SimilarGroup.transform.GetChild(2).gameObject.SetActive(isActive);
                m_SimilarGroup.transform.GetChild(3).gameObject.SetActive(isActive);
                m_SimilarGroup.transform.GetChild(4).gameObject.SetActive(isActive);
                m_SimilarGroup.transform.GetChild(5).gameObject.SetActive(isActive);
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
                currentTaskMode = TaskMode.COUNTING_DYNAMIC_SPHERE_EASY;
                break;

            case "Dynm Sphere 5":
                m_DynamicSphereNum = 5;
                InitDynamicSpheres();
                currentTaskMode = TaskMode.COUNTING_DYNAMIC_SPHERE_MEDIUM;
                break;

            case "Dynm Sphere 7":
                m_DynamicSphereNum = 7;
                InitDynamicSpheres();
                currentTaskMode = TaskMode.COUNTING_DYNAMIC_SPHERE_HARD;
                break;

            case "Human Dir 23":
                InitDigitsNumbersForHumanDir();
                m_ARController.InitHumanSpriteForUserStudy(TaskMode.DIRECT_INDICATOR_EASY);
                currentTaskMode = TaskMode.DIRECT_INDICATOR_EASY;
                break;

            case "Human Dir 56":
                InitDigitsNumbersForHumanDir();
                m_ARController.InitHumanSpriteForUserStudy(TaskMode.DIRECT_INDICATOR_MEDIUM);
                currentTaskMode = TaskMode.DIRECT_INDICATOR_MEDIUM;
                break;

            case "Human Dir 12":
                InitDigitsNumbersForHumanDir();
                m_ARController.InitHumanSpriteForUserStudy(TaskMode.DIRECT_INDICATOR_HARD);
                currentTaskMode = TaskMode.DIRECT_INDICATOR_MEDIUM;
                break;

            case "Closest 1":
                InitClosestSphereGroup(TaskMode.ClOSEST_SPHERE_GROUP_EASY);
                currentTaskMode = TaskMode.ClOSEST_SPHERE_GROUP_EASY;
                break;

            case "Closest 2":
                InitClosestSphereGroup(TaskMode.ClOSEST_SPHERE_GROUP_MEDIUM);
                currentTaskMode = TaskMode.ClOSEST_SPHERE_GROUP_MEDIUM;
                break;

            case "Closest 3":
                InitClosestSphereGroup(TaskMode.ClOSEST_SPHERE_GROUP_HARD);
                currentTaskMode = TaskMode.ClOSEST_SPHERE_GROUP_HARD;
                break;

            case "Similar 1":
                InitSimilarGroup(TaskMode.SIMILAR_GROUP_EASY);
                currentTaskMode = TaskMode.SIMILAR_GROUP_EASY;
                break;

            case "Similar 2":
                InitSimilarGroup(TaskMode.SIMILAR_GROUP_MEDIUM);
                currentTaskMode = TaskMode.SIMILAR_GROUP_MEDIUM;
                break;

            case "Similar 3":
                InitSimilarGroup(TaskMode.SIMILAR_GROUP_HARD);
                currentTaskMode = TaskMode.SIMILAR_GROUP_HARD;
                break;

            default:
                break;
        }
    }
}
