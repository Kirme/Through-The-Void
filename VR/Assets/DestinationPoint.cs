using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationPoint : MonoBehaviour
{
    public int numWaypoints = 5;
    public GameObject player;
    public int distance = 4250;
    public void RandomizePosition()
    {
        transform.position = player.transform.position + Random.insideUnitSphere.normalized * distance;
    }
}
