using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using TMPro;

public class InteractionHandler : MonoBehaviour {
    [SerializeField] Camera _mainCamera;
    [SerializeField] GameObject _ship;
    private AudioManager _audioManager;
    [SerializeField] Slider _timeSlider;
    [SerializeField] TextMeshProUGUI _textTimeHeld;

    private float timeHeld = 0f;

    [SerializeField] Color selectedColor = Color.cyan;
    [SerializeField] Color brokenColor = Color.red;

    private Color defaultColor = Color.white;

    private FaultHandler faultHandler;

    private Transform previousPart;

    private Dictionary<string, Fault> fixesToFaults = new Dictionary<string, Fault>();
    private List<string> faultLocations = new List<string>();

    private void Awake() {
        _audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start() {
        faultHandler = GetComponent<FaultHandler>();
    }

    private void Update() {
        RegisterTouch();
        UpdateHeldText();
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
        
        Ray ray = _mainCamera.ScreenPointToRay(touch.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) { // Check if ray hits object
            Transform part = GetPart(hit);

            // Component in parts used to fix other parts
            Panel panel = part.GetComponent<Panel>();

            // Is part broken, used to fix something, etc.?
            int partStatus = GetPartStatus(part);
            UpdateInformation(part); // Update UI

            if (partStatus != 0) { // Not broken
                UpdateColor(hit, touch);
            }
            
            if (panel != null) { // Part has ability to repair
                if (HeldCorrectTime(part, panel, touch.phase)) { // Have fixed part
                    FixPart(part);
                }
            }

            previousPart = part; // Update previous part
            
        } else {
            Reset();
        }
    }

    private void Reset() {
        if (previousPart != null && GetPartStatus(previousPart) != 0)
            SetPartColor(previousPart, defaultColor);
        ResetTime();
    }

    private bool HeldCorrectTime(Transform part, Panel panel, TouchPhase phase) {
        if (previousPart == part) {
            return AddTime(panel, phase);
        }
        
        return ResetTime();
    }

    private void FixPart(Transform part) {
        string partName = part.name;

        if (fixesToFaults.ContainsKey(partName)) {
            Fault fault = fixesToFaults[partName];

            SetPartColor(fault.faultLocation, defaultColor);
            RemoveFault(fault.faultLocation, part);
            faultHandler.SendMessage(fault.id);
        }
    }

    // Based on current ship model, change based on where collider is
    private Transform GetPart(RaycastHit hit) {
        return hit.collider.transform;
    }

    private void UpdateColor(RaycastHit hit, Touch touch) {
        // Change color of part
        Transform part = GetPart(hit);

        // If stopped touching, reset to default color
        if (touch.phase == TouchPhase.Ended)
            SetPartColor(part, defaultColor);
        else
            SetPartColor(part, selectedColor);

        if (HasMovedToNewPart(touch, part)) { // We moved between two different parts
            Reset();
        }
    }

    // Update UI information
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
        2 - if part is panel, but cannot currently fix broken part
        -1 - if part has default status
     */
    private int GetPartStatus(Transform part) {
        string partName = part.transform.name;

        if (faultLocations.Contains(partName)) // Fault location
            return 0;
        if (fixesToFaults.ContainsKey(partName)) // Panel that can fix fault
            return 1;
        Panel panel = part.GetComponent<Panel>(); 
        if (panel != null) // Panel that cannot currently fix fault
            return 2;

        return -1;
    }

    // Has the player moved their finger between two parts of the ship
    private bool HasMovedToNewPart(Touch touch, Transform part) {
        return touch.phase == TouchPhase.Moved && previousPart && previousPart != part;
    }

    public void SetBroken(string name) {
        SetPartColor(name, brokenColor);
        //_audioManager.Play("Broken");
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

    // Add a fault
    public void AddFault(Fault fault, int variation) {
        if (!faultLocations.Contains(fault.faultLocation)) {
            faultLocations.Add(fault.faultLocation);
        }
        // Get fix location based on variation
        string fixLocation = fault.fixLocations[variation].Split('_')[1];

        if (!fixesToFaults.ContainsKey(fixLocation)) {
            fixesToFaults.Add(fixLocation, fault);
        }

        Transform faultLocation = _ship.transform.Find(fault.faultLocation);
    }

    // Helper function for removing a fault
    private void RemoveFaultLocation(string fault, Transform faultLocation) {
        faultLocations.Remove(fault);
        faultLocation.GetComponent<TextHandler>().ShowDescription(false);
    }

    // Helper function for removing a fix location
    private void RemoveFixLocation(Transform fix) {
        faultLocations.Remove(fix.name);
        fix.GetComponent<TextHandler>().ShowDescription(false);
    }

    // Remove a fault
    private void RemoveFault(string fault, Transform fix) {
        Transform faultLocation = _ship.transform.Find(fault);
        RemoveFaultLocation(fault, faultLocation);
        RemoveFixLocation(fix);

        _audioManager.Play("Heal");
    }

    // Functions for handling UI slider

    private bool AddTime(Panel panel, TouchPhase phase) {
        timeHeld += Time.deltaTime;

        if (phase == TouchPhase.Ended) {
            return HandleEndTime(panel);
        }

        return false;
    }

    private bool HandleEndTime(Panel panel) {
        float timeToHold = panel.GetTimeToHold();
        float margin = 0.5f;

        // Stopped holding at correct time
        if (Mathf.Abs(timeToHold - timeHeld) <= 0.5f && panel.GetCanRepair()) {
            timeHeld = 0f;
            panel.SetCanRepair(false);
            return true;
        }

        // Did not stop at correct time

        // Give margin for error, which doesn't punish
        if (timeHeld < margin)
            return false;

        // Tell VR to create consequence


        return false;
    }

    private bool ResetTime() {
        timeHeld = 0f;

        return false;
    }

    private void UpdateHeldText() {
        float margin = 0.2f;

        if (timeHeld < margin) {
            _textTimeHeld.gameObject.SetActive(false);
        }
        else if (!_textTimeHeld.IsActive()) {
            _textTimeHeld.gameObject.SetActive(true);
        }

        _textTimeHeld.text = timeHeld.ToString("0.00");
    }

    /*
     * Show slider, no longer necessary
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
    */
}
