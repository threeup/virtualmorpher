using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CamCtrl : MonoBehaviour, IMotor {

    public static CamCtrl First;
    public static CamCtrl Second;
    
    public Transform keyInterest = null;
    public List<Transform> pointsOfInterest = new List<Transform>();
    public Actor cursor;
    public Actor selection;
    public Camera cam; 
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
    
    
    
    public GameObject hudcanvas;
    public GameObject floaterPrototype;
    List<Floater> floaters = new List<Floater>();
    
    float selectionExpireTimer = -1f;
    
	// Use this for initialization
	void Awake () {
	    cam = this.GetComponent<Camera>();
        if( First == null )
        {
            First = this;
        }
        else
        {
            Second = this;
        }
        for(int i=0; i<8;++i)
        {
            GameObject go = GameObject.Instantiate(floaterPrototype);
            go.name = "Floater"+i;
            go.transform.SetParent(hudcanvas.transform);
            go.SetActive(false);
            Floater floater = go.GetComponent<Floater>();
            floater.camCtrl = this;
            floaters.Add(floater);
        }
        hudcanvas.gameObject.SetActive(true);
	}
    
    void Start()
    {
        cursor = ActorWorld.Ins.CreateItem(ActorWorld.Ins.cursorPrototype, null);
        selection = ActorWorld.Ins.CreateItem(ActorWorld.Ins.selectionPrototype, null);
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
        Rect screenRect = new Rect(0,0, Screen.width, Screen.height);
        if (screenRect.Contains(Input.mousePosition))
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);   
            if (plane.Raycast(ray, out distance))
            {
                cursor.transform.position = ray.GetPoint(distance);
            }
        }
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
        if( selectionExpireTimer > 0 )
        {
            selectionExpireTimer -= Time.deltaTime;
            if( selectionExpireTimer <= 0f )
            {
                selection.body.gameObject.SetActive(false);
                HideFloater(selection.transform);
            }
        }
	}
    
    public void ShowFloater(Transform target, int line, string text, Action<bool> onClick)
    {
        Floater nextFloater = null;
        for(int i=0; i<floaters.Count; ++i)
        {
            Floater floater = floaters[i];
            if(floater.target == target && floater.line == line)
            {
                nextFloater = floater;
                break;
            }
            if(nextFloater == null && floater.target == null)
            {
                nextFloater = floater;
            }
        }
        if( nextFloater != null )
        {
            nextFloater.gameObject.SetActive(true);
            nextFloater.target = target;
            nextFloater.line = line;
            nextFloater.textField.text = text;
            if( onClick != null )
            {
                nextFloater.onClick = onClick;
            }
        }
    }
    
    public void HideFloater(Transform target, int line)
    {
        foreach(Floater floater in floaters)
        {
            if(floater.target == target && floater.line == line)
            {
                floater.Hide();
            }
        }
    }
    public void HideFloater(Transform target)
    {
        foreach(Floater floater in floaters)
        {
            if(floater.target == target)
            {
                floater.Hide();
            }
        }
    }
    public void ClickFloater(Transform target)
    {
        foreach(Floater floater in floaters)
        {
            if(floater.target == target)
            {
                floater.Click();
            }
        }
    }
    
    public void ClickHeld(Vector3 position)
    {
        selection.SetDestination(position);
        selectionExpireTimer = 1f;
    }
    
    public void ClickDown(Vector3 position)
    {
        bool clickOnSelection = (position - selection.transform.position).sqrMagnitude < 1.2f*1.2f;
        float mag = (position - selection.transform.position).magnitude;
        if( clickOnSelection )
        {   
            ClickFloater(selection.transform);
        }
        selection.SetDestination(position);
        selection.body.gameObject.SetActive(true);
    }
    
    public void ClickUp(Vector3 position)
    {
       
    }
}
