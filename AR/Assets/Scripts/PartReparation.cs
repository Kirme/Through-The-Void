using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartReparation : MonoBehaviour
{
    private bool canRepair = false;
    private string shipPart;

    private void Start() {
        shipPart = transform.parent.name;
    }

    public string GetPartName() {
        return shipPart;
    }

    public string GetName() {
        return transform.name;
    }

    public void SetCanRepair(bool setTo) {
        canRepair = setTo;
    }

    public bool GetCanRepair() {
        return canRepair;
    }
}
