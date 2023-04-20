using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Terrain;10;10;[[0,0], [0,33], [33,0], [33,33]]

public class LayerInfoClass
{
    public Position position { get; set; }
    public GameObject parent => layer.transform.parent.gameObject;
    public GameObject layer { get; set; }
    public LayerInfoClass(GameObject layer_, Color color)
    {
        layer = layer_;
        layer.transform.GetComponent<Renderer>().material.color = color;
    }
    
    public class Position
    {
        public int x { get; set; }
        public int y { get; set; }
        
        public int rank { get; set; }
    }
}
public class LayersInformation : MonoBehaviour
{
    private bool _ended = true;
    
   
    
    public void ShowLayerBtn()
    {

        var button = EventSystem.current.currentSelectedGameObject;
        
        if (!_ended)
            return;
        
        _ended = false;
        
        foreach (var layerInfoClass in Global.LayerInfoClassesList)
        {
            if(Int16.Parse(button.name) == layerInfoClass.position.rank)
            {
                if (layerInfoClass.layer.activeInHierarchy)
                    StartCoroutine(ChangePosition(layerInfoClass.layer, -(0.5f * layerInfoClass.position.rank + 0.1f), 0, false));
                else
                {
                    layerInfoClass.layer.SetActive(true);
                    StartCoroutine(ChangePosition(layerInfoClass.layer, -(0.5f * layerInfoClass.position.rank + 0.5f), 0.09f, true));
                }
            }
        }
    }

    IEnumerator ChangePosition(GameObject culture, float destination, float scale, bool setActive)
    {
        float time = 0;
        float wantedTime = 1f;
        
        while (time < wantedTime)
        {
            var localPosition = culture.transform.localPosition;
            var valuePos = Mathf.Lerp(localPosition.z, destination, time / wantedTime);
            localPosition = new Vector3(localPosition.x, localPosition.y, valuePos);
            culture.transform.localPosition = localPosition;
            
            var value = Mathf.Lerp(culture.transform.localScale.x, scale, time/wantedTime);
            culture.transform.localScale = new Vector3(value, value, value);
            
            time += Time.deltaTime;
            yield return null;
        }

        if(!setActive)
            culture.SetActive(false);
        
        _ended = true;
    }
}
