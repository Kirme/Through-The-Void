using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class InteractionHandler : MonoBehaviour {
    [SerializeField] Camera _mainCamera;
    [SerializeField] GameObject _ship;
    [SerializeField] Slider _timeSlider;
    private AudioManager _audioManager;

    [SerializeField] Color selectedColor = Color.cyan;
    [SerializeField] Color brokenColor = Color.red;

    private Color defaultColor = Color.white;
    private Color partColor;

    //Variables for Panel Handler 
    private string activePanel;
    private int CountForPanel;
    private float distX, distZ;
    private bool dragging = false;
    private float offset;
    private Vector3 v3;
    private Vector3 initPosition;
    private Vector3 finalPosition;

    private FaultHandler faultHandler;

    private Transform previousPart;

    private Dictionary<string, Fault> fixesToFaults = new Dictionary<string, Fault>();
    private Dictionary<string, Fault> faultsToFix = new Dictionary<string, Fault>();

    private bool updateFaultColors = false;

    // Time tracking
    private float timeHeld = 0f;
    private float timeToHold = 3.0f;

    private void Start() {
        partColor = defaultColor;
        faultHandler = GetComponent<FaultHandler>();
        _audioManager = FindObjectOfType<AudioManager>();
    }

    private void Update() {
        RegisterTouch();

        if (updateFaultColors)
            UpdateFaultColors();
    }

    private void OnDisable() {
        Reset();
    }

    private void UpdateFaultColors() {
        foreach (string fault in faultsToFix.Keys) {
            SetPartColor(fault, brokenColor);
        }

        updateFaultColors = false;
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
            if (dragging && part != previousPart)
            {
                part = previousPart;
            }
            int partStatus = GetPartStatus(part.name);

            switch (partStatus) {
                case 0: // Part is broken
                    UpdateInformation(part);
                    timeHeld = 0f;
                    break;
                default:
                    if (FixedPart(part)) {
                        if (partStatus == 1) // Part can fix other part
                            FixPart(part.name);
                    }

                    UpdateInformation(part);
                    UpdatePanel(part);
                    MissionHandler(activePanel, part, touch);
                    UpdateSlider(part);
                    UpdateColorOnTouch(part, touch);
                    previousPart = part;
                    break;
            }
        }
        else {
            if(!dragging)
            {
                Reset();
            }
        }
    }

    private void Reset() {
        if(previousPart.tag == "struct")
        {
            SetPartColor(previousPart, defaultColor);
        }
        timeHeld = 0f;
        if(previousPart.parent.name == "Panel2" && dragging)
        {
            previousPart.position = finalPosition;
            dragging = false;
        }
    }

    private bool FixedPart(Transform part) {
        if (previousPart == part) {
            timeHeld += Time.deltaTime;

            if (timeHeld > timeToHold) {
                timeHeld = 0f;
                return true;
            }
        } else {
            timeHeld = 0f;
        }

        return false;
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
        string tag = hit.collider.tag;
        if(tag == "struct")
        {
            return hit.collider.transform.parent;
        }
        else
        {
            return hit.collider.transform;
        }
    }

    private void UpdateColorOnTouch(Transform part, Touch touch) {
        // Change color of part
        if(part.tag == "struct")
        {
            SetPartColor(part, partColor);

            if (HasMovedToNewPart(touch, part))
            { // We moved between two different parts
                if (previousPart.tag == "struct")
                {
                    SetPartColor(previousPart, defaultColor); // Reset color of previously selected part
                }
            }
        }
     }

    private void UpdateInformation(Transform part) {
        if (previousPart != null && previousPart.tag == "struct" && previousPart != part) {
            previousPart.Find("Text").gameObject.SetActive(false);
        }
        if (part.tag == "struct" && (previousPart != part || !part.Find("Text").gameObject.activeInHierarchy)) {
            part.Find("Text").gameObject.SetActive(true);
        }
    }

    private void UpdatePanel(Transform part)
    {
        if(activePanel != "" && part.tag != "Panel" && previousPart != part)
        {
            GameObject panel = GameObject.Find(activePanel);
            if(panel != null)
            {
                panel.SetActive(false); 
                activePanel = "";
                resetPanel(panel);
            }
        }
        if (part.tag == "Button")
        {
            GameObject panel = part.GetChild(0).gameObject;
            panel.SetActive(true);
            activePanel = panel.name;
        }
    }

    private void MissionHandler(string name, Transform touchedPart, Touch touch)
    {
        switch (name)
        {
            case "Panel1":

                if (touchedPart != previousPart && touchedPart.tag == "Panel" && touchedPart.name != "Panel1")
                {
                    CountForPanel++;
                    if(CountForPanel == 2)
                    {
                        Color color1 = touchedPart.gameObject.GetComponent<Renderer>().material.color;
                        Color color2 = previousPart.gameObject.GetComponent<Renderer>().material.color;

                        if (color1 == Color.red && color2 == Color.red)
                        {
                            touchedPart.parent.GetChild(8).gameObject.SetActive(true);
                        }
                        else if (color1 == Color.green && color2 == Color.green)
                        {
                            touchedPart.parent.GetChild(7).gameObject.SetActive(true);
                        }
                        else if (color1 == Color.blue && color2 == Color.blue)
                        {
                            touchedPart.parent.GetChild(6).gameObject.SetActive(true);
                        }
                        else
                        {
                            touchedPart.parent.GetChild(6).gameObject.SetActive(false);
                            touchedPart.parent.GetChild(7).gameObject.SetActive(false);
                            touchedPart.parent.GetChild(8).gameObject.SetActive(false);
                        }
                        CountForPanel = 0;
                    }

                }
                else if(touchedPart !=previousPart)
                {
                    CountForPanel = 0;
                }
                break;

            case "Panel2":
                if(touchedPart.parent.name == "Panel2")
                {
                    if(touch.phase == TouchPhase.Began)
                    {
                        initPosition = touchedPart.position;
                        finalPosition = initPosition;
                        distX = touchedPart.position.x - _mainCamera.transform.position.x;
                        distZ = touchedPart.position.z - _mainCamera.transform.position.z;
                        v3 = new Vector3(touch.position.x, touch.position.y, distZ);
                        v3 = _mainCamera.ScreenToWorldPoint(v3);
                        offset = touchedPart.position.x - v3.x;
                        dragging = true;
                    }
                    if(dragging && touch.phase == TouchPhase.Moved)
                    {
                        v3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distZ);
                        v3 = _mainCamera.ScreenToWorldPoint(v3);
                        double newPosX = v3.x + offset;
                        if(newPosX < initPosition.x - 0.24)
                        {
                            newPosX = initPosition.x - 0.24;
                            finalPosition = new Vector3((float)newPosX, initPosition.y, initPosition.z);
                            touchedPart.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                        }
                        if(newPosX > finalPosition.x)
                        {
                            newPosX = finalPosition.x;
                        }
                        touchedPart.position = new Vector3((float)newPosX, touchedPart.position.y, touchedPart.position.z);
                    }
                }
                break;

            default:
                break;
        }
    }

    private void resetPanel(GameObject panel)
    {
        string name = panel.name;
        switch (name)
        {
            case "Panel1":
                panel.transform.GetChild(6).gameObject.SetActive(false);
                panel.transform.GetChild(7).gameObject.SetActive(false);
                panel.transform.GetChild(8).gameObject.SetActive(false);
                break;

            case "Panel2":
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

            default:
                break;
        }
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

        _audioManager.Play("Heal");
    }

    private void UpdateSlider(Transform part) {
        float margin = 0.2f;

        if (timeHeld < margin) {
            _timeSlider.gameObject.SetActive(false);
        } else if (part.tag== "Button" && !_timeSlider.IsActive()) {
            _timeSlider.gameObject.SetActive(true);
        } else {
            _timeSlider.value = (timeHeld - margin) / (timeToHold - margin);
        }
    }
}
