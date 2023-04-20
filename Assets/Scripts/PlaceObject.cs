/**
 * author: L.L.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Random = UnityEngine.Random;


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

    [SerializeField]
    private Button ButtonLayer;


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
            InstanciateType(k, Global.inScene[k]);
        }
        center = FindCenterOfTransforms(prefabInstance);
        foreach (GameObject element in prefabInstance)
        {
            scene.transform.position = center;
            element.transform.parent = scene.transform;
        }
        AddButtonlayers();
    }

    void AddButtonlayers()
    {
        if(Global.LayerInfoClassesList.Count > 0)
        {
            var totalRank = Global.LayerInfoClassesList.Max(x => x.position.rank) + 1;
            print("here");
            print(totalRank);
            for (int j = 0; j < totalRank; j++)
            {
                var btn = Instantiate(ButtonLayer, GameObject.Find("ContentScrollView").transform, true);
                btn.onClick.AddListener(() => aRSessionOrigin.GetComponent<LayersInformation>().ShowLayerBtn());
                btn.GetComponentInChildren<Text>().text = "Layer " + j;
                btn.name = j.ToString();
            }
        }
    }

    /// <summary>
    /// Instanciate all element present on the list
    /// </summary>
    /// <param name="name">name of the element</param>
    /// <param name="objects">informations (coordinates "coord", the object is composed by others ? "iscomposed")</param>
    private void InstanciateType(string name, List<Dictionary<string, object>> objects)
    {

        for (int i = 0; i < objects.Count; i++)
        {
            GameObject prefab = Resources.Load("Prefabs/" + name + "/" + name + "_0") as GameObject;
            if (objects[i].ContainsKey("isComposed"))
            {
                if (name == "Line")
                {
                    GameObject visu = instantiateOneObject(prefab, name);
                    List<Vector3> points = (List<Vector3>)objects[i]["isComposed"];
                    Vector3 center = new Vector3(points.Average(x => x[0]), points.Average(x => x[1]), points.Average(x => x[2]));
                    visu.transform.position = center;
                    Vector3 scale = new Vector3(points.Max(x => x[0]) - points.Min(x => x[0]), points.Max(x => x[1]) - points.Min(x => x[1]), points.Max(x => x[2]) - points.Min(x => x[2])) / 2;
                    visu.transform.localScale += scale;

                }
                else //case of culture
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
                            
                            GameObject obj_visu = instantiateOneObject(prefab, name);
                            obj_visu.transform.position = new Vector3(x, 0, y);
                            
                            AddLayer(x, y, new Color(255, 0, 0, 0.1f), obj_visu);

                            if(y >= y_max/2)
                                AddLayer(x, y, new Color(0, 255, 0, 0.1f), obj_visu);
                            if(y >= y_min/3)
                                AddLayer(x, y, new Color(0, 0, 255, 0.1f), obj_visu);
                            
                        }
                    }


                  
                }

            }
            else //case of indivudual plant
            {
                GameObject visu = instantiateOneObject(prefab, name);
                visu.transform.position = (Vector3)objects[i]["coord"];
            }
            

           
        }
        
        
    }

    private void AddLayer(int x, int y, Color color, GameObject parent)
    {
        int rank;
        rank = Global.LayerInfoClassesList.Where(obj => obj.position.x == x && obj.position.y == y).ToList().Count;
        
        GameObject layerInformationPrefab = Resources.Load<GameObject>("Prefabs/LayerInfos/LayerInformation");
        GameObject info_layer = Instantiate(layerInformationPrefab, parent.transform, true);
        info_layer.transform.position = new Vector3(x, 0.1f, y);
        info_layer.transform.localScale = Vector3.zero;
        

        Global.LayerInfoClassesList.Add(new LayerInfoClass(info_layer, color)
        {
            position =new LayerInfoClass.Position
            {
                x = x,
                y = y,
                rank = rank
            }
        });

        print("infoadded");

    }

  
    private GameObject instantiateOneObject(GameObject prefab, string name)
    {
        GameObject visu = Instantiate(prefab);
        attachToParent(visu, name);

        if (visu.layer == LayerMask.NameToLayer("Tree"))
            visu.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
        
        visu.SetActive(false);
        return visu;
    }

    private void attachToParent(GameObject child, string elementName)
    {
        GameObject parent = Resources.Load("Prefabs/Plant") as GameObject;
        parent = Instantiate(parent);
        child.transform.parent = parent.transform;
        parent.GetComponent<Growth>().SetDirectory(elementName);
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
        scene.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f); //0.07*
        scene.transform.position = Vector3.zero;
    }

    public void displayHalo()
    {
        Growth[] plants = (Growth[])FindObjectsOfType(typeof(Growth));
        Debug.Log(plants.Length + " 512");

        StartCoroutine(enumerator(plants));
    }

    IEnumerator enumerator(Growth[] plants)
    {
        int countWave = 0;

        foreach (Growth plant in plants)
        {
            countWave++;
            GameObject area = Resources.Load("Prefabs/Area") as GameObject;
            area = Instantiate(area);
            area.transform.position = plant.GetComponentInChildren<Renderer>().transform.position;
            area.transform.localScale = Vector3.one * 0.05f;

            if (countWave >= 5)
            {
                countWave = 0;
                yield return 0;
            }
        }
    }

}