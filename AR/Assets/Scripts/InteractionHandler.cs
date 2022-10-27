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
    private string activePanel = "";
    private bool holding = false;

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
            Transform part = hit.collider.transform;

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
        ResetTime();
        if (previousPart == null)
            return;

        if (GetPartStatus(previousPart) != 0 && !previousPart.CompareTag("Panel")) {
            ResetColor(previousPart);
        }
    }

    private bool HeldCorrectTime(Transform part, Panel panel, TouchPhase phase) {
        if (previousPart == part) {
            return AddTime(panel, phase);
        }
        
        return ResetTime();
    }

    private void FixPart(Transform part) {
        string key = GetFixKey(part);

        if (fixesToFaults.ContainsKey(key)) {
            Fault fault = fixesToFaults[key];

            RemoveFault(fault.faultLocation, part);
            faultHandler.SendMessage(fault.id);
        } else {
            _UIHandler.SetMistake();
            faultHandler.SendMessage("mistake");
        }
    }

    private void UpdateColor(RaycastHit hit, Touch touch) {
        // Change color of part
        Transform part = hit.collider.transform;

        // If stopped touching, reset to default color
        if (touch.phase == TouchPhase.Ended) {
            ResetColor(part);
        } else
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
        if (fixesToFaults.ContainsKey(GetFixKey(part))) // Panel that can fix fault
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

        ColorTracker ct = part.GetComponent<ColorTracker>();
        ct.SetColor(color);
    }

    private void ResetColor(string name) {
        foreach (Renderer partRenderer in _ship.GetComponentsInChildren<Renderer>()) {
            if (string.Compare(partRenderer.gameObject.name, name, System.StringComparison.OrdinalIgnoreCase) == 0) {
                ResetColor(partRenderer.gameObject.transform);
                return;
            }
        }
    }

    private void ResetColor(Transform part) {
        ColorTracker ct = part.GetComponent<ColorTracker>();
        ct.ResetColors();
    }

    // Add a fault
    public bool AddFault(Fault fault, int variation) {
        // Get fix location based on variation
        string fixLocation = fault.fixLocations[variation];

        if (!faultLocations.Contains(fault.faultLocation) && !fixesToFaults.ContainsKey(fixLocation)) {
            faultLocations.Add(fault.faultLocation);
            fixesToFaults.Add(fixLocation, fault);

            return true;
        }

        return false;
    }

    // Helper function for removing a fault
    private void RemoveFaultLocation(string fault, Transform faultLocation) {
        ResetColor(fault);
        faultLocations.Remove(fault);
        faultLocation.GetComponent<TextHandler>().ShowDescription(false);
    }

    // Helper function for removing a fix location
    private void RemoveFixLocation(Transform fix) {
        fixesToFaults.Remove(GetFixKey(fix));
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
            case "Button":
                if (string.Compare(touchedPart.name, "redButton") == 0) {
                    ButtonPanel buttonPanel = touchedPart.GetComponentInParent<ButtonPanel>();

                    if (buttonPanel.IsSolved(touchedPart, touch)) {
                        FixPart(touchedPart.parent.parent); // Button
                    }

                    holding = true;
                } else if (string.Compare(previousPart.name, "redButton") == 0) {
                    ButtonPanel buttonPanel = previousPart.GetComponentInParent<ButtonPanel>();
                    buttonPanel.ResetPanel(previousPart);
                }

                break;
        }
    }

    private void UpdatePanel(Transform part) {
        if (activePanel != "" && part.tag != "Panel" && previousPart != part) {
            GameObject panel = GameObject.Find(activePanel);
            if (panel != null) {
                panel.SetActive(false);
                activePanel = "";

                // Reset wires
                if (string.Compare(panel.name, "Wires") == 0) {
                    panel.GetComponent<WirePanel>().ResetPanel();
                }
            }
        }
        if (part.tag == "Button") {
            GameObject panel = part.GetChild(0).gameObject;
            panel.SetActive(true);
            activePanel = panel.name;
        }
    }

    // Gets "parent_child" from transform
    private string GetFixKey(Transform fix) {
        string val = fix.parent.name;
        val = string.Concat(val, "_");
        val = string.Concat(val, fix.name);

        return val;
    }
}
