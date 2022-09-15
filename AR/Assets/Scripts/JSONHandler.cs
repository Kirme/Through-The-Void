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

    public Fault GetFault(int id) {
        foreach (Fault fault in faultsInJson.faults) {
            if (fault.id == id)
                return fault;
        }

        return null;
    }
}

[System.Serializable]
public class Fault {
    public int id;
    public string faultLocation;
    public string fixLocation;
}

[System.Serializable]
public class Faults {
    public Fault[] faults;
}