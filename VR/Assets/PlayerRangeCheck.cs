using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRangeCheck : MonoBehaviour
{
    public Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(playerTransform.localPosition.x);
    }
}
