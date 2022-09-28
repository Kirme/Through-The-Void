using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaultDisplay : MonoBehaviour
{
    public List<GameObject> lamps = new List<GameObject>();

    private void Start()
    {
        SetNumFaults(0);
    }

    public void SetNumFaults(int num)
    {
        foreach (GameObject lamp in lamps)
        {
            lamp.GetComponent<FaultLamp>().SetEnabled(num > lamps.IndexOf(lamp));
        }
    }
}
