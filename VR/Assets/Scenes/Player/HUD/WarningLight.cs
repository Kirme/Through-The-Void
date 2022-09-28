using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class WarningLight : MonoBehaviour
{

    public float speed = 0.1f, maxIntensity = 1.5f, minIntensity = 0.5f;
    private float time = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsTrue(maxIntensity >= minIntensity, "maxIntensity must be larger or equal to minIntensity");   
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        GetComponent<Light>().intensity = (Mathf.Sin(time*speed) + 1)/2*(maxIntensity - minIntensity) + minIntensity;
    }
}
