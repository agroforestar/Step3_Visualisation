using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// This class is responsible for placing and moving instance of the prefab in the real world.
/// </summary>
[RequireComponent(typeof(ARRaycastManager))]
public class PlaceObject : MonoBehaviour
{
    private ARSessionOrigin aRSessionOrigin;
    // Prefab which will be spawned in the real world.
    [SerializeField]
    private List<GameObject> prefab;

    

    // Instance of the prefab.
    private List<GameObject> prefabInstance;
    private GameObject scene;
    private Vector3 center;

    private List<Vector3> crop;
    private List<Vector3> tree;
    private List<Vector3> line;

    
    /// <summary>
    /// Unity method called in before first frame.
    /// </summary>
    private void Start()
    {
        scene = new GameObject();

        aRSessionOrigin = GetComponent<ARSessionOrigin>();
        prefabInstance = new List<GameObject>();
        CreateScene();
       
    }
    /// <summary>
    /// Create the scene in the space and link all element to scene gameobject
    /// </summary>
    public void CreateScene()
    {
        foreach (string k in Global.inScene.Keys)
        {
            instanciateType(k, Global.inScene[k]);
        }
        center = FindCenterOfTransforms(prefabInstance);
        foreach (GameObject element in prefabInstance)
        {
            scene.transform.position = center;
            element.transform.parent = scene.transform;
        }
    }

    /// <summary>
    /// Instanciate all element present on the list
    /// </summary>
    /// <param name="name">name of the element</param>
    /// <param name="objects">informations (coordinates "coord", the object is composed by others ? "iscomposed")</param>
    private void instanciateType(string name, List<Dictionary<string, object>> objects)
    {
        for(int i = 0; i< objects.Count; i++)
        {        
            GameObject visu = Resources.Load("Prefabs/" + name + "/" + name + "_0") as GameObject;
            visu = Instantiate(visu);
            attachToParent(visu, name);

            if (objects[i].ContainsKey("isComposed") ){
                List<Vector3> points = (List<Vector3>)objects[i]["isComposed"];
                Vector3 center = new Vector3(points.Average(x => x[0]), points.Average(x => x[1]), points.Average(x => x[2]));
                visu.transform.position = center;
                Vector3 scale = new Vector3(points.Max(x => x[0])- points.Min(x => x[0]), points.Max(x => x[1]) - points.Min(x => x[1]), points.Max(x => x[2]) - points.Min(x => x[2]))/2;
                visu.transform.localScale = visu.transform.localScale + scale;
            }
            else
            {
                visu.transform.position = (Vector3)objects[i]["coord"];
            }
            visu.transform.localScale = visu.transform.localScale;
            visu.SetActive(false);
        }
    }

    private void attachToParent(GameObject child, string elementName)
    {
        GameObject parent = Resources.Load("Prefabs/Plant") as GameObject;
        parent = Instantiate(parent);
        child.transform.parent = parent.transform;
        parent.GetComponent<growth>().setDirectory(elementName);
        prefabInstance.Add(parent);
    }

    public Vector3 FindCenterOfTransforms(List<GameObject> transforms)
    {
        var bound = new Bounds(transforms[0].transform.position, Vector3.zero);
        for (int i = 1; i < transforms.Count; i++)
        {
            bound.Encapsulate(transforms[i].transform.position);
        }
        return bound.center;
    }

    /// <summary>
    /// Unity method called every frame.
    /// </summary>
    private void Update()
    {
    }

    /// <summary>
    /// Active the element and place the scene on the marker position
    /// </summary>
    /// <param name="originPoint">marker position</param>
    public void activateScene(Transform originPoint)
    {
        foreach (GameObject child in prefabInstance)
        {
            GameObject visu = child.transform.GetChild(0).gameObject;
            {
                if (!visu.activeInHierarchy)
                {
                    visu.SetActive(true);
                }
            }
            // In the instance is inactive, enable it.
            
           
        }
        scene.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
        scene.transform.position = Vector3.zero;
    }

}