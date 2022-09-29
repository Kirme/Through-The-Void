using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DestinationDisplay : MonoBehaviour
{
    public GameObject destination, arrow, text;
    public UnityEvent onDestinationReached;

    void Update()
    {
        if (destination != null)
        {
            float distance = (destination.transform.position - transform.position).magnitude;
            if (distance < 100)
            {
                onDestinationReached.Invoke();
            }

            if(arrow != null)
            {
                arrow.transform.LookAt(destination.transform.position);
            }
            if(text != null)
            {
                string str = "";
                if (distance > 1000f)
                {
                    str = string.Format("{0:0.0}km", (distance)/1000);
                } else
                {
                    str = string.Format("{0:0}m", ((int)distance)/10*10);
                }
                text.GetComponent<TextMesh>().text = str;
            }
        }
    }
}
