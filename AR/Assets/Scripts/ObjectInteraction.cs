using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ObjectInteraction : MonoBehaviour {
    List<ARRaycastHit> _hits = new List<ARRaycastHit>();

    [SerializeField] Camera _mainCamera;

    [SerializeField] Color selectedColor = Color.yellow;
    private Color defaultColor = Color.white;
    private Color partColor;

    // Class that defines ship parts to avoid string references
    static class ShipParts {
        public const string Core = "Core";
        public const string Engine = "Engine";
        public const string LWing = "LWing";
        public const string RWing = "RWing";
    }

    private void Start() {
        partColor = defaultColor;
    }

    void Update() {
        RegisterTouch();
    }

    private void RegisterTouch() {
        if (Input.touchCount <= 0) return; // Return if not touching screen

        Touch touch = Input.GetTouch(0); // Only care about first finger

        if (touch.phase == TouchPhase.Began)
            partColor = selectedColor;
        else if (touch.phase == TouchPhase.Ended)
            partColor = defaultColor;

        Ray ray = _mainCamera.ScreenPointToRay(touch.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) { // Check if ray hits object
            switch (hit.collider.transform.name) { // Switch case for different parts of ship
                default:
                    // Change color of part
                    Transform part = hit.collider.transform.parent;
                    part.GetComponent<Renderer>().material.color = partColor;
                    break;
            }
        }
    }
}
