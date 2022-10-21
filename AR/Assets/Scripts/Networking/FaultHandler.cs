using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaultHandler : MonoBehaviour {
    private Client client;
    private JSONHandler jsonHandler;
    private InteractionHandler interactionHandler;

    public Dictionary<string, Fault> faultDictionary = new Dictionary<string, Fault>();

    [SerializeField] GameObject ship;
    private Fault currentFault;
    private string currentVar;
    private bool addFault = false;

    void Awake() {
        client = GetComponent<Client>();
    }

    private void Start() {
        jsonHandler = GetComponent<JSONHandler>();
        interactionHandler = GetComponent<InteractionHandler>();

        faultDictionary = jsonHandler.GetFaultDictionary();
    }

    private void Update() {
        if (currentFault != null && addFault) {
            ParseFault(currentFault, currentVar);

            if (!faultDictionary.ContainsKey(currentFault.id))
                faultDictionary.Add(currentFault.id, currentFault);

            addFault = false;
        }
    }

    public void ReceiveMessage(string msg) {
        Debug.Log("Got message " + msg);

        interactionHandler.ClearFaults();
    }

    // Function called by Client when receiving information from other player
    public void ReceiveMessage(string id, string variation) {
        Debug.Log("Got message " + id);
        Fault fault = jsonHandler.GetFault(id);

        if (fault != null) {
            currentFault = fault;
            currentVar = variation;

            addFault = true;
        }
    }

    private void ParseFault(Fault fault, string variation) {
        interactionHandler.SetBroken(fault.faultLocation);
        Panel[] panels = ship.GetComponentsInChildren<Panel>();

        int var = int.Parse(variation);
        string[] fix = fault.fixLocations[var].Split('_');
        string fixPart = fix[0];
        string fixPanel = fix[1];

        // Set text of fault location
        TextHandler th = ship.transform.Find(fault.faultLocation).GetComponent<TextHandler>();

        // Construct description of fix location, seen at fault location
        List<string> description = new List<string>();
        description.Add("Find ");
        description.Add(fixPanel);
        description.Add(" at the ");
        description.Add(fixPart);

        HandleDescription(th, string.Concat(description));
        /*
         * Show description for all panels
        foreach (string key in fault.otherARDescriptions.Keys) {
            string[] l = key.Split("_");
            Panel panel = ship.transform.Find(l[0]).Find(l[1]).GetComponent<Panel>();
            HandleDescription(panel.GetComponent<TextHandler>(), fault.otherARDescriptions[key][var]);
        }
        */
        string fixDesc = fault.otherARDescriptions[fault.fixLocations[var]][var];
        // Find fix location and set text
        foreach (Panel panel in panels) {
            string partName = panel.GetPartName();
            string panelName = panel.GetPanelName();
            
            //HandleDescription(th, panelName + ": " + partName + " = " + fixPart);
            if (string.Compare(partName, fixPart) == 0 && string.Compare(panelName, fixPanel) == 0) {
                panel.SetTimeToHold(fault.fixActions[var]);
                panel.SetCanRepair(true);
                HandleDescription(panel.GetComponent<TextHandler>(), fixDesc);

                break;
            }
        }

        interactionHandler.AddFault(fault, var);
    }

    private void HandleDescription(TextHandler th, string desc) {
        if (th != null) {
            th.SetDescription(desc);
            th.ShowDescription(true);
        }
    }

    public void SendMessage(string msg) {
        client.SendMessage(msg);
    }
}
