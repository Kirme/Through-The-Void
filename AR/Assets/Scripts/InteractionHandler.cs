using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionHandler : MonoBehaviour {
    [SerializeField] Camera _mainCamera;
    [SerializeField] GameObject _ship;
    
    //[SerializeField] Slider _timeSlider;
    [SerializeField] UIHandler _UIHandler;

    private AudioManager _audioManager;
    private float timeHeld = 0f;

    [SerializeField] Color selectedColor = Color.cyan;
    [SerializeField] Color brokenColor = Color.red;

    private Color defaultColor = Color.white;

    private FaultHandler faultHandler;

    private Transform previousPart;

    private Dictionary<string, Fault> fixesToFaults = new Dictionary<string, Fault>();
    private List<string> faultLocations = new List<string>();

    // Mini games
    private int countForPanel;
    private string activePanel;
    private float distZ;
    private bool dragging = false;
    private bool holding = false;
    private float offset;
    private Vector3 initPosition;
    private Vector3 finalPosition;
    private float currentRotation = 0;

    private void Awake() {
        _audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start() {
        faultHandler = GetComponent<FaultHandler>();
    }

    private void Update() {
        RegisterTouch();
        _UIHandler.SetTimeHeld(timeHeld);
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
            
            // Handle panel
            UpdatePanel(part);
            if (part.CompareTag("Panel")) {
                MissionHandler(activePanel, part, touch);
                previousPart = part; // Update previous part
                return;
            }

            if (part.CompareTag("Button"))
                return;

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
        if (previousPart != null && GetPartStatus(previousPart) != 0 && !previousPart.CompareTag("Panel"))
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
    public bool AddFault(Fault fault, int variation) {
        // Get fix location based on variation
        string fixLocation = fault.fixLocations[variation].Split('_')[1];

        if (!faultLocations.Contains(fault.faultLocation) && !fixesToFaults.ContainsKey(fixLocation)) {
            faultLocations.Add(fault.faultLocation);
            fixesToFaults.Add(fixLocation, fault);

            return true;
        }

        return false;
    }

    // Helper function for removing a fault
    private void RemoveFaultLocation(string fault, Transform faultLocation) {
        SetPartColor(fault, defaultColor);
        faultLocations.Remove(fault);
        faultLocation.GetComponent<TextHandler>().ShowDescription(false);
    }

    // Helper function for removing a fix location
    private void RemoveFixLocation(Transform fix) {
        fixesToFaults.Remove(fix.name);
        fix.GetComponent<TextHandler>().ShowDescription(false);
    }

    // Remove a fault
    private void RemoveFault(string fault, Transform fix) {
        Transform faultLocation = _ship.transform.Find(fault);
        RemoveFaultLocation(fault, faultLocation);
        RemoveFixLocation(fix);

        //_audioManager.Play("Heal");
    }

    private bool AddTime(Panel panel, TouchPhase phase) {
        timeHeld += Time.deltaTime;

        if (phase == TouchPhase.Ended) {
            return HandleEndTime(panel);
        }

        return false;
    }

    private bool HandleEndTime(Panel panel) {
        float timeToHold = panel.timeToHold;
        float margin = 0.5f;

        // Stopped holding at correct time
        if (panel.canRepair && Mathf.Abs(timeToHold - timeHeld) <= 0.5f) {
            timeHeld = 0f;
            panel.canRepair = false;
            return true;
        }

        // Did not stop at correct time, or not on correct panel

        // Give margin for error, which doesn't punish
        if (timeHeld < margin)
            return false;

        // Tell VR to create consequence and update UI
        _UIHandler.SetMistake(panel, timeHeld);
        faultHandler.SendMessage("mistake");

        return false;
    }

    private bool ResetTime() {
        timeHeld = 0f;

        return false;
    }

    public void ClearFaults() {
        Panel[] panels = _ship.transform.GetComponentsInChildren<Panel>();

        foreach (Panel panel in panels) {
            if (!panel.canRepair)
                continue;

            RemoveFault(panel.fault, panel.transform);
        }
    }

    // Mini games

    private void MissionHandler(string name, Transform touchedPart, Touch touch) {
        Vector3 v3;

        switch (name) {
            case "Wires":
                if (touchedPart != previousPart && touchedPart.CompareTag("Panel") && touchedPart.name != "Wires") {
                    countForPanel++;

                    if (countForPanel == 2) {
                        WirePanel wirePanel = touchedPart.GetComponentInParent<WirePanel>();
                        
                        if (wirePanel.IsSolved(touchedPart, previousPart)) {
                            FixPart(touchedPart.parent.parent); // Button
                        }
                        
                        countForPanel = 0;
                    }
                }
                else if (touchedPart != previousPart) {
                    countForPanel = 0;
                }
                break;
            case "Slider":
                if (touchedPart.parent.name == "Slider") {
                    if (touch.phase == TouchPhase.Began) {
                        initPosition = touchedPart.position;
                        finalPosition = initPosition;
                        distZ = touchedPart.position.z - _mainCamera.transform.position.z;
                        v3 = new Vector3(touch.position.x, touch.position.y, distZ);
                        v3 = _mainCamera.ScreenToWorldPoint(v3);
                        offset = touchedPart.position.x - v3.x;
                        dragging = true;
                    }
                    if (dragging && touch.phase == TouchPhase.Moved) {
                        v3 = new Vector3(touch.position.x, touch.position.y, distZ);
                        v3 = _mainCamera.ScreenToWorldPoint(v3);
                        double newPosX = v3.x + offset;
                        if (newPosX < initPosition.x - 0.24) {
                            newPosX = initPosition.x - 0.24;
                            finalPosition = new Vector3((float) newPosX, initPosition.y, initPosition.z);
                            touchedPart.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                        }
                        if (newPosX > finalPosition.x) {
                            // We're done with one
                            newPosX = finalPosition.x;
                        }
                        touchedPart.position = new Vector3((float) newPosX, touchedPart.position.y, touchedPart.localPosition.z);
                    }
                }
                break;

            case "Lever":
                if (touchedPart.name == "sink") {
                    if (touch.phase == TouchPhase.Began) {
                        initPosition = touchedPart.parent.localRotation.eulerAngles;
                        distZ = touchedPart.position.z - _mainCamera.transform.position.z;
                        finalPosition = initPosition;

                        v3 = new Vector3(touch.position.x, touch.position.y, distZ);
                        v3 = _mainCamera.ScreenToWorldPoint(v3);
                        offset = v3.x;
                        dragging = true;
                    }
                    if (dragging && touch.phase == TouchPhase.Moved) {
                        v3 = new Vector3(touch.position.x, touch.position.y, distZ);
                        v3 = _mainCamera.ScreenToWorldPoint(v3);
                        float newRot = -(v3.x - offset) * 1000;
                        offset = v3.x;
                        currentRotation += newRot;
                        if (currentRotation >= 130) {
                            
                            newRot = 0;
                            finalPosition = touchedPart.parent.localRotation.eulerAngles;
                            touchedPart.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                        }
                        if (currentRotation <= 0) {
                            newRot = 0;
                        }
                        touchedPart.parent.localEulerAngles = new Vector3(touchedPart.parent.localEulerAngles.x, touchedPart.parent.localEulerAngles.y, newRot);
                    }
                }
                break;

            case "Button":
                if (!holding && touchedPart.name == "redButton") {
                    touchedPart.localPosition = new Vector3(touchedPart.localPosition.x, touchedPart.localPosition.y, touchedPart.localPosition.z - 0.3f);
                    holding = true;
                }
                else if (holding && touchedPart.name != "redButton" || touch.phase == TouchPhase.Ended) {
                    previousPart.localPosition = new Vector3(previousPart.localPosition.x, previousPart.localPosition.y, previousPart.localPosition.z + 0.3f);
                    holding = false;
                }
                break;
        }
    }

    private void ResetPanel(GameObject panel) {
        string name = panel.name;
        switch (name) {
            case "Wires":
                panel.GetComponent<WirePanel>().ResetPanel();
                break;
            case "Slider":
                Vector3 pos;
                panel.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Default");
                pos = panel.transform.GetChild(0).position;
                panel.transform.GetChild(0).position = new Vector3(initPosition.x, pos.y, pos.z);

                panel.transform.GetChild(1).gameObject.layer = LayerMask.NameToLayer("Default");
                pos = panel.transform.GetChild(1).position;
                panel.transform.GetChild(1).position = new Vector3(initPosition.x, pos.y, pos.z);

                panel.transform.GetChild(2).gameObject.layer = LayerMask.NameToLayer("Default");
                pos = panel.transform.GetChild(2).position;
                panel.transform.GetChild(2).position = new Vector3(initPosition.x, pos.y, pos.z);
                break;

            case "Lever":
                Transform child = panel.transform.GetChild(1);
                child.localEulerAngles = initPosition;
                child.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Default");
                currentRotation = 0;
                break;
        }
    }

    private void UpdatePanel(Transform part) {
        if (activePanel != "" && part.tag != "Panel" && previousPart != part) {
            GameObject panel = GameObject.Find(activePanel);
            if (panel != null) {
                panel.SetActive(false);
                activePanel = "";
                ResetPanel(panel);
            }
        }
        if (part.tag == "Button") {
            GameObject panel = part.GetChild(0).gameObject;
            panel.SetActive(true);
            activePanel = panel.name;
        }
    }
}
