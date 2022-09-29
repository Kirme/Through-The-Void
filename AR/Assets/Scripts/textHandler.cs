using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class textHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public class Fault
    {
        public string id;
        public string faultLocation;
        public string faultName;
        public string fixLocation;
    }

    public class Faults
    {
        public Fault[] faults;
    }

    public TextMeshPro Text;
    public TextAsset json;
    private Faults faultsInJson;

    void Start()
    {
        Text = GetComponent<TextMeshPro>();
        Text.text = gameObject.transform.parent.name;
        Text.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Show()
    {
        Text.gameObject.SetActive(true);
    }

    public void Hide()
    {
        Text.gameObject.SetActive(false);
    }

    public string ReadJSON()
    {
        string result = "Wing";
        faultsInJson = JsonUtility.FromJson<Faults>(json.text);

        foreach (Fault fault in faultsInJson.faults)
        {
            if(fault.faultLocation == "text")
            {
                result += "\n fault : " + fault.faultName;
                result += "\n To do : " + fault.fixLocation;
            }
        }
        return result;
    }
}
