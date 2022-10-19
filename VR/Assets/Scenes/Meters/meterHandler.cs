/*  meter handler
    An object that takes in the four meters in play and defines public methods for changing them accordingly.
    Just load in the four meters as GameObjects and they can all be accessed centrally from here! 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meterHandler : MonoBehaviour
{   
    public GameObject air_pressure;
    public GameObject fuel;
    public GameObject oxygen_level;
    public GameObject electricity;
    public float pendle_speed = 10f;

    public void handle_break(Fault fault, int numFaults){
        Dictionary<string, float[]> metrics = fault.GetMetrics();

        foreach(var item in metrics){
            string key = item.Key;
            float[] value = item.Value;
            string[] split = key.Split("_");
            string location = split[0];
            string meter = split[1];

            if(meter == "pressure"){
                start_pendle_air_pressure(value[0], value[1], pendle_speed);
            }

            
            else if(meter == "fuelCirculation"){
                start_pendle_fuel(value[0], value[1], pendle_speed);
            }

            
            else if(meter == "oxygen"){
                start_pendle_oxygen(value[0], value[1], pendle_speed);
            }

            
            else if(meter == "electricity"){
                start_pendle_electricity(value[0], value[1], pendle_speed);
            }
        }
    }

    // Interpolate & Setters for the meters:
    public void interpolate_air_pressure(float percentage, float speed){
        air_pressure.GetComponent<meter>().interpolate_value(percentage, speed);
    }

    public void set_air_pressure(float percentage){
        air_pressure.GetComponent<meter>().set_value(percentage);
    }

    public void start_pendle_air_pressure(float min, float max, float speed){
        air_pressure.GetComponent<meter>().start_pendle(min, max, speed);
    }

    public void interpolate_fuel(float percentage, float speed){
        fuel.GetComponent<meter>().interpolate_value(percentage, speed);
    }
    
    public void set_fuel(float percentage){
        fuel.GetComponent<meter>().set_value(percentage);
    }

    public void start_pendle_fuel(float min, float max, float speed){
        fuel.GetComponent<meter>().start_pendle(min, max, speed);
    }

    public void interpolate_oxygen(float percentage, float speed){
        oxygen_level.GetComponent<meter>().interpolate_value(percentage, speed);
    }

    public void set_oxygen(float percentage){
        oxygen_level.GetComponent<meter>().set_value(percentage);
    }

    public void start_pendle_oxygen(float min, float max, float speed){
        oxygen_level.GetComponent<meter>().start_pendle(min, max, speed);
    }

    public void interpolate_electricity(float percentage, float speed){
        electricity.GetComponent<meter>().interpolate_value(percentage, speed);
    }

    public void set_electricity(float percentage){
        electricity.GetComponent<meter>().set_value(percentage);
    }

    public void start_pendle_electricity(float min, float max, float speed){
        electricity.GetComponent<meter>().start_pendle(min, max, speed);
    }

    // Start is called before the first frame update
    void Start()
    {   

        // TESTING:
        /*
        set_air_pressure(0.0f);
        interpolate_air_pressure(1.0f, 10f);
        start_pendle_air_pressure(0.6f, 0.8f, 4f);
        

        set_fuel(0.0f);
        interpolate_fuel(1.0f, 10f);
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
