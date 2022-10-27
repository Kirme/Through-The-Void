using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPanel : MonoBehaviour
{
    private bool holding = false;

    public bool IsSolved(Transform part, Touch touch) {
        if (!holding && part.name == "redButton") {
            part.localPosition = new Vector3(part.localPosition.x, part.localPosition.y, part.localPosition.z - 0.3f);
            holding = true;
        }
        
        if (touch.phase == TouchPhase.Ended) {
            ResetPanel(part);

            return true;
        }

        return false;
    }

    public void ResetPanel(Transform part) {
        if (holding) {
            part.localPosition = new Vector3(part.localPosition.x, part.localPosition.y, part.localPosition.z + 0.3f);
            holding = false;
        }
    }
}
