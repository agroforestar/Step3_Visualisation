using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// This class is responsible for placing and moving instance of the prefab in the real world.
/// </summary>
[RequireComponent(typeof(ARRaycastManager))]
public class ARPlaceObject : MonoBehaviour
{
    // Reference to the AR Raycast Manager
    private ARRaycastManager raycastManager;

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
        crop = new List<Vector3> { new Vector3(8, 0, 4), new Vector3(3, 0, 4) };
        tree = new List<Vector3> { new Vector3(11, 0, 8), new Vector3(11, 0, 6), new Vector3(11, 0, 4), new Vector3(11, 0, 1),
                                    new Vector3(1, 0, 8), new Vector3(1, 0, 6), new Vector3(1, 0, 4), new Vector3(1, 0, 1),
                                    new Vector3(6, 0, 8), new Vector3(6, 0, 6), new Vector3(6, 0, 4), new Vector3(6, 0, 1)};
        line = new List<Vector3> { new Vector3(11, 0, 5), new Vector3(1, 0, 4), new Vector3(6, 0, 4) };
        scene = new GameObject();

        raycastManager = GetComponent<ARRaycastManager>();
        prefabInstance = new List<GameObject>();
        instanciateType(crop, prefab[0]);
        instanciateType(tree, prefab[1]);
        instanciateType(line, prefab[2]);
        center = FindCenterOfTransforms(prefabInstance);
        foreach (GameObject element in prefabInstance)
        {
            element.transform.parent = scene.transform;
            element.transform.position = element.transform.position - center;
        }
        scene.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);    
    }

    private void instanciateType(List<Vector3> objects, GameObject prefab)
    {
        for(int i = 0; i< objects.Count; i++)
        {
            GameObject temp = Instantiate(prefab);
            temp.transform.position = objects[i];
            temp.SetActive(false);
            prefabInstance.Add(temp);
        }
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
        // List of the hit points in real world.
        var hitList = new List<ARRaycastHit>();
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began) { return; }

        // Raycast from the center of the screen which should hit only detected surfaces.
        if (raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hitList, TrackableType.PlaneWithinBounds | TrackableType.PlaneWithinPolygon))
        {
            foreach (GameObject child in prefabInstance)
            {
                // In the instance is inactive, enable it.
                if (!child.activeInHierarchy)
                {
                    child.SetActive(true);
                }
            }

            hitList = hitList.OrderBy(h => h.distance).ToList();
            var hitPoint = hitList[0];
            Debug.Log(hitPoint.pose.position);
 
            scene.transform.position = hitPoint.pose.position;
            scene.transform.up = hitPoint.pose.up;
            
           
        }
        /*else
        {
            for (int i = 0; i < prefabInstance.Count; i++)
            {
                // In the instance is active, disable it.
                if (prefabInstance[i].activeInHierarchy)
                {
                    prefabInstance[i].SetActive(false);
                }
            }
        }*/
    }
}