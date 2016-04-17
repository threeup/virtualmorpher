using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CamCtrl : MonoBehaviour, IMotor {

    
    public Transform keyInterest = null;
    public List<Transform> pointsOfInterest = new List<Transform>();
    public Camera cam; 
    public UICtrl uiCtrl; 
    public Plane plane = new Plane(Vector3.up, Vector3.zero);
    
    float motorSpeed = 0f;
    public float GetSpeed() { return motorSpeed; }
    Vector3 motorDirection = Vector3.forward;
    public Vector3 GetDirection() { return motorDirection; }
    
    Vector3 currentCenter = Vector3.zero;
    Vector3 averageInterest;
    
    float currentZoomLevel = 10;
    public float zoomPadding = 4f;
    public float minZoom = 8f;
    public float maxZoom = 15f;
    
	// Use this for initialization
	void Awake () {
	    cam = this.GetComponent<Camera>();
	    uiCtrl = this.GetComponent<UICtrl>();
        Boss.camCtrls.Add(this);
	}
    
    void Start()
    {
        MotorWorld.Ins.Add(this);
    }
    
    void OnEnable()
    {
        if( MotorWorld.Ins != null && !MotorWorld.Ins.Contains(this))
        {
            MotorWorld.Ins.Add(this);
        }
    }
    
    void OnDisable()
    {
        MotorWorld.Ins.Remove(this);
    }
    
    void ScanInterest()
    {
        Ray ray;
        float distance = 0; 
        Vector2 midPoint = new Vector2(cam.pixelWidth/2,cam.pixelHeight/2);
        ray = cam.ScreenPointToRay(midPoint);
        if( plane.Raycast(ray, out distance) )
        {
            currentCenter = ray.GetPoint(distance);
        }
        averageInterest = Vector3.zero;
	    foreach(Transform point in pointsOfInterest)
        {
            Vector3 interestingPosition = point.position;
            interestingPosition.y = 0;
            averageInterest += interestingPosition;
        }
        averageInterest /= pointsOfInterest.Count;
        float maxDistToInterest = 0f; 
        foreach(Transform point in pointsOfInterest)
        {
            float sqrMag = (point.position - averageInterest).sqrMagnitude;
            if( sqrMag > maxDistToInterest * maxDistToInterest )
            {
                maxDistToInterest = Mathf.Sqrt(sqrMag);
            }
        }
        Vector3 velocity = Vector3.zero;
        if( keyInterest == null )
        {
            velocity = averageInterest - currentCenter;
        }
        else
        {
            velocity = keyInterest.position - currentCenter;
        }
        float desiredZoomLevel = Mathf.Clamp( maxDistToInterest, minZoom, maxZoom);
        velocity.y = desiredZoomLevel - currentZoomLevel;
        SetRelativeDestination(velocity);
    }
    
    public void SetRelativeDestination(Vector3 amount)
    {
        float minDist = 6f;
        float magn = amount.magnitude;
        if( magn < minDist )
        {
            motorSpeed = 0;
            motorDirection = Vector3.zero;
        }
        else
        {
            motorSpeed = Mathf.Min(magn, 20);
            motorDirection = amount.normalized;    
        }
        
    }
    
    public void UpdateMotion(float deltaTime)
    {
        Vector3 nextPosition = this.transform.position + GetDirection()*GetSpeed()*deltaTime;
        this.transform.position = nextPosition;
        currentZoomLevel = this.transform.position.y - zoomPadding; 
    }
	
	// Update is called once per frame
	void Update () {
        ScanInterest();
	}
    
    
    
}
