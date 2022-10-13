using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartReparation : MonoBehaviour
{
    private bool repaired = false;
    private string shipPart;

    private void Start() {
    }

    public string GetPartName() {
        return transform.parent.name;
    }

    public string GetName() {
        return transform.name;
    }

    public void SetRepaired(bool setTo) {
        repaired = setTo;
    }

    public bool GetRepaired() {
        return repaired;
    }
}
