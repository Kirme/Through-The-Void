using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ObjectInteraction : MonoBehaviour {
    List<ARRaycastHit> _hits = new List<ARRaycastHit>();

    [SerializeField] Camera _mainCamera;

    // Class that defines ship parts to avoid string references
    static class ShipParts {
        public const string Core = "Core";
        public const string Engine = "Engine";
        public const string LWing = "LWing";
        public const string RWing = "RWing";
    }

    void Update() {
        RegisterTouch();
    }

    private void RegisterTouch() {
        if (Input.touchCount <= 0) return; // Return if not touching screen

        Touch touch = Input.GetTouch(0); // Only care about first finger

        Ray ray = _mainCamera.ScreenPointToRay(touch.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) { // Check if ray hits object
            switch (hit.collider.transform.name) { // Switch case for different parts of ship
                case ShipParts.Core:
                    
                    break;
                default:
                    break;
            }
        }
    }
}
