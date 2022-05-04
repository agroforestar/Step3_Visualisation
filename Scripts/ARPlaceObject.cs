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
    private ARPlaneManager m_ARPlaneManager;
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

        raycastManager = GetComponent<ARRaycastManager>();
        m_ARPlaneManager = GetComponent<ARPlaneManager>();
        aRSessionOrigin = GetComponent<ARSessionOrigin>();
        prefabInstance = new List<GameObject>();
        foreach (string k in Global.inScene.Keys)
        {
            Debug.Log(k);
            instanciateType(k, Global.inScene[k]);
        }
        center = FindCenterOfTransforms(prefabInstance);
        foreach (GameObject element in prefabInstance)
        {
            element.transform.parent = scene.transform;
            element.transform.position = element.transform.position - center;
        }
        scene.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);    
    }

    private void instanciateType(string name, List<Dictionary<string, object>> objects)
    {
        for(int i = 0; i< objects.Count; i++)
        {
            GameObject prefab = Resources.Load("Prefabs/Plant") as GameObject;
            prefab = Instantiate(prefab);
            GameObject visu = Resources.Load("Prefabs/" + name+"/"+name+"_0") as GameObject;
            GameObject temp = Instantiate(visu);
            temp.transform.parent = prefab.transform;
            if(objects[i].ContainsKey("isComposed") ){
                List<Vector3> points = (List<Vector3>)objects[i]["isComposed"];
                Vector3 center = new Vector3(points.Average(x => x[0]), points.Average(x => x[1]), points.Average(x => x[2]));
                Debug.Log((Vector3)objects[i]["coord"]);
                Debug.Log(center);
                temp.transform.position = center;
                Vector3 scale = new Vector3(points.Max(x => x[0])- points.Min(x => x[0]), points.Max(x => x[1]) - points.Min(x => x[1]), points.Max(x => x[2]) - points.Min(x => x[2]))/2;
                temp.transform.localScale = temp.transform.localScale + scale;
            }
            else
            {
                temp.transform.position = (Vector3)objects[i]["coord"];
            }
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
                aRSessionOrigin.MakeContentAppearAt(this.transform, hitList[0].pose.position);
            }
            stopPlaneDectection();


            hitList = hitList.OrderBy(h => h.distance).ToList();
            var hitPoint = hitList[0];
 
            scene.transform.position = hitPoint.pose.position;
            scene.transform.up = hitPoint.pose.up;
            
           
        }
    }

    private void stopPlaneDectection()
    {
        foreach (var plane in m_ARPlaneManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
        m_ARPlaneManager.enabled = false;
    }
}