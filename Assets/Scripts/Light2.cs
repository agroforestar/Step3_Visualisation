using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  TO Do vérifier code n'est plus utilisé car remplace rpar l'asset
/// SimpleFileBrowser ok supprimer class
/// </summary>
public class Light2 : MonoBehaviour
{

    public float theta;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.RotateAround(Vector3.zero, Vector3.right, theta);
    }
}
