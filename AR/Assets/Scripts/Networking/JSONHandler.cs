using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONHandler : MonoBehaviour
{
    public TextAsset jsonFile;
    private Faults faultsInJson;

    private void Start() {
        GetData();
    }

    private void GetData()
    {
        faultsInJson = JsonUtility.FromJson<Faults>(jsonFile.text); // Get faults from JSON

        // Debug
        foreach (Fault fault in faultsInJson.faults) {
            Debug.Log(fault.id + " " + fault.faultLocation + " " + fault.fixLocation);
        }
    }

    public Dictionary<string, Fault> GetFaultDictionary() {
        Dictionary<string, Fault> dict = new Dictionary<string, Fault>();

        foreach (Fault fault in faultsInJson.faults) {
            if (!dict.ContainsKey(fault.id))
                dict.Add(fault.id, fault);
        }

        return dict;
    }

    public Fault GetFault(string id) {
        foreach (Fault fault in faultsInJson.faults) {
            if (string.Compare(fault.id, id) == 0)
                return fault;
        }

        return null;
    }
}

[System.Serializable]
public class Fault {
    public string id;
    public string faultLocation;
    public string fixLocation;
}

[System.Serializable]
public class Faults {
    public Fault[] faults;
}