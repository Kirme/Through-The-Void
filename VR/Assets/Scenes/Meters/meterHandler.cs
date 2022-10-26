/*  meter handler
    An object that takes in the four meters in play and defines public methods for changing them accordingly.
    Just load in the four meters as GameObjects and they can all be accessed centrally from here! 
*/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class meterHandler : MonoBehaviour
{   
    public enum SHIP_PART
    {
        ENGINE,
        LEFT_WING,
        RIGHT_WING,
        CORE,
        NONE,
    }

    public GameObject meterTopLabel;
    public GameObject air_pressure;
    public GameObject fuel;
    public GameObject oxygen_level;
    public GameObject electricity;
    public float pendle_speed = 10f;
    public float restore_speed = 10;

    private SHIP_PART currentPart = SHIP_PART.NONE;

    private List<Fault> faults = new List<Fault>();

    private float timer = 1.0f;

    private string GetShipPartName(SHIP_PART part)
    {
        switch (part)
        {
            case SHIP_PART.ENGINE:
                return "Engine";
            case SHIP_PART.CORE:
                return "Core";
            case SHIP_PART.LEFT_WING:
                return "Left Wing";
            case SHIP_PART.RIGHT_WING:
                return "Right Wing";
            default:
                return "Core";
        }
    }

    private SHIP_PART GetShipPartEnum(string part)
    {
        switch (part)
        {
            case "engine":
                return SHIP_PART.ENGINE;
            case "core":
                return SHIP_PART.CORE;
            case "lwing":
                return SHIP_PART.LEFT_WING;
            case "rwing":
                return SHIP_PART.RIGHT_WING;
            default:
                Debug.LogWarning("Incorrect ship part in faults: " + part);
                return SHIP_PART.CORE;
        }
    }

    public void SetKnobValue(float val)
    {

        SHIP_PART prevPart = currentPart;

        if(val < 0.125f || val >= 0.875f)
        {
            //Core
            currentPart = SHIP_PART.CORE;
        } else if(val >= 0.125 && val < 0.375f)
        {
            //Right Wing
            currentPart = SHIP_PART.RIGHT_WING;

        } else if (val >= 0.375f && val < 0.625f)
        {
            //Engine 
            currentPart = SHIP_PART.ENGINE;

        } else
        {
            //Left Wing
            currentPart = SHIP_PART.LEFT_WING;

        }

        if (currentPart != prevPart)
        {
            meterTopLabel.GetComponent<TextMeshPro>().text = GetShipPartName(currentPart);
            SetMeters();
        }
    }

    public void handle_break(Fault fault, int numFaults){

        faults.Add(fault);
        SetMeters();
    }

    public void FixFault(Fault fault)
    {

        faults.Remove(fault);
        SetMeters();
    }

    public void FixAllFaults()
    {

        faults.Clear();
    }

    public void SetMeters()
    {
        bool fastInterp = false;
        foreach (Fault fault in faults) { 
            Dictionary<string, float[]> metrics = fault.GetMetrics();

            foreach (var item in metrics)
            {
                string key = item.Key;
                float[] value = item.Value;
                string[] split = key.Split("_");
                SHIP_PART location = GetShipPartEnum(split[0]);
                string meter = split[1];

                if(location != currentPart)
                {
                    continue;
                }

                if (meter == "pressure")
                {
                    /*float val = air_pressure.GetComponent<meter>().get_value();
                    if (false)//val < value[0] || val > value[1])
                    {
                        float targetVal = value[0] + (value[1] - value[0]) / 2;

                        interpolate_air_pressure(targetVal, 50f);
                        fastInterp = true;
                    } else {*/
                        start_pendle_air_pressure(value[0], value[1], pendle_speed);
                    //}

                }


                else if (meter == "fuel")
                {
                    /*float val = fuel.GetComponent<meter>().get_value();
                    if (false)//val < value[0] || val > value[1])
                    {
                        float targetVal = value[0] + (value[1] - value[0]) / 2;

                        interpolate_fuel(targetVal, 50f);
                        fastInterp = true;
                    }
                    else
                    {*/
                        start_pendle_fuel(value[0], value[1], pendle_speed);
                    //}
                }


                else if (meter == "oxygen")
                {

                    /*float val = oxygen_level.GetComponent<meter>().get_value();
                    if (false)//val < value[0] || val > value[1])
                    {
                        float targetVal = value[0] + (value[1] - value[0]) / 2;

                        interpolate_oxygen(targetVal, 50f);
                        fastInterp = true;
                    }
                    else
                    {*/
                        start_pendle_oxygen(value[0], value[1], pendle_speed);
                    //}
                }


                else if (meter == "electricity")
                {

                    /*float val = electricity.GetComponent<meter>().get_value();
                    if (false)//val < value[0] || val > value[1])
                    {
                        float targetVal = value[0] + (value[1] - value[0]) / 2;

                        interpolate_electricity(targetVal, 50f);
                        fastInterp = true;
                    }
                    else
                    {*/
                        start_pendle_electricity(value[0], value[1], pendle_speed);
                    //}
                }
            }
        }
        if (fastInterp)
        {
            timer = 1.0f;
        }
    }

    void Update()
    {
        if(timer > 0.0f)
        {
            timer -= Time.deltaTime;

            if(timer <= 0.0f)
            {
                SetMeters();
            }
        }
    }

    // Interpolate & Setters for the meters:
    public void set_air_pressure(float percentage){
        air_pressure.GetComponent<meter>().set_value(percentage);
    }

    public void start_pendle_air_pressure(float min, float max, float speed){
        air_pressure.GetComponent<meter>().start_pendle(min, max, speed);
    }
    
    public void set_fuel(float percentage){
        fuel.GetComponent<meter>().set_value(percentage);
    }

    public void start_pendle_fuel(float min, float max, float speed){
        fuel.GetComponent<meter>().start_pendle(min, max, speed);
    }

    public void set_oxygen(float percentage){
        oxygen_level.GetComponent<meter>().set_value(percentage);
    }

    public void start_pendle_oxygen(float min, float max, float speed){
        oxygen_level.GetComponent<meter>().start_pendle(min, max, speed);
    }

    public void set_electricity(float percentage){
        electricity.GetComponent<meter>().set_value(percentage);
    }

    public void start_pendle_electricity(float min, float max, float speed){
        electricity.GetComponent<meter>().start_pendle(min, max, speed);
    }

    // Restore all the meters to 100%, to be called when all is fixed:
    public void restore_all_to_max()
    {
        /*air_pressure.GetComponent<meter>().interpolate_value(1.0f, restore_speed);
        electricity.GetComponent<meter>().interpolate_value(1.0f, restore_speed);
        fuel.GetComponent<meter>().interpolate_value(1.0f, restore_speed);
        oxygen_level.GetComponent<meter>().interpolate_value(1.0f, restore_speed);
        */
    }

    // Start is called before the first frame update
    void Start()
    {


        start_pendle_air_pressure(Random.Range(0.1f, 1.0f), 1.0f, pendle_speed);
        start_pendle_oxygen(Random.Range(0.1f, 1.0f), 1.0f, pendle_speed);
        start_pendle_fuel(Random.Range(0.1f, 1.0f), 1.0f, pendle_speed);
        start_pendle_electricity(Random.Range(0.1f, 1.0f), 1.0f, pendle_speed);
        
        start_pendle_air_pressure(0.0f, 1.0f, 1f);

        SetKnobValue(0.0f);
        // TESTING:
        /*
        set_air_pressure(0.0f);
        interpolate_air_pressure(1.0f, 10f);
        start_pendle_air_pressure(0.6f, 0.8f, 4f);
        

        set_fuel(0.0f);
        interpolate_fuel(1.0f, 10f);
        */
    }
}
