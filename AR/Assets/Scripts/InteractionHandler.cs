using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class InteractionHandler : MonoBehaviour {
    List<ARRaycastHit> _hits = new List<ARRaycastHit>();

    [SerializeField] Camera _mainCamera;

    [SerializeField] Color selectedColor = Color.cyan;
    [SerializeField] Color brokenColor = Color.red;
    private Color defaultColor = Color.white;
    private Color partColor;

    private Transform previousPart;

    private Dictionary<string, string> fixesToFaults = new Dictionary<string, string>();
    private Dictionary<string, string> faultsToFix = new Dictionary<string, string>();

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
        partColor = HandlePhase(touch);
        
        Ray ray = _mainCamera.ScreenPointToRay(touch.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) { // Check if ray hits object
            string partName = GetPart(hit).name;
            switch (GetPartStatus(partName)) {
                case 0: // Part is broken
                    break;
                case 1: // Part can fix other broken part
                    UpdateColorOnTouch(hit, touch);
                    FixPart(partName);
                    break;
                default:
                    UpdateColorOnTouch(hit, touch);
                    break;
            }
        }
    }

    private void FixPart(string partName) {
        if (fixesToFaults.ContainsKey(partName)) {
            SetPartColor(fixesToFaults[partName], defaultColor);
            RemoveFault(fixesToFaults[partName], partName);
        }
    }

    // Based on current ship model, change based on where collider is
    private Transform GetPart(RaycastHit hit) {
        return hit.collider.transform.parent;
    }

    private void UpdateColorOnTouch(RaycastHit hit, Touch touch) {
        // Change color of part
        Transform part = GetPart(hit); 
        SetPartColor(part, partColor);

        if (HasMovedToNewPart(touch, part)) { // We moved between two different parts
            SetPartColor(previousPart, defaultColor); // Reset color of previously selected part
        }

        previousPart = part; // Update previous part
    }

    /*
     Returns:
        0 - if part is broken
        1 - if part can fix broken part
        -1 - if part has default status
     */
    private int GetPartStatus(string partName) {
        if (faultsToFix.ContainsKey(partName))
            return 0;
        if (fixesToFaults.ContainsKey(partName))
            return 1;

        return -1;
    }

    // Has the player moved their finger between two parts of the ship
    private bool HasMovedToNewPart(Touch touch, Transform part) {
        return touch.phase == TouchPhase.Moved && previousPart && previousPart != part;
    }

    private void SetPartColor(string name, Color color) {
        GameObject ship = GameObject.FindGameObjectWithTag("Ship");
        foreach (Renderer partRenderer in ship.GetComponentsInChildren<Renderer>()) {
            if (string.Compare(partRenderer.gameObject.name, name, System.StringComparison.OrdinalIgnoreCase) == 0) {
                SetPartColor(partRenderer.gameObject.transform, color);
                return;
            }
        }
    }

    // Set the color of a specific ship part
    private void SetPartColor(Transform part, Color color) {
        part.GetComponent<Renderer>().material.color = color;
    }

    // Handle player starting and stopping touching the screen
    private Color HandlePhase(Touch touch) {
        if (touch.phase == TouchPhase.Began)
            return selectedColor;
        else if (touch.phase == TouchPhase.Ended)
            return defaultColor;

        return partColor;
    }

    public void AddFault(Fault fault) {
        faultsToFix.Add(fault.faultLocation, fault.fixLocation);
        fixesToFaults.Add(fault.fixLocation, fault.faultLocation);

        SetPartColor(fault.faultLocation, brokenColor);
    }

    private void RemoveFault(string fault, string fix) {
        faultsToFix.Remove(fault);
        fixesToFaults.Remove(fix);
    }
}
