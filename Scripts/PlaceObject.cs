﻿using System.Collections.Generic;
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
        for (int i = 0; i < objects.Count; i++)
        {
            GameObject prefab = Resources.Load("Prefabs/" + name + "/" + name + "_0") as GameObject;

            if (objects[i].ContainsKey("isComposed"))
            {
                if (name == "Line")
                {
                    GameObject visu = instaciateOneObject(prefab, name);
                    List<Vector3> points = (List<Vector3>)objects[i]["isComposed"];
                    Vector3 center = new Vector3(points.Average(x => x[0]), points.Average(x => x[1]), points.Average(x => x[2]));
                    visu.transform.position = center;
                    Vector3 scale = new Vector3(points.Max(x => x[0]) - points.Min(x => x[0]), points.Max(x => x[1]) - points.Min(x => x[1]), points.Max(x => x[2]) - points.Min(x => x[2])) / 2;
                    visu.transform.localScale = visu.transform.localScale + scale;
                    
                }
                else
                {
                    List<Vector3> points = (List<Vector3>)objects[i]["isComposed"];
                    int x_min = (int)points.Min(x => x[0]);
                    int x_max = (int)points.Max(x => x[0]);
                    int y_min = (int)points.Min(y => y[2]);
                    int y_max = (int)points.Max(y => y[2]);
                    for (int x = x_min; x < x_max; x++)
                    {
                        for (int y = y_min; y < y_max; y++)
                        {
                            //Debug.Log("X: " + x + "|| Y: "+ y);
                            GameObject obj_visu = Instantiate(prefab, new Vector3(x, 0, y), Quaternion.identity);
                            attachToParent(obj_visu, name);
                            obj_visu.SetActive(false);
                        }
                    }
                }

            }
            else
            {
                GameObject visu = instaciateOneObject(prefab, name);
                visu.transform.position = (Vector3)objects[i]["coord"];
            }
            //visu.transform.localScale = visu.transform.localScale;

        }
    }

    private GameObject instaciateOneObject(GameObject prefab, string name)
    {
        GameObject visu = Instantiate(prefab);
        attachToParent(visu, name);
        visu.SetActive(false);
        return visu;
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