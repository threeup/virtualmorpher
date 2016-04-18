using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CamCtrl : MonoBehaviour, IMotor {

    
    public Transform keyInterest = null;
    public List<Transform> pointsOfInterest = new List<Transform>();
    public List<Vector3> posOfInterest = new List<Vector3>();
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
        Boss.Add(this);
    }
    
    void OnEnable()
    {
        Boss.Add(this);
    }
    
    void OnDisable()
    {
        Boss.Remove(this);
    }
    
    void ScanInterest()
    {
        Ray ray;
        float distance = 0; 
        Vector2 camPoint = new Vector2(cam.pixelWidth/2,cam.pixelHeight/2);
        ray = cam.ScreenPointToRay(camPoint);
        if( plane.Raycast(ray, out distance) )
        {
            currentCenter = ray.GetPoint(distance);
        }
        camPoint = new Vector2(0,cam.pixelHeight/2);
        ray = cam.ScreenPointToRay(camPoint);
        Vector3 worldLeft = Vector3.zero;
        if( plane.Raycast(ray, out distance) )
        {
            worldLeft = ray.GetPoint(distance);
        }
        camPoint = new Vector2(cam.pixelWidth,cam.pixelHeight/2);
        ray = cam.ScreenPointToRay(camPoint);
        Vector3 worldRight = Vector3.zero;
        if( plane.Raycast(ray, out distance) )
        {
            worldRight = ray.GetPoint(distance);
        }
        camPoint = new Vector2(cam.pixelWidth/2,cam.pixelHeight);
        ray = cam.ScreenPointToRay(camPoint);
        Vector3 worldTop = Vector3.zero;
        if( plane.Raycast(ray, out distance) )
        {
            worldTop = ray.GetPoint(distance);
        }
        camPoint = new Vector2(cam.pixelWidth/2,0);
        ray = cam.ScreenPointToRay(camPoint);
        Vector3 worldBottom = Vector3.zero;
        if( plane.Raycast(ray, out distance) )
        {
            worldBottom = ray.GetPoint(distance);
        }

        
        averageInterest = Vector3.zero;
        posOfInterest.Clear();
        if( keyInterest )
        {
            posOfInterest.Add(keyInterest.position);
        }
        Rect screenRect = new Rect(0,0, Screen.width, Screen.height);
        if (screenRect.Contains(Input.mousePosition))
        {
            if( Input.mousePosition.y < cam.pixelHeight*0.1)
            {
                posOfInterest.Add(worldBottom);
            }
            if( Input.mousePosition.y > cam.pixelHeight*0.9)
            {
                posOfInterest.Add(worldTop);
            }
            if( Input.mousePosition.x < cam.pixelWidth*0.1)
            {
                posOfInterest.Add(worldLeft);
            }
            if( Input.mousePosition.x > cam.pixelWidth*0.9)
            {
                posOfInterest.Add(worldRight);
            }
        }
	    foreach(Vector3 point in posOfInterest)
        {
            Vector3 interestingPosition = point;
            interestingPosition.y = 0;
            averageInterest += interestingPosition;
        }
        averageInterest /= posOfInterest.Count;
        float maxDistToInterest = 0f; 
        foreach(Vector3 point in posOfInterest)
        {
            float sqrMag = (point - averageInterest).sqrMagnitude;
            if( sqrMag > maxDistToInterest * maxDistToInterest )
            {
                maxDistToInterest = Mathf.Sqrt(sqrMag);
            }
        }
        Vector3 velocity = averageInterest - currentCenter;
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
