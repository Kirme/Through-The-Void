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
        else if (holding && part.name != "redButton" || touch.phase == TouchPhase.Ended) {
            ResetPanel(part);

            return touch.phase == TouchPhase.Ended;
        }

        return false;
    }

    public void ResetPanel(Transform part) {
        part.localPosition = new Vector3(part.localPosition.x, part.localPosition.y, part.localPosition.z + 0.3f);
        holding = false;
    }
}
