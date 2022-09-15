using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaultHandler : MonoBehaviour {
    private Client client;
    private JSONHandler jsonHandler;
    private InteractionHandler interactionHandler;

    void Awake() {
        //client = GetComponent<Client>();
        
    }

    private void Start() {
        jsonHandler = GetComponent<JSONHandler>();
        interactionHandler = GetComponent<InteractionHandler>();
        StartCoroutine(SendFault());
    }

    IEnumerator SendFault() {
        yield return new WaitForSeconds(5);

        ReceiveMessage(1);
    }

    // Function called by Client when receiving information from other player
    public void ReceiveMessage(int id) {
        Fault fault = jsonHandler.GetFault(id);

        if (fault != null)
            interactionHandler.AddFault(fault);
    }

    public void SendMessage(Fault fault) {
        // TODO Send message
    }
}
