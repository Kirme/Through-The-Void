using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaultHandler : MonoBehaviour {
    private Client client;
    private JSONHandler jsonHandler;
    private InteractionHandler interactionHandler;

    public Dictionary<string, Fault> faultDictionary = new Dictionary<string, Fault>();

    void Awake() {
        client = GetComponent<Client>();
    }

    private void Start() {
        jsonHandler = GetComponent<JSONHandler>();
        interactionHandler = GetComponent<InteractionHandler>();

        faultDictionary = jsonHandler.GetFaultDictionary();
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
        GameObject ship = GameObject.FindGameObjectWithTag("Spawnable");

        interactionHandler.SetBroken(fault.faultLocation);
        PartReparation[] parts = ship.GetComponentsInChildren<PartReparation>();

        int var = int.Parse(variation);
        string[] fix = fault.fixLocations[var].Split('_');
        string fixPart = fix[0];
        string fixPanel = fix[1];

        foreach (PartReparation part in parts) {
            string partName = part.GetPartName();
            string panelName = part.GetName();

            if (string.Compare(partName, fixPart) == 0 && string.Compare(panelName, fixPanel) == 0) {
                part.SetCanRepair(true);
                break;
            }
        }

        interactionHandler.AddFault(fault, var);
    }

    public void SendMessage(string msg) {
        client.SendMessage(msg);
    }
}
