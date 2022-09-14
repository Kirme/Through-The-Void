using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaultHandler : MonoBehaviour {
    private Client client;

    void Awake() {
        client = FindObjectOfType<Client>(); // Find client in scene
    }

    // Update is called once per frame
    void Update() {
        
    }

    // Function called when receiving information from other player
    public void ReceiveMessage(/* Some data in here */) {

    }
}
