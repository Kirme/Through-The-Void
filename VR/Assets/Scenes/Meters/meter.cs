/*  meter
    The meter class defines the behaviour of a meter. Each meter can be defined to meassure different quantities and their units, for example air pressure or fuel.
    The meter object can be changed outside by calling the meter's set_value(float percentage) function, which will make the meter walk towards that percentage value.
    The public quantities can be changed in the editor to create a specific meter for a specific task.

    TLDR; Just use set_value(percentage) from the outside to make the meters walk towards a percentage and get_value() to extract the value at any time.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class meter : MonoBehaviour
{   
    // PUBLIC MEMBERS:
    public string name = "meter";               // name:      Defines the name at the top of the meter object.
    public string unit = "bar";                 // unit:      Defines the unit meassured in max and min.
    public float min_val = 0f;                  // min_val:   Defines the min value meassured in the meter.
    public float max_val = 100f;                // max_val:   Defines the max value meassured in the meter.


    // PRIVATE MEMBERS:
    private float velocity = 1f;                // velocity:             Defines the speed of change when changing the value.
    private float step_size = 0.01f;            // step_size:            Defines how often we should take a step (smaller stepsize makes it look smoother).
    
    // Text display objects:
    private GameObject text_pro;                // text_pro:             Defines the object that defines the label with the variable "name".
    private GameObject min_text;                // min_text:             Defines the object that displays the text for the min value.
    private GameObject half_text;               // half_text:            Defines the object that displays the half value mark.
    private GameObject max_text;                // max_text:             Defines the object does the same for the max value.
    
    // Data values:
    private Transform inner_disk_transform;     // inner_disk_transform: Defines the transform for the inner disk of the meter so we can rotate it later.  
    private float value = 0f;                   // value:                Defines the value the meter is on, a value between [min_val and max_val].
    private float angle = 0f;                   // angle:                Defines the current rotation of the inner disk (with the needle).
    private float target_value = 0f;            // target_value:         Defines the next value to interpolate towards.
    
    // Pendling:
    private bool pendling = false;              // pendling:    A flag to see if we should do pendling or not.
    private float max_pendle;                   // max_pendle:  Defines a maximum point in the pendle movement.
    private float min_pendle;                   // min_pendle:  Defines a minimum point in the pendle movement.
    private float pendle_velocity; 

    private void Awake(){
        // Set start value to be 100%:
        set_value(1.0f);
        
        // Set a start for pendling values:
        max_pendle = max_val;
        min_pendle = min_val;

        // Get the inner disk:
        inner_disk_transform = transform.Find("Inner Cylinder");
        
        // Set the upper label:
        text_pro = transform.Find("Label").gameObject;
        text_pro.GetComponent<TMPro.TextMeshPro>().text = name;

        // Set the min value label:
        min_text = transform.Find("Min_Label").gameObject;
        min_text.GetComponent<TMPro.TextMeshPro>().text = min_val + "";

        // Half Value Mark labels:
        half_text = transform.Find("Half_Val").gameObject;
        int average_val = (int)((max_val + min_val)/2);
        half_text.GetComponent<TMPro.TextMeshPro>().text = average_val + "";

        // Set the max value label:
        max_text = transform.Find("Max_Label").gameObject;
        max_text.GetComponent<TMPro.TextMeshPro>().text = max_val + "";

   }

    // A setter for the meter:
    public void set_value(float percent){
        value = (max_val - min_val) * percent;
        target_value = value;
    }

    // A getter for the meter:
    public float get_value(){
        return value;
    }
    

    // Set a new target value and walk towards it:
    public void interpolate_value(float percent, float speed){
        target_value = (max_val - min_val) * percent;
        velocity = speed;
        pendling = false;
    }

    // Start pendle sets a new interval for pendling and sets a new speed (preferably slow):
    public void start_pendle(float min, float max, float speed){
        velocity = speed;
        pendle_velocity = speed;
        max_pendle = (max_val-min_val) * max;
        min_pendle = (max_val-min_val) * min;
        target_value = max_pendle;
        pendling = true;
    }

    // Pendle performs a pendle movement by setting the target value, depending on the value, so that
    // we work within an interval provided to us as [min, max].

    private void pendle(){
        if(value <= min_pendle){
            target_value = max_pendle;
        }
        else if (value >= max_pendle){
            target_value = min_pendle;
        }
    }

    private void set_angle(){
        float disparity = (max_val - min_val);
        angle = value * (180f/disparity);
    }

    // Updates the rotation:
    void Update()
    {   

        Debug.Log(value + " " + target_value);
        // Normalize: 1. Per frame and 2. with regards to max/min value:
        float normalized_step = velocity * (max_val - min_val)/100 * Time.deltaTime;
        if(target_value < value){
            value -= normalized_step;
        }
        else if(target_value > value){
            value += normalized_step;
        }

        // 2. Should we start to pendle between two values again if we reach the interpolation? 
        if(!pendling && value == target_value){
            //pendling = true;
            //velocity = pendle_velocity;
        }

        // 3. If the pendling flag is set to be true, pendle between our values:
        if(pendling){
            pendle();
        }

        // 4. Update the angle of the inner disk:
        inner_disk_transform.localRotation = Quaternion.Euler(inner_disk_transform.localRotation.x, inner_disk_transform.localRotation.y - angle, inner_disk_transform.localRotation.z);
        set_angle();
    }
}
