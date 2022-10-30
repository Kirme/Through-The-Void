using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class healthBar : MonoBehaviour
{   
    private float interpolation_speed = 0.5f;
    [SerializeField] private Image hullSprite;
    [SerializeField] private GameObject hullText;
    private float hull_current = 1.0f;
    private float hull_target = 1.0f;

    [SerializeField] private Image faultSprite;
    [SerializeField] private GameObject faultText;
    private float fault_current = 1.0f;
    private float fault_target = 1.0f;

    public void setHullBar(float percent){
        hull_target = percent;
        String new_text = String.Format("Hull Integrity:\n{0}%", (int)(percent*100));
        hullText.GetComponent<TMPro.TextMeshPro>().text = new_text;
    }
    
    public void setFaultBar(int faults){
        fault_target = 1.0f - ((float)faults)/5;
        String new_text = String.Format("Faults:\n({0}/5)", faults);
        faultText.GetComponent<TMPro.TextMeshPro>().text = new_text;
    }

    void Start(){
       // Just for testing:
       // setHullBar(0.15f);
    }

   void Update(){
        // Adds sliding interpolation of the health bars:
        hull_current = Mathf.MoveTowards(hull_current, hull_target, interpolation_speed*Time.deltaTime);
        fault_current = Mathf.MoveTowards(fault_current, fault_target, interpolation_speed*Time.deltaTime);
        faultSprite.fillAmount = fault_current;
        hullSprite.fillAmount = hull_current;
    }

}
