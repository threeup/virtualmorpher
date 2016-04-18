using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Cheats : MonoBehaviour
{
    public Slider slider;
    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            RotateCamera();    
        }
        float scrollAxis = Input.GetAxis("Mouse ScrollWheel");
        if( Mathf.Abs(scrollAxis) > 0.1 )
        {
            slider.value -= 0.03f*scrollAxis/1000f;
        }
    }  
    
    public void RotateCamera()
    {
        Camera.main.transform.RotateAround(Vector3.zero, Vector3.up, 45f);
    }
    
    public void SetZoom(float factor)
    {
        CamCtrl camctrl = Camera.main.GetComponent<CamCtrl>();
        camctrl.zoomPadding = 4f*(0.6f+factor*1.5f);
        camctrl.minZoom = 8f*(0.3f+factor*1.5f);
        camctrl.maxZoom = 15f*(0.3f+factor*1.5f);
        
    }
    
    public void SetGameSpeed(float factor)
    {
        Boss.referee.gameSpeed = 0.75f+Mathf.Round(factor*4)/4;
        
    }
    
    /*public void SetDouble(bool val)
    {
        FloatIcon[] fis = FindObjectsOfType(typeof(FloatIcon)) as FloatIcon[];
        foreach(FloatIcon fi in fis)
        {
            fi.SetDoubleScale( val );
        }
    }*/
    public void SetDouble(bool val)
    {
        Pawn[] ps = FindObjectsOfType(typeof(Pawn)) as Pawn[];
        foreach(Pawn p in ps)
        {
            p.motor.defaultTopSpeed = val ? 3.5f : 2f;
            p.motor.currentTopSpeed = p.motor.defaultTopSpeed;
        }
    }
}
