using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hessburg;

[ExecuteInEditMode]
public class CopySunLightRotation : MonoBehaviour
{
    public SunLight SunLight; // set in inspector
    private Transform SkyBoxLightTransform;
    private Light SkyBoxLight;
    private Light SunLightSkyboxLight;
    private Light SunLightSceneLight;
    private LensFlare SunLightLensFlare;
    private Transform SunLightSkyboxLightTransform;
    private GameObject SunLightSkyboxLightGameObject;
    private GameObject SunLightSceneLightGameObject;
    private GameObject SunLightLensFlareGameObject;
    
    void Start()
    {
        if (SunLight != null)
        {
            SunLightSkyboxLight = SunLight.GetSkyboxLight();
            SunLightSceneLight = SunLight.GetSceneLight();
            SunLightLensFlare = SunLight.GetLensFlare();
            SunLightSkyboxLightTransform = SunLightSkyboxLight.transform;
            SunLightSkyboxLightGameObject = SunLightSkyboxLight.gameObject;
            SunLightSceneLightGameObject = SunLightSceneLight.gameObject;
            SunLightLensFlareGameObject = SunLightLensFlare.gameObject;
            SunLightSkyboxLightGameObject.SetActive(false);
            SunLightSceneLightGameObject.SetActive(false);
            SunLightLensFlareGameObject.SetActive(false);
            SkyBoxLightTransform=this.transform;
            SkyBoxLight=this.GetComponent<Light>();
        }   
    }

    // Update is called once per frame
    void LateUpdate()
    {
        SkyBoxLightTransform.rotation=SunLightSkyboxLightTransform.rotation;
        SkyBoxLight.color=SunLightSceneLight.color;
    }
}
