using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WirePanel : MonoBehaviour {
    [SerializeField] GameObject red;
    [SerializeField] GameObject green;
    [SerializeField] GameObject blue;

    public bool IsSolved(Transform p1, Transform p2) {
        Color color1 = p1.gameObject.GetComponent<Renderer>().material.color;
        Color color2 = p2.gameObject.GetComponent<Renderer>().material.color;

        if (color1 == color2) {
            ActivateWire(color1);
        }

        return red.activeInHierarchy && green.activeInHierarchy && blue.activeInHierarchy;
    }

    private void ActivateWire(Color c) {
        if (c == Color.red) {
            red.SetActive(true);
        } else if (c == Color.green) {
            green.SetActive(true);
        } else if (c == Color.blue) {
            blue.SetActive(true);
        } else {
            ResetPanel();
        }
    }

    public void ResetPanel() {
        red.SetActive(false);
        green.SetActive(false);
        blue.SetActive(false);
    }
}
