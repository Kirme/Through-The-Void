using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel : MonoBehaviour {
    private bool m_canRepair;
    private float m_timeToHold;
    private string m_fault;
    
    public string fault {
        get { return m_fault; }
        set { m_fault = value; }
    }

    public bool canRepair {
        get { return m_canRepair; }
        set { m_canRepair = value; }
    }

    public float timeToHold {
        get { return m_timeToHold; }
        set { m_timeToHold = value; }
    }

    // Get name of ship part panel is attached to
    public string GetPartName() {
        return transform.parent.name;
    }

    // Get  name of panel
    public string GetPanelName() {
        return transform.name;
    }
}
