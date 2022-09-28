using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationDisplay : MonoBehaviour
{
    public GameObject destination, arrow, text;

    void Update()
    {
        if (destination != null)
        {
            if(arrow != null)
            {
                arrow.transform.LookAt(destination.transform.position);
            }
            if(text != null)
            {
                float distance = (destination.transform.position - transform.position).magnitude;

                string str = "";
                if (distance > 1000f)
                {
                    str = string.Format("{0:0.0}km", (distance)/1000);
                } else
                {
                    str = string.Format("{0:0.0}m", distance);
                }
                text.GetComponent<TextMesh>().text = str;
            }
        }
    }
}
