using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class StatsHandler : MonoBehaviour
{
    private List<Fault> faults = new List<Fault>();

    public float maxSpeedModifier = 1.0f, maxTurnSpeedModifier = 1.0f;
    public float accelerationModifier = 1.0f, turnAccelerationModifier = 1.0f;

    public bool invertX = true, invertY = false, invertZ = false;
    public bool noLight = false;
    public bool fullSpeed = true;
    public float visibility = 1.0f;

    public void ResetFaults()
    {
        faults.Clear();
        maxSpeedModifier = 1.0f; maxTurnSpeedModifier = 1.0f;
        accelerationModifier = 1.0f; turnAccelerationModifier = 1.0f;

        invertX = false; invertY = false; invertZ = false;
        noLight = false;
        fullSpeed = false;
        visibility = 1.0f;
}

    public void AddFault(Fault fault)
    {
        faults.Add(fault);

        Dictionary<string, float> negatives = fault.negatives;

        if (negatives.ContainsKey("maxSpeedModifier"))
        {
            maxSpeedModifier *= negatives["maxSpeedModifier"];
        }
        if (negatives.ContainsKey("accelerationModifier"))
        {
            accelerationModifier *= negatives["accelerationModifier"];
        }
        if (negatives.ContainsKey("maxTurnSpeedModifier"))
        {
            maxTurnSpeedModifier *= negatives["maxTurnSpeedModifier"];
        }
        if (negatives.ContainsKey("turnAccelerationModifier"))
        {
            turnAccelerationModifier *= negatives["turnAccelerationModifier"];
        }

        if (negatives.ContainsKey("invertX"))
        {
            invertX = negatives["invertX"] > 0.5;
        }
        if (negatives.ContainsKey("invertY"))
        {
            invertY = negatives["invertY"] > 0.5;
        }
        if (negatives.ContainsKey("invertZ"))
        {
            invertZ = negatives["invertZ"] > 0.5;
        }

        if (negatives.ContainsKey("noLight"))
        {
            noLight = negatives["noLight"] > 0.5;
        }
        if (negatives.ContainsKey("fullSpeed"))
        {
            fullSpeed = negatives["fullSpeed"] > 0.5;
        }
        if (negatives.ContainsKey("visibility"))
        {
            visibility *= negatives["visibility"];

            GameObject obj = GameObject.FindGameObjectWithTag("Glass");
            if (obj != null)
            {
                obj.GetComponent<Renderer>().material.color = new UnityEngine.Color(1f, 1f, 1f, 1.0f - visibility);
            }
        }

    }

    public void FixFault(Fault fault)
    {
        faults.Remove(fault);
        List<Fault> faultsCopy = new List<Fault>();

        ResetFaults();

        foreach(Fault faultCopy in faultsCopy)
        {
            AddFault(faultCopy);
        }


        
    }

}
