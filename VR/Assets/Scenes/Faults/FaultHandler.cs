using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class FaultHandler : MonoBehaviour {
    private SocketServer client;
    private JSONHandler jsonHandler;
    public GameObject frontText;
    private TextMesh frontTextComponent;
    public GameObject sideText, connectedLabel, startGameKnob;
    private TextMesh sideTextComponent;

    private Dictionary<string, Fault> faults = new Dictionary<string, Fault>();
    public GameObject player;
    private List<string> queuedFixes = new List<string>();
    private int queuedMistakes = 0;
    public UnityEvent<Fault, int> onBreak;
    public UnityEvent<Fault> onFix;

    private bool tutorialMode = true;

    [ContextMenu("Generate Fault")]
    public void GenerateFault()
    {
        Break();
    }

    [ContextMenu("Generate Fault 1")]
    public void GenerateFault1()
    {
        string faultID = "0";
        Fault fault = jsonHandler.GetFault(faultID);
        fault.SetVariation();
        /*if (faults.ContainsKey(faultID))
        {
            return; //TODO, make sure same fault is not generated twice
        }*/

        faults.Add(faultID, fault);

        onBreak.Invoke(fault, faults.Count);

        player.GetComponent<PlayerController>().Break(fault, faults.Count);
        client.SendData(faultID + " " + fault.GetVariation());
        frontTextComponent.text = CreateFrontText();
    }
    [ContextMenu("Generate Fault 2")]
    public void GenerateFault2()
    {
        string faultID = "1";
        Fault fault = jsonHandler.GetFault(faultID);
        fault.SetVariation();
        /*if (faults.ContainsKey(faultID))
        {
            return; //TODO, make sure same fault is not generated twice
        }*/

        faults.Add(faultID, fault);

        onBreak.Invoke(fault, faults.Count);

        player.GetComponent<PlayerController>().Break(fault, faults.Count);
        client.SendData(faultID + " " + fault.GetVariation());
        frontTextComponent.text = CreateFrontText();
    }
    [ContextMenu("Generate Fault 3")]
    public void GenerateFault3()
    {
        string faultID = "2";
        Fault fault = jsonHandler.GetFault(faultID);
        fault.SetVariation();
        /*if (faults.ContainsKey(faultID))
        {
            return; //TODO, make sure same fault is not generated twice
        }*/

        faults.Add(faultID, fault);

        onBreak.Invoke(fault, faults.Count);

        player.GetComponent<PlayerController>().Break(fault, faults.Count);
        client.SendData(faultID + " " + fault.GetVariation());
        frontTextComponent.text = CreateFrontText();
    }

    public void SetTutorialMode(bool val)
    {
        tutorialMode = val;
        if (client.CheckConnection())
        {
            Break();
        }
    }

    void Awake() {
        client = GetComponent<SocketServer>();

    }

    private void Start() {
        StartCoroutine(ContinuousBreak());
        jsonHandler = GetComponent<JSONHandler>();
        frontTextComponent = frontText.GetComponent<TextMesh>();
        sideTextComponent = sideText.GetComponent<TextMesh>();
        sideTextComponent.text = CreateSideText();
        

    }

    // Function called by Client when receiving information from other player
    public void ReceiveMessage(string id) {
        Debug.Log("Got message" + id);
        Fault fault = jsonHandler.GetFault(id);

        if (fault != null)
        {

        }

    }

    private void Update()
    {
        if(connectedLabel != null)
        {
            connectedLabel.SetActive(!client.CheckConnection());
        }

        if(startGameKnob != null && tutorialMode)
        {
            startGameKnob.GetComponent<OnOffKnob>().SetDisabled(!client.CheckConnection());
            startGameKnob.GetComponent<SphereCollider>().enabled = client.CheckConnection();
        }

        if(queuedMistakes > 0)
        {
            player.GetComponent<PlayerController>().TakeDamage(queuedMistakes * 25);
            queuedMistakes = 0;
        }

        if(queuedFixes.Count > 0)
        {
            Fix(queuedFixes[0]);

            queuedFixes.RemoveAt(0);
        }
    }

    public int DictionaryLength()
    {
        return faults.Count;
    }

    public void QueueMistake()
    {
        queuedMistakes += 1;
    }

    public void QueueFix(string id)
    {
        queuedFixes.Add(id);
    }
    public void FixAll()
    {
        faults.Clear();
        player.GetComponent<PlayerController>().FixAll();
    }
    public void Fix(string id)
    {
        id = (int.Parse(id)).ToString();
       

        Debug.Log("'" + id + "'");
        Debug.Log("map:");
       
        foreach(string key in faults.Keys)
        {
            Debug.Log("'" + key + "'");
        }
        
        if (!faults.ContainsKey(id))
        {
            Debug.Log("Cannot fix fault with id " + id + ", because it doesn't exist among current faults.");
            return;
        }

        Fault fault = jsonHandler.GetFault(id);
        onFix.Invoke(fault);

        faults.Remove(id);
        player.GetComponent<PlayerController>().Fix(fault, DictionaryLength());

        Debug.Log("Fixed " + id);
        //frontTextComponent.text = CreateFrontText(); // TODO: What to do with the front text?
    }

    public void Break()
    {
        Debug.Log("break!");
        //Random Fault

        string faultID = ""; //Spaghetti carbonara
        //TODO: Put non-active faults in a seperate list, and randomize from there
        for(int i=0; i<30; i++)
        {
            int rnd = Random.Range(0, jsonHandler.faultsInJson.faults.Length);
            faultID = rnd.ToString();
            if (!faults.ContainsKey(faultID))
            {
                break; //TODO, make sure same fault is not generated twice
            }

        }
        if (faults.ContainsKey(faultID))
        {
            return;
        }
        Debug.Log(faultID);


        Fault fault = jsonHandler.GetFault(faultID);
        fault.SetVariation();
        /*if (faults.ContainsKey(faultID))
        {
            return; //TODO, make sure same fault is not generated twice
        }*/

        faults.Add(faultID, fault);

        onBreak.Invoke(fault, faults.Count);

        player.GetComponent<PlayerController>().Break(fault, faults.Count);
        client.SendData(faultID + " " + fault.GetVariation());
        frontTextComponent.text = CreateFrontText();
        Debug.Log("Broken:");
        foreach (string key in faults.Keys)
        {
            Debug.Log(key + " variation: " + fault.GetVariation());
        }

    }

    public string CreateFrontText()
    {
        string returnvalue = "Components currently broken: \n";
        foreach (var value in faults.Values)
        {
            returnvalue += value.faultLocation + ", " + "can be fixed in: " + value.fixLocation + "\n";
        }
        return returnvalue;
    }
    private string CreateSideText()
    {
        Debug.Log("Making side text");
        float player_hitpoints = player.GetComponent<PlayerController>().GetHitpoints();
        string returnvalue = "Estimated hull integrity: " + Mathf.RoundToInt(player.GetComponent<PlayerController>().GetHitpoints()).ToString() + "%\n" + "Reparation manual: \n";
        //string returnvalue = "Reparation manual: \n";
        foreach (Fault value in jsonHandler.faultsInJson.faults)
        {
            returnvalue += value.faultLocation + ", " + "can be fixed in: " + value.fixLocation + "\n";
        }

        return returnvalue;
    }

    public void UpdateSideText(){
        sideTextComponent.text = CreateSideText();
    }

    private IEnumerator ContinuousBreak()
    {
        while (true)
        {
            if (client.CheckConnection())
            {
                yield return new WaitForSeconds(60);
                if (!tutorialMode)
                {
                    Break();
                } 

            }
            else {
                yield return new WaitForSeconds(5);
                //Break();
            }      
        }
    }
}
