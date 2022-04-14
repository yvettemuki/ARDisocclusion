using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserStudyController : MonoBehaviour
{
    [SerializeField]
    private ARContorller m_ARController;

    // Prefab
    public GameObject m_SpherePrefab;

    // UI

    // Other
    public Material m_CircleMaterial;

    public enum CaptureDegree
    {
        HIGHLY_SUCCESS,
        COMMON_SUCCESS,
        FILED,
        HIGHLY_FAILD
    };

    void Start()
    {
        
    }

    void Update()
    {
        
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

    public void PlaceSphere()
    {
        Vector3 pos_in_world = Vector3.zero;
        m_ARController.PortalObjectPos2World(in ControllerStates.SPHERE_POS_IN_PORTAL_1, out pos_in_world);

        // design a array of gameobjects with specific position

        Instantiate(m_SpherePrefab, pos_in_world, Quaternion.identity);
    }

}
