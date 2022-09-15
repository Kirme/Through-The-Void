using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONHandler : MonoBehaviour
{
    public TextAsset jsonFile;

    private void Start() {
        GetData();
    }

    void GetData()
    {
        Faults faultsInJson = JsonUtility.FromJson<Faults>(jsonFile.text);

        foreach (Fault fault in faultsInJson.faults) {
            Debug.Log(fault.id + " " + fault.faultLocation + " " + fault.fixLocation);
        }
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