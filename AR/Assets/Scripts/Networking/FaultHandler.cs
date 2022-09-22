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
    public void ReceiveMessage(string id) {
        Debug.Log("Got message" + id);
        Fault fault = jsonHandler.GetFault(id);

        if (fault != null) {
            interactionHandler.AddFault(fault);
            
            if (!faultDictionary.ContainsKey(fault.id))
                faultDictionary.Add(fault.id, fault);
        }
    }

    public void SendMessage(string msg) {
        client.SendMessage(msg);
    }
}
