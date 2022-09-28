using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class FaultLamp : MonoBehaviour
{
    public void SetEnabled(bool val)
    {
        GetComponent<Light>().enabled = val;
        GetComponent <Renderer>().material.SetColor("_EmissionColor", val ? new Color(1, 0, 0, 1) : new Color(0,0,0,0)); 
    }
}
