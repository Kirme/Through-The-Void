using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaultHandler : MonoBehaviour {
    private Client client;
    private JSONHandler jsonHandler;
    private InteractionHandler interactionHandler;

    public Dictionary<string, Fault> faultDictionary = new Dictionary<string, Fault>();

    private GameObject ship;

    void Awake() {
        client = GetComponent<Client>();
    }

    private void Start() {
        jsonHandler = GetComponent<JSONHandler>();
        interactionHandler = GetComponent<InteractionHandler>();

        faultDictionary = jsonHandler.GetFaultDictionary();

        ship = GameObject.FindGameObjectWithTag("Spawnable");
    }

    // Function called by Client when receiving information from other player
    public void ReceiveMessage(string id, string variation) {
        Debug.Log("Got message " + id);
        Fault fault = jsonHandler.GetFault(id);

        if (fault != null) {
            ParseFault(fault, variation);

            if (!faultDictionary.ContainsKey(fault.id))
                faultDictionary.Add(fault.id, fault);
        }
    }

    private void ParseFault(Fault fault, string variation) {
        interactionHandler.SetBroken(fault.faultLocation);
        PartReparation[] parts = ship.GetComponentsInChildren<PartReparation>();

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

        string fixDesc = fault.otherARDescriptions[fault.fixLocations[var]][var];
        // Find fix location and set text
        foreach (PartReparation part in parts) {
            string partName = part.GetPartName();
            string panelName = part.GetName();
            
            //HandleDescription(th, panelName + ": " + partName + " = " + fixPart);
            if (string.Compare(partName, fixPart) == 0 && string.Compare(panelName, fixPanel) == 0) {
               part.SetRepaired(false);
                
                HandleDescription(part.GetComponent<TextHandler>(), fixDesc);
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
