using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DestinationDisplay : MonoBehaviour
{
    public GameObject destination, arrow, arrowMesh, text, waypointText;
    public UnityEvent onDestinationReached;
    public List<Color> colors = new List<Color>();
    private Gradient gradient;

    private int numWaypointsReached = 0;
    public UnityEvent<int> onWaypointReached;

    private void Start()
    {
        gradient = new Gradient();
        GradientColorKey[] key = new GradientColorKey[colors.Count];
        int i = 0;
        foreach(Color color in colors)
        {
            key[i].color = color;
            key[i].time = ((float)i)/(colors.Count - 1.0f);
            i++;
        }
        int numWaypoints = destination.GetComponent<DestinationPoint>().numWaypoints;
        waypointText.GetComponent<TextMesh>().text = "Waypoint " + (numWaypointsReached + 1) + "/" + numWaypoints;
        gradient.SetKeys(key, new GradientAlphaKey[colors.Count]);
    }

    void Update()
    {
        if (destination != null)
        {
            float distance = (destination.transform.position - transform.position).magnitude;
            if (distance < 100)
            {
                numWaypointsReached += 1;
                int numWaypoints = destination.GetComponent<DestinationPoint>().numWaypoints;
                if (numWaypointsReached < numWaypoints)
                {
                    onWaypointReached.Invoke(numWaypointsReached);
                } else
                {
                    onWaypointReached.Invoke(numWaypointsReached);
                    onDestinationReached.Invoke();
                    numWaypointsReached = 0;
                }

                if(waypointText != null)
                {
                    waypointText.GetComponent<TextMesh>().text = "Waypoint " + (numWaypointsReached + 1) + "/" + numWaypoints;
                }
            }

            if(arrow != null)
            {
                arrow.transform.LookAt(destination.transform.position);
                Vector3 angles = arrow.transform.localRotation.eulerAngles;

                if (angles.x > 180)
                {
                    angles.x = 360 - angles.x;
                }
                if (angles.y > 180)
                {
                    angles.y = 360 - angles.y;
                }

                float maxAngle = Mathf.Max(angles.x, angles.y);
                arrowMesh.GetComponent<MeshRenderer>().material.color = gradient.Evaluate(maxAngle / 180.0f);
                arrowMesh.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", gradient.Evaluate(maxAngle / 180.0f));
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
