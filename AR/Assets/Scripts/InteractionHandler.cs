using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class InteractionHandler : MonoBehaviour {
    [SerializeField] Camera _mainCamera;
    [SerializeField] GameObject _ship;

    [SerializeField] Color selectedColor = Color.cyan;
    [SerializeField] Color brokenColor = Color.red;
    private Color defaultColor = Color.white;
    private Color partColor;

    private FaultHandler faultHandler;

    private Transform previousPart;

    private Dictionary<string, Fault> fixesToFaults = new Dictionary<string, Fault>();
    private Dictionary<string, Fault> faultsToFix = new Dictionary<string, Fault>();

    private bool updateFaultColors = false;

    // Time tracking
    private float timeHeld = 0f;
    private float timeToHold = 1.0f;

    private void Start() {
        partColor = defaultColor;
        faultHandler = GetComponent<FaultHandler>();
    }

    void Update() {
        RegisterTouch();

        if (updateFaultColors)
            UpdateFaultColors();
    }

    private void UpdateFaultColors() {
        foreach (string fault in faultsToFix.Keys) {
            SetPartColor(fault, brokenColor);
        }

        updateFaultColors = false;
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
                    if (previousPart == GetPart(hit)) {
                        timeHeld += Time.deltaTime;

                        if (timeHeld > timeToHold) {
                            FixPart(partName);
                        }
                    } else {
                        timeHeld = 0f;
                    }

                    UpdateColorOnTouch(hit, touch);
                    break;
                default:
                    UpdateColorOnTouch(hit, touch);
                    break;
            }
        } else {
            SetPartColor(previousPart, defaultColor);
            previousPart = null;
        }
    }

    private void FixPart(string partName) {
        if (fixesToFaults.ContainsKey(partName)) {
            Fault fault = fixesToFaults[partName];

            SetPartColor(fault.faultLocation, defaultColor);
            RemoveFault(fault.faultLocation, partName);
            faultHandler.SendMessage(fault.id);
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
        foreach (Renderer partRenderer in _ship.GetComponentsInChildren<Renderer>()) {
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
        if (!faultsToFix.ContainsKey(fault.faultLocation)) {
            faultsToFix.Add(fault.faultLocation, fault);
            updateFaultColors = true;
        }
            
        if (!fixesToFaults.ContainsKey(fault.fixLocation)) {
            fixesToFaults.Add(fault.fixLocation, fault);
            updateFaultColors = true;
        }
    }

    private void RemoveFault(string fault, string fix) {
        faultsToFix.Remove(fault);
        fixesToFaults.Remove(fix);
    }
}
