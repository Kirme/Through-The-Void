using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{
    private bool canRepair = false;
    private float timeToHold = 0.0f;

    // Get name of ship part panel is attached to
    public string GetPartName() {
        return transform.parent.name;
    }

    // Get  name of panel
    public string GetPanelName() {
        return transform.name;
    }

    public void SetTimeToHold(float setTo) {
        timeToHold = setTo;
    }

    public float GetTimeToHold() {
        return timeToHold;
    }

    public void SetCanRepair(bool setTo) {
        canRepair = setTo;
    }

    public bool GetCanRepair() {
        return canRepair;
    }
}
