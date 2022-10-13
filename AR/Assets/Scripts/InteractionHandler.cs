using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class InteractionHandler : MonoBehaviour {
    [SerializeField] Camera _mainCamera;
    [SerializeField] GameObject _ship;
    private AudioManager _audioManager;
    [SerializeField] Slider _timeSlider;

    private float timeHeld = 0f;
    private float timeToHold = 3.0f;

    [SerializeField] Color selectedColor = Color.cyan;
    [SerializeField] Color brokenColor = Color.red;

    private Color defaultColor = Color.white;
    private Color partColor;

    private FaultHandler faultHandler;

    private Transform previousPart;

    private Dictionary<string, Fault> fixesToFaults = new Dictionary<string, Fault>();
    private List<string> faultLocations = new List<string>();

    private void Start() {
        partColor = defaultColor;
        faultHandler = GetComponent<FaultHandler>();
        _audioManager = FindObjectOfType<AudioManager>();
    }

    private void Update() {
        RegisterTouch();
        UpdateSlider();
    }

    private void OnDisable() {
        Reset();
    }

    private void RegisterTouch() {
        if (Input.touchCount <= 0) {
            Reset();
            return; // Return if not touching screen
        }

        Touch touch = Input.GetTouch(0); // Only care about first finger
        partColor = HandlePhase(touch);
        
        Ray ray = _mainCamera.ScreenPointToRay(touch.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) { // Check if ray hits object
            Transform part = GetPart(hit);

            // Component in parts used to fix other parts
            PartReparation partRepair = part.GetComponent<PartReparation>();

            // Is part broken, used to fix something, etc.?
            int partStatus = GetPartStatus(part.name);
            UpdateInformation(part); // Update UI

            if (partStatus != 0) { // Not broken
                UpdateColorOnTouch(hit, touch);
            }
            
            if (partRepair != null) { // Part has ability to repair
                if (TimeExceededMax(part, partRepair)) { // Have fixed part
                    FixPart(part.name);
                }
            }

            previousPart = part; // Update previous part
            
        } else {
            Reset();
        }
    }

    private void Reset() {
        if (previousPart != null && GetPartStatus(previousPart.name) != 0)
            SetPartColor(previousPart, defaultColor);
        ResetTime();
    }

    private bool TimeExceededMax(Transform part, PartReparation partRepair) {
        if (previousPart == part) {
            return AddTime();
        }
        
        return ResetTime();
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
        return hit.collider.transform;
    }

    private void UpdateColorOnTouch(RaycastHit hit, Touch touch) {
        // Change color of part
        Transform part = GetPart(hit);
        SetPartColor(part, partColor);

        if (HasMovedToNewPart(touch, part)) { // We moved between two different parts
            Reset();
        }
    }

    private void UpdateInformation(Transform part) {
        string txtName = "Text"; // Name of object containing UI text

        if (previousPart != null && previousPart != part) {
            Transform prevTxt = previousPart.Find(txtName);
            if (prevTxt != null)
                prevTxt.gameObject.SetActive(false);
        }
        Transform txt = part.Find(txtName);
        if (txt == null)
            return;

        if (previousPart != part || !txt.gameObject.activeInHierarchy) {
            txt.gameObject.SetActive(true);
        }
    }

    /*
     Returns:
        0 - if part is broken
        1 - if part can fix broken part
        -1 - if part has default status
     */
    private int GetPartStatus(string partName) {
        if (faultLocations.Contains(partName))
            return 0;
        if (fixesToFaults.ContainsKey(partName))
            return 1;

        return -1;
    }

    // Has the player moved their finger between two parts of the ship
    private bool HasMovedToNewPart(Touch touch, Transform part) {
        return touch.phase == TouchPhase.Moved && previousPart && previousPart != part;
    }

    public void SetBroken(string name) {
        SetPartColor(name, brokenColor);
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
        if (part == null)
            return;

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

    public void AddFault(Fault fault, int variation) {
        if (!faultLocations.Contains(fault.faultLocation)) {
            faultLocations.Add(fault.faultLocation);
        }
        string fixLocation = fault.fixLocations[variation].Split('_')[1];

        if (!fixesToFaults.ContainsKey(fixLocation)) {
            fixesToFaults.Add(fixLocation, fault);
        }
    }

    private void RemoveFaultLocation(string fault, Transform faultLocation) {
        faultLocations.Remove(fault);
        faultLocation.GetComponent<TextHandler>().ShowDescription(false);
    }

    private void RemoveFixLocation(string fix, Transform faultLocation) {
        faultLocations.Remove(fix);
        faultLocation.Find(fix).GetComponent<TextHandler>().ShowDescription(false);
    }

    private void RemoveFault(string fault, string fix) {
        Transform faultLocation = _ship.transform.Find(fault);
        RemoveFaultLocation(fault, faultLocation);
        RemoveFixLocation(fix, faultLocation);

        _audioManager.Play("Heal");
    }

    private bool AddTime() {
        timeHeld += Time.deltaTime;

        if (timeHeld >= timeToHold) {
            timeHeld = 0f;
            return true;
        }

        return false;
    }

    private bool ResetTime() {
        timeHeld = 0f;

        return false;
    }

    private void UpdateSlider() {
        float margin = 0.2f;

        if (timeHeld < margin) {
            _timeSlider.gameObject.SetActive(false);
        }
        else if (!_timeSlider.IsActive()) {
            _timeSlider.gameObject.SetActive(true);
        }
        else {
            _timeSlider.value = (timeHeld - margin) / (timeToHold - margin);
        }
    }
}
