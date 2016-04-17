using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

public class UICtrl : MonoBehaviour 
{

    CamCtrl camCtrl;
    public Camera cam;
    public Actor stickCursor;
    public Actor activeCursor;
    public Actor floorCursor;
    
    public GameObject hudcanvas;
    public GameObject floaterPrototype;
    public GameObject stickCursorPrototype;
    public GameObject activeCursorPrototype;
    public GameObject floorCursorPrototype;
    List<Floater> floaters = new List<Floater>();
    
    float floorCursorExpireTimer = -1f;
    
    public bool editPathMode = false;
    List<IntVec2> activePath;
    Transform activeTransform;
    Pawn activePawn;
    
    List<VectorLine> allPathLines = new List<VectorLine>();
    public Texture lineTex;
    public float lineWidth = 20f;
    
    VectorLine grid;
    
	// Use this for initialization
	void Awake () {
	    camCtrl = this.GetComponent<CamCtrl>();
	    cam = this.GetComponent<Camera>();
        for(int i=0; i<8;++i)
        {
            GameObject go = Boss.RequestObject(this.floaterPrototype);
            go.name = "Floater"+i;
            go.transform.SetParent(hudcanvas.transform);
            go.SetActive(false);
            Floater floater = go.GetComponent<Floater>();
            floater.uiCtrl = this;
            floaters.Add(floater);
        }
        
        
        int linesX = 9;
        int linesZ = 18;
        float distanceBetweenLines = 2f;
        var lineSteps = new List<Vector3>();
        int posMinX = -linesX/2;
        int posMaxX = posMinX+linesX;
        int posMinZ = -linesZ/2;
        int posMaxZ = posMinZ+linesZ;
        // Lines down X axis
        for (int i = posMinX; i <= posMaxX; ++i) {
            lineSteps.Add(new Vector3(i * distanceBetweenLines, 0, posMinZ* distanceBetweenLines));
            lineSteps.Add(new Vector3(i * distanceBetweenLines, 0, posMaxZ* distanceBetweenLines));
        }
        // Lines down Z axis
        for (int i = posMinZ; i <= posMaxZ; ++i) {
            lineSteps.Add(new Vector3(posMinX* distanceBetweenLines, 0, i * distanceBetweenLines));
            lineSteps.Add(new Vector3(posMaxX* distanceBetweenLines, 0, i * distanceBetweenLines));
        }
        var grid = new VectorLine("Grid", lineSteps, lineTex, lineWidth/3f, LineType.Discrete, Joins.None);
        grid.color = new Color(0.6f,0.6f,0.6f,0.2f);
        grid.Draw3DAuto();
        
        Boss.uiCtrls.Add(this);
        hudcanvas.gameObject.SetActive(true);
	}
    
    void Start()
    {
        stickCursor = Boss.RequestActor(this.stickCursorPrototype, null, false);
        activeCursor = Boss.RequestActor(this.activeCursorPrototype, null, false);
        floorCursor = Boss.RequestActor(this.floorCursorPrototype, null, false);
    }
    
    
	
	// Update is called once per frame
	void Update () {
        Ray ray;
        float distance = 0; 
        Rect screenRect = new Rect(0,0, Screen.width, Screen.height);
        if (screenRect.Contains(Input.mousePosition))
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);   
            if (camCtrl.plane.Raycast(ray, out distance))
            {
                stickCursor.transform.position = ray.GetPoint(distance);
            }
        }
        if( floorCursorExpireTimer > 0 )
        {
            floorCursorExpireTimer -= Time.deltaTime;
            if( floorCursorExpireTimer <= 0f )
            {
                floorCursor.body.gameObject.SetActive(false);
                HideFloater(floorCursor.transform);
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
        floorCursor.SetDestination(position);
        floorCursorExpireTimer = 1f;
        
        if( editPathMode )
        {
            IntVec2 vec = floorCursor.transform.position.ToIntVec2();
            if( activePath.Count > 0 )
            {
                IntVec2 last = activePath[activePath.Count-1]; 
                float min = 1.42f;
                float sqrMag = (last.ToVector3() - floorCursor.transform.position).sqrMagnitude;
                if( sqrMag > min*min )
                {
                    activePath.Add(vec);
                    activePawn.line.points3.Add(vec.ToVector3());
                    activePawn.line.Draw3DAuto();
                }
            }
            else
            {
                IntVec2 step = activePawn.transform.position.ToIntVec2();
                while(step.NotAdjacent(vec) && activePath.Count < 20)
                {
                    step = step.StepTowards(vec);
                    activePath.Add(step);
                }
                activePath.Add(vec);
                LineUtils.BuildLine(activePawn, lineTex, lineWidth);
            }
        }
    }
    
    public void ClickDown(Vector3 position)
    {
        bool clickOnFloorCursor = (position - floorCursor.transform.position).sqrMagnitude < 1.2f*1.2f;
        if( clickOnFloorCursor )
        {   
            ClickFloater(floorCursor.transform);
        }
        floorCursor.SetDestination(position);
        floorCursor.body.gameObject.SetActive(true);
        
        if( editPathMode )
        {
            //IntVec2 vec = activeCursor.transform.position.ToIntVec2();
            activePath.Clear();
            activePawn.line.points3.Clear();
        }
        
    }
    
    public void ClickUp(Vector3 position)
    {

       
    }
    
    public void SelectCockpit(Pawn cockpit)
    {
        editPathMode = cockpit != null;
        
        if( editPathMode )
        {
            if( activePawn != null && activePawn.line != null )
            {
                Color lc = activePawn.line.color;
                activePawn.line.color = new Color(lc.r,lc.g,lc.b, 0.3f);
            }
            activePawn = cockpit;
            if( activePawn != null && activePawn.line != null )
            {
                Color lc = activePawn.line.color;
                activePawn.line.color = new Color(lc.r,lc.g,lc.b, 1f);
            }
            if( activePawn.path == null )
            {
                activePawn.path = new List<IntVec2>();
            }
            activePath = activePawn.path;
        }
    }
    
    public void KillPathLines()
    {
        VectorLine.Destroy(allPathLines);
        allPathLines.Clear();
    }
    
    
}
