using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class PlaceTrackedImages : MonoBehaviour {
    private ARTrackedImageManager _trackedImageManager;
    public GameObject[] ArPrefabs; // List of prefabs to instantiate
    private readonly Dictionary<string, GameObject> _instantiatedPrefabs = new Dictionary<string, GameObject>();

    private void Awake() {
        _trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    // On enable, should call function OnTrackedImagesChanged
    private void OnEnable() {
        _trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    // On disable, should stop calling function OnTrackedImagesChanged
    private void OnDisable() {
        _trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    // Handles tracking of images and placing of gameObjects
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs) {
        foreach (ARTrackedImage trackedImage in eventArgs.added) { // New image added
            string imageName = trackedImage.referenceImage.name; // Get name

            foreach (GameObject curPrefab in ArPrefabs) { // Go through all potential prefabs
                if (string.Compare(curPrefab.name, imageName, System.StringComparison.OrdinalIgnoreCase) == 0
                    && !_instantiatedPrefabs.ContainsKey(imageName)) { // If we should place an object on this image, and haven't already
                    // Instantiate prefab
                    GameObject newPrefab = Instantiate(curPrefab, trackedImage.transform);
                    _instantiatedPrefabs[imageName] = newPrefab;
                }
            }
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated) { // Check if created images should be tracked
            _instantiatedPrefabs[trackedImage.referenceImage.name]
            .SetActive(trackedImage.trackingState == TrackingState.Tracking);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed) { // Remove tracked image
            Destroy(_instantiatedPrefabs[trackedImage.referenceImage.name]);
        }
    }
}
