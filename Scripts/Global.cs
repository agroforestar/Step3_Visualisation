using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Global : MonoBehaviour
{

    public static Loading loading;
    public static Dictionary<string, List<Vector3>> inScene;


    public enum LoadingType
    {
        None, LoadLevel1,LoadMenu
    }

   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }
}
