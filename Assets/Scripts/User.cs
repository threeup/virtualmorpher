using UnityEngine;
using System;
using System.Collections.Generic;

public class User : MonoBehaviour
{
    bool isLocal;
    public bool IsLocal { get { return isLocal; } } 
    bool isAI;
    public bool IsAI { get { return isAI; } } 
    
    public CamCtrl camCtrl;
    public UICtrl uiCtrl;
    Actor stickCursor;
    Actor activeCursor;
    Actor floorCursor;
    public Team team;
    Team desiredTeam;

    public Pawn cockpit;
    
    public delegate bool InputDelegate(float deltaTime, InputCtrl.InputParams inputs);
    public InputDelegate ProcessInput;
    
    void Awake()
    {
        ProcessInput = NoInput;
        isLocal = GetComponent<InputCtrl>() != null;
        isAI = GetComponent<AICtrl>() != null;
    }
    
    public void AssignCamera(CamCtrl camCtrl)
    {
        this.camCtrl = camCtrl;
        if( camCtrl )
        {
            this.uiCtrl = camCtrl.uiCtrl;
            this.stickCursor = uiCtrl.stickCursor;
            this.activeCursor = uiCtrl.activeCursor;
            this.floorCursor = uiCtrl.floorCursor;
        }
        else
        {
            this.uiCtrl = null;
            this.stickCursor = null;
            this.activeCursor = null;
            this.floorCursor = null;
        }
    }
    
    public void HandleRefStateChange(Referee.RefState state)
    {
        switch(state)
        {
            case Referee.RefState.SPAWNING:
                if( isLocal )
                {
                    ProcessInput = NoInput;
                }
                break;
            case Referee.RefState.ACTORSELECT:
                if( isLocal )
                {
                    uiCtrl.HideFloater(floorCursor.transform, 0);
                    camCtrl.pointsOfInterest.Add(stickCursor.transform);
                    camCtrl.pointsOfInterest.Add(Boss.referee.transform);
                    ProcessInput = ActorSelectInput;
                }
                break;
            case Referee.RefState.COUNTDOWN:
                if( isLocal && team )
                {
                    ProcessInput = NoInput;
                    uiCtrl.HideFloater(floorCursor.transform, 0);
                    camCtrl.pointsOfInterest.Clear();
                    camCtrl.pointsOfInterest.Add(team.alpha.transform);
                    camCtrl.pointsOfInterest.Add(team.bravo.transform);
                    camCtrl.pointsOfInterest.Add(team.charlie.transform);
                    camCtrl.pointsOfInterest.Add(Boss.referee.transform);
                }
                break;
            case Referee.RefState.PLAYING:
                if( isLocal )
                {
                    if(cockpit == null)
                    {
                        SelectNextActor();
                    }
                    camCtrl.pointsOfInterest.Remove(Boss.referee.transform);
                    ProcessInput = PlayingInput;
                } 
                break;
            case Referee.RefState.RESETGAME:
                if( isLocal )
                {
                    camCtrl.pointsOfInterest.Clear();
                    camCtrl.keyInterest = null;
                    uiCtrl.SelectCockpit(null);
                    ProcessInput = NoInput;
                } 
                break;
            case Referee.RefState.FINISHED:
                if( isLocal )
                {
                    camCtrl.pointsOfInterest.Clear();
                    ProcessInput = NoInput;
                } 
                break;
        }
    }
    
    public bool NoInput(float deltaTime, InputCtrl.InputParams inputs)
    {
        return false;
    }
    
    public bool ActorSelectInput(float deltaTime, InputCtrl.InputParams inputs)
    {
        CheckStickCursor(inputs);
        CheckSideSelect(inputs);
        return true;
    }
    
    
    public bool PlayingInput(float deltaTime, InputCtrl.InputParams inputs)
    {
        CheckButtons(deltaTime, inputs);
        CheckStickCursor(inputs);
        return true;
    
    }
    
    void CheckSideSelect(InputCtrl.InputParams inputs)
    {
        
    }
    
    public void JoinTeam(Team nextTeam)
    {
        if( team != null )
        {
            team.Remove(this);
        }
        team = nextTeam;
        if( team != null )
        {
            team.Add(this);
            if( isLocal )
            {
                stickCursor.body.ApplyColor(team.teamColor);
                activeCursor.body.ApplyColor(team.teamColor);
                floorCursor.body.ApplyColor(team.teamColor);
            }
        }
        else
        {
            if( isLocal )
            {
                stickCursor.body.ApplyColor(Color.white);
                activeCursor.body.ApplyColor(Color.white);
                floorCursor.body.ApplyColor(Color.white);
            }
        }
    }
    
    
    void CheckButtons(float deltaTime, InputCtrl.InputParams inputs)
    {
        if( cockpit != null )
        {
            bool directMode = false;
            if( directMode )
            {
                cockpit.SetAxis(inputs.leftAxis);
            }    
        }
        DoButtons(deltaTime, inputs.button[2], inputs.button[3], inputs.button[4]);
    }
    
    void CheckStickCursor(InputCtrl.InputParams inputs)
    {
        if( cockpit != null )
        {
            activeCursor.SetAxis(cockpit.transform.position - activeCursor.transform.position);
        }
        if( inputs.button[1] )
        {
            uiCtrl.ClickHeld(stickCursor.transform.position);
        }
        if( inputs.buttonDown[1] )
        {
            uiCtrl.ClickDown(stickCursor.transform.position);
        }
        if( inputs.buttonUp[1] )
        {
            uiCtrl.ClickUp(stickCursor.transform.position);
        }
        if( inputs.buttonDown[0] )
        {
            SelectNearestActor(stickCursor.transform.position);
            if( isLocal )
            {
                uiCtrl.SelectCockpit(cockpit);
                camCtrl.keyInterest = cockpit != null ? cockpit.transform : null;
            }
        }
        if( inputs.buttonDown[5] )
        {
            SelectNextActor();
            if( isLocal )
            {
                uiCtrl.SelectCockpit(cockpit);
                camCtrl.keyInterest = cockpit != null ? cockpit.transform : null;
            }
        }
    }
    
    
    public void SelectNearestActor(Vector3 pos)
    {
        float bestDistSqr = 999999;
        Pawn bestPawn = null;
        List<Pawn> pawns = Boss.actorWorld.activePawns;
        if( team != null )
        {
            pawns = team.pawns;
        }
        foreach(Pawn p in pawns)
        {
            if( (p.transform.position - pos).sqrMagnitude < bestDistSqr )
            {
                bestDistSqr = (p.transform.position - pos).sqrMagnitude;
                bestPawn = p;
            }
        }
        if( bestPawn != null && team == null )
        {
            JoinTeam(bestPawn.team);
        }
        cockpit = bestPawn;
        if( isLocal )
        {
            if( cockpit )
            {
                uiCtrl.LeftClickDown(cockpit.transform.position);
            }
            else
            {
                uiCtrl.LeftClickDown(pos);
            }
        }
    }
    
    public void SelectNextActor()
    {
        if( cockpit == null )
        {
            cockpit = team.alpha;
        }
        else if( cockpit == team.alpha )
        {
            cockpit = team.bravo;
        }
        else if( cockpit == team.bravo )
        {
            cockpit = team.charlie;
        }
        else if( cockpit == team.charlie )
        {
            cockpit = team.alpha;
        }
    }
    
    public void MakePath(Vector3 endPoint)
    {
        IntVec2 vec = endPoint.ToIntVec2();
        IntVec2 step = cockpit.transform.position.ToIntVec2();
        cockpit.path.Clear();
        while(step.NotAdjacent(vec) && cockpit.path.Count < 20)
        {
            step = step.StepTowards(vec);
            cockpit.path.Add(step);
        }
        cockpit.path.Add(vec);
    }
    
    public void DoButtons(float deltaTime, bool primary, bool secondary, bool tertiary)
    {
        foreach(Pawn p in team.pawns)
        {
            if(p==cockpit)
            {
                p.DoButtons(deltaTime, primary, secondary, tertiary);
            }
            else
            {
                p.DoButtons(deltaTime, false, false, false);
            }
        }
    }
    
     
}
