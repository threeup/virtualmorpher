using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MotorWorld : MonoBehaviour {

    public List<IMotor> activeMotors = new List<IMotor>();

	void Awake() 
    {
	    Boss.motorWorld = this;
	}
    
    public void Add(IMotor motor)
    {
        activeMotors.Add(motor);
    }
    
    public void Remove(IMotor motor)
    {
        activeMotors.Remove(motor);
    }
    
    public bool Contains(IMotor motor)
    {
        return activeMotors.Contains(motor);
    }
    
	void Update () 
    {
        float deltaTime = Time.deltaTime;
        foreach(IMotor motor in activeMotors)
        {
            motor.UpdateMotion(deltaTime);
        }
	}
}
