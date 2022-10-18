using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMetrics : MonoBehaviour
{   
    private float airPressure = 0.85f;
    private float oxygenLevel = 1.0f;
    [SerializeField] GameObject server_obj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void sendAirPressure(){
        server_obj.GetComponent<SocketServer>().SendData("Air-Pressure " + airPressure);
    }

    void setAirPressure(float pressure){
        airPressure = pressure;
    }

    void sendOxygenLevel(){
        server_obj.GetComponent<SocketServer>().SendData("Oxygen-Level " + oxygenLevel);
    }

    void setOxygenLevel(float oxygen){
        oxygenLevel = oxygen;
    }

    // Update is called once per frame
    void Update()
    {
        sendAirPressure();
    }
}
