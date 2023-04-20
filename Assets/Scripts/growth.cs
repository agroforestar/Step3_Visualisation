/**
 * author: L.L.
 */

using UnityEngine;

/// <summary>
/// Change the prefab of 3D object to simulate the growth
/// TO DO : modify to instatiante at the openning of the scene and not when chnge prefab is called (optimization)
/// </summary>

public class Growth : MonoBehaviour
{
    private string dir;
    private string name;
    private int currentVisu = 0;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(ChangePrefab), 10.0f, 3.0f);
    }


    private void ChangePrefab()
    {
        object[] allVisu = Resources.LoadAll(dir);

        GameObject current = this.transform.GetChild(0).gameObject;
        
        currentVisu = (currentVisu+1)%allVisu.Length;
        GameObject newVisu = Resources.Load(dir + name + "_" + currentVisu) as GameObject;
        newVisu = Instantiate(newVisu, this.transform, true);
        newVisu.transform.position = current.transform.position;
        newVisu.transform.rotation = current.transform.rotation;
        newVisu.transform.localScale = current.transform.localScale * current.transform.parent.transform.localScale.x;


        var childCount = current.transform.childCount;
        if (childCount > 0)
            for (int i = 0; i < childCount; i++)
                current.transform.GetChild(0).transform.SetParent(newVisu.transform);
        
        Destroy(current);
    }

    public void SetDirectory(string plantName)
    {
        dir = "Prefabs/" + plantName +"/";
        this.name = plantName;
    }
}
