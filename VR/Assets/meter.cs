/*  meter
    The meter class defines the behaviour of a meter. Each meter can be defined to meassure different quantities and their units, for example air pressure or fuel.
    The meter object can be changed outside by calling the meter's set_value(float percentage) function, which will make the meter walk towards that percentage value.
    The public quantities can be changed in the editor to create a specific meter for a specific task.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class meter : MonoBehaviour
{   
    // Public members:
    public string name = "meter";               // name:      Defines the name at the top of the meter object.
    public string unit = "bar";                 // unit:      Defines the unit meassured in max and min.
    public float min_val = 0f;                  // min_val:   Defines the min value meassured in the meter.
    public float max_val = 100f;                // max_val:   Defines the max value meassured in the meter.
    public float velocity = 1f;                 // velocity:  Defines the speed of change when changing the value.
    public float step_size = 0.1f;              // step_size: Defines how often we should take a step (smaller stepsize makes it look smoother).


    // Private members:
    private GameObject text_pro;                // text_pro:             Defines the object that defines the label with the variable "name".
    private GameObject min_text;                // min_text:             Defines the object that displays the text for the min value.
    private GameObject max_text;                // max_text:             Defines the object does the same for the max value.
    private Transform inner_disk_transform;     // inner_disk_transform: Defines the transform for the inner disk of the meter so we can rotate it later.  
    private float value = 0f;                   // value:                Defines the value the meter is on, a value between [min_val and max_val].
    private float angle = 0f;                   // angle:                Defines the current rotation of the inner disk (with the needle).


    private void Awake(){
        // Set start value to be 100%:
        value = max_val;

        // Get the inner disk:
        inner_disk_transform = transform.Find("Inner Cylinder");
        
        // Set the upper label:
        text_pro = transform.Find("Label").gameObject;
        text_pro.GetComponent<TMPro.TextMeshPro>().text = name;

        // Set the min value label:
        min_text = transform.Find("Min_Label").gameObject;
        min_text.GetComponent<TMPro.TextMeshPro>().text = min_val + "\n" + unit;

        // Set the max value label:
        max_text = transform.Find("Max_Label").gameObject;
        max_text.GetComponent<TMPro.TextMeshPro>().text = max_val + "\n" + unit;

        set_value(0.5f);

   }
    
    /*  set_value
        set_value takes in a percentage value that we want to set our meter to, and it will either increase or decrease towards that value with a step size and 
        velocity. The meter always start at the max value, 100% that is to say. If we then pass in set_value(0.5f), this entails that we are decreaseing the meter
        with said velocity and stepsize, until we reach the 50% mark. If our value was a value lower than 50% we would increase instead.

        TLDR; set_value makes the meter walk towards the given percentage value.  
    */

    public void set_value(float percent){
        float val = (max_val - min_val) * percent;
        if(value < val){
            increase_to(val);

        }
        else if(value > val){
            decrease_to(val);
        }
    }

    /*  set_angle
        Each time a change is made to the value, either by increase or decrease, the set_angle adjusts the rotation according to that value.
    */
    private void set_angle(){
        float disparity = (max_val - min_val);
        angle = value * (180f/disparity);
    }

    /*  decrease_to
        decrease_to is a helper function to set_value, that makes decrementation of the meter. We call upon a coroutine to decrement our value until we reach
        the target value, all while updating the angle via the helper function set_angle().
    */

    private void decrease_to(float stop_val){
        StartCoroutine(decrease_to_coroutine(stop_val));
    }

    private IEnumerator decrease_to_coroutine(float stop_val){
        yield return new WaitForSeconds(step_size);
        if(value > stop_val){
            value -= step_size*velocity;
            set_angle();
            StartCoroutine(decrease_to_coroutine(stop_val));
        }

    }

    
    /*  increase_to
        increase_to is a helper function to set_value, that makes decrementation of the meter. We call upon a coroutine to increment our value until we reach
        the target value, all while updating the angle via the helper function set_angle().
    */

    private void increase_to(float stop_val){
        StartCoroutine(increase_to_coroutine(stop_val));
    }

    private IEnumerator increase_to_coroutine(float stop_val){
        yield return new WaitForSeconds(step_size);
        if(value < stop_val){
            value += step_size*velocity;
            set_angle();
            StartCoroutine(increase_to_coroutine(stop_val));
        }

    }

    // Updates the rotation:
    void Update()
    {   
        inner_disk_transform.localRotation = Quaternion.Euler(inner_disk_transform.localRotation.x, inner_disk_transform.localRotation.y - angle, inner_disk_transform.localRotation.z);
    }
}
