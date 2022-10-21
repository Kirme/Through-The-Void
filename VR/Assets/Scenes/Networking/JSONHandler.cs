using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Security.Cryptography;

public class JSONHandler : MonoBehaviour
{
    public TextAsset jsonFile;
    public Faults faultsInJson;

    private void Awake()
    {
        GetData();
    }

    private void GetData()
    {
        //faultsInJson = JsonUtility.FromJson<Faults>(jsonFile.text); // Old, here for documentation
        faultsInJson = JsonConvert.DeserializeObject<Faults>(jsonFile.text); // Get faults from JSON

        // Debug
        foreach (Fault fault in faultsInJson.faults)
        {
            Debug.Log(fault.id + " " + fault.faultLocation);
        }
    }

    public Dictionary<string, Fault> GetFaultDictionary()
    {
        Dictionary<string, Fault> dict = new Dictionary<string, Fault>();

        foreach (Fault fault in faultsInJson.faults)
        {
            if (!dict.ContainsKey(fault.id))
                dict.Add(fault.id, fault);
        }

        return dict;
    }

    public Fault GetFault(string id)
    {
        foreach (Fault fault in faultsInJson.faults)
        {
            if (string.Compare(fault.id, id) == 0)
                return fault;
        }

        return null;
    }
}

[System.Serializable]
public class Fault
{
    public string id;

    public int severity;
    public string faultLocation;
    public int numVariations;
    public int variation;
    public string fixLocation; // Remove later, exists to avoid errors with references
    public string[] fixLocations;
    public string[] fixActions;

    public string arDescription;
    public Dictionary<string, string[]> otherARDescriptions;
    public Dictionary<string, float[][]> metrics;
    public List<string[]> effects;
    public string displayName;
    public Dictionary<string, float> negatives;

    public void SetVariation()
    {
        this.variation = RandomNumberGenerator.GetInt32(numVariations);
    }

    public int GetVariation()
    {
        return variation;
    }

    public string GetFixLocation()
    {
        return fixLocations[variation];
    }

    public string GetFixAction()
    {
        return fixActions[variation];
    }

    public Dictionary<string, float[]> GetMetrics()
    {
        Debug.Log(metrics);

        if(metrics == null)
        {
            return new Dictionary<string, float[]>(); ;
        }

        Dictionary<string, float[]> newMetrics = new Dictionary<string, float[]>();

        foreach (string key in metrics.Keys)
        {
            newMetrics[key] = metrics[key][variation];
        }

        return newMetrics;
    }

    public Dictionary<string, string> GetOtherARDescriptions()
    {

        Dictionary<string, string> descriptions = new Dictionary<string, string>();

        foreach (string key in metrics.Keys)
        {
            descriptions[key] = otherARDescriptions[key][variation];
        }

        return descriptions;
    }

    public List<string> GetEffects()
    {
        List<string> newEffects = new List<string>();

        foreach(string[] dat in effects){
            newEffects.Add(dat[variation]);
        }

        return newEffects;
    }

}

[System.Serializable]
public class Faults
{
    public Fault[] faults;
}