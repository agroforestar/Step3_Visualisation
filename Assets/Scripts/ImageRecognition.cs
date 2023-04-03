using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

public class ImageRecognition : MonoBehaviour
{

    private ARTrackedImageManager aRTrackedImageManager;
    private PlaceObject _placeObject;

    private void Awake()
    {
        aRTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        _placeObject = FindObjectOfType<PlaceObject>();
    }

    public void OnEnable()
    {
        aRTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    public void OnDisable()
    {
        aRTrackedImageManager.trackedImagesChanged -= OnImageChanged;
    }

    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach(var trackedImage in args.added)
        {
            _placeObject.activateScene(trackedImage.transform);
        }
    }
    
}
