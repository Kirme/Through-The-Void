using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class JSONHandler : MonoBehaviour
{
    public TextAsset jsonFile;
    private Faults faultsInJson;

    private void Awake() {
        GetData();
    }

    private void GetData()
    {
        //faultsInJson = JsonUtility.FromJson<Faults>(jsonFile.text); // Old, here for documentation
        faultsInJson = JsonConvert.DeserializeObject<Faults>(jsonFile.text); // Get faults from JSON

        // Debug
        foreach (Fault fault in faultsInJson.faults) {
            Debug.Log(fault.id + " " + fault.faultLocation);
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

    public int severity;
    public string faultLocation;
    public int numVariations;
    public string fixLocation; // Remove later, exists to avoid errors with references
    public string[] fixLocations;
    public float[] fixActions;

    public string arDescription;
    public Dictionary<string, string[]> otherARDescriptions;
    public Dictionary<string, float[][]> metrics;
    public List<List<string>> effects;
    public string displayName;
    public Dictionary<string, float> negatives;
}

[System.Serializable]
public class Faults {
    public Fault[] faults;
}