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
        public const string Body = "Body";
        public const string Cockpit = "Cockpit";
        public const string LWing = "LWing";
        public const string RWing = "RWing";
    }

    void Update() {
        RegisterTouch();
    }

    private void RegisterTouch() {
        if (Input.touchCount <= 0) return; // Return if not touching screen

        Touch touch = Input.GetTouch(0); // Only care about first finger
        Color shipColor;

        // Color should be green when touching segment, white otherwise
        if (touch.phase == TouchPhase.Began)
            shipColor = Color.green;
        else if (touch.phase == TouchPhase.Ended)
            shipColor = Color.white;
        else
            return; // Don't need to change anything

        Ray ray = _mainCamera.ScreenPointToRay(touch.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) { // Check if ray hits object
            switch (hit.collider.transform.name) { // Switch case for different parts of ship
                case ShipParts.Cockpit:
                    hit.collider.transform.GetComponent<Renderer>().material.color = Color.yellow; // Change cockpit color to yellow
                    break;
                default:
                    hit.collider.transform.GetComponent<Renderer>().material.color = shipColor; // Change color
                    break;
            }
        }
    }
}
