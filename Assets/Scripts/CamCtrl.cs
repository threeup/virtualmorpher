using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CamCtrl : MonoBehaviour, IMotor {

    public static CamCtrl First;
    public static CamCtrl Second;
    
    public Transform keyInterest = null;
    public List<Transform> pointsOfInterest = new List<Transform>();
    public Actor cursor;
    public Actor selection;
    Camera localCam; 
    Plane plane = new Plane(Vector3.up, Vector3.zero);
    
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
	    localCam = this.GetComponent<Camera>();
        if( First == null )
        {
            First = this;
        }
        else
        {
            Second = this;
        }
	}
    
    void Start()
    {
        cursor = ActorWorld.Ins.CreateItem(ActorWorld.Ins.cursorPrototype, null);
        selection = ActorWorld.Ins.CreateItem(ActorWorld.Ins.cursorPrototype, null);
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
        Ray ray = localCam.ScreenPointToRay(Input.mousePosition);
        float distance = 0; 
        if (plane.Raycast(ray, out distance))
        {
             cursor.transform.position = ray.GetPoint(distance);
        }
        Vector2 midPoint = new Vector2(localCam.pixelWidth/2,localCam.pixelHeight/2);
        ray = localCam.ScreenPointToRay(midPoint);
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
        SetVelocity(velocity);
    }
    
    public void SetVelocity(Vector3 amount)
    {
        float minDist = 0.5f;
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
