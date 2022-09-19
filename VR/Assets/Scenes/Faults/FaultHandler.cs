using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaultHandler : MonoBehaviour {
    private SocketServer client;
    private JSONHandler jsonHandler;

    private Dictionary<string, Fault> faults = new Dictionary<string, Fault>();
    public GameObject player;

    void Awake() {
        client = GetComponent<SocketServer>();
    }

    private void Start() {
        StartCoroutine(ContinuousBreak());
        jsonHandler = GetComponent<JSONHandler>();
    }

    // Function called by Client when receiving information from other player
    public void ReceiveMessage(string id) {
        Debug.Log("Got message" + id);
        Fault fault = jsonHandler.GetFault(id);

        if (fault != null)
        {

        }

    }

    private void Break()
    {
        //Random Fault

        string faultID = "0";


        if (faults.ContainsKey(faultID))
        {
            return; //TODO, make sure same fault is not generated twice
        }

        Fault fault = jsonHandler.GetFault(faultID);
        faults.Add(faultID, fault);

        player.GetComponent<PlayerController>().Break(fault);
        client.SendData(faultID);
    }

    private IEnumerator ContinuousBreak()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            Break();
        }
    }
}
