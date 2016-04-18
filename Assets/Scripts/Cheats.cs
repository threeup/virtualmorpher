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
            slider.value += 0.1f*scrollAxis/1000f;
        }
    }  
    
    public void RotateCamera()
    {
        Camera.main.transform.RotateAround(Vector3.zero, Vector3.up, 45f);
    }
    
    public void SetZoom(float factor)
    {
        CamCtrl camctrl = Camera.main.GetComponent<CamCtrl>();
        camctrl.zoomPadding = 4f*(0.3f+factor*2);
        camctrl.minZoom = 8f*(0.1f+factor*2);
        camctrl.maxZoom = 15f*(0.1f+factor*2);
        
    }
    
    public void SetDouble(bool val)
    {
        FloatIcon[] fis = FindObjectsOfType(typeof(FloatIcon)) as FloatIcon[];
        foreach(FloatIcon fi in fis)
        {
            fi.SetDoubleScale( val );
        }
    }
}
