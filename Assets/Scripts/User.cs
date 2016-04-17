using UnityEngine;
using System;

public class User : MonoBehaviour
{
    bool isLocal;
    public bool IsLocal { get { return isLocal; } } 
    
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
                    camCtrl.pointsOfInterest.Add(Referee.Ins.transform);
                    ProcessInput = ActorSelectInput;
                }
                break;
            case Referee.RefState.COUNTDOWN:
                if( isLocal )
                {
                    ProcessInput = NoInput;
                    uiCtrl.HideFloater(floorCursor.transform, 0);
                    camCtrl.pointsOfInterest.Clear();
                    camCtrl.pointsOfInterest.Add(team.alpha.transform);
                    camCtrl.pointsOfInterest.Add(team.bravo.transform);
                    camCtrl.pointsOfInterest.Add(team.charlie.transform);
                    camCtrl.pointsOfInterest.Add(Referee.Ins.transform);
                }
                break;
            case Referee.RefState.PLAYING:
                if( isLocal )
                {
                    camCtrl.pointsOfInterest.Remove(Referee.Ins.transform);
                    ProcessInput = PlayingInput;
                } 
                break;
            case Referee.RefState.FINISHED:
                if( isLocal )
                {
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
        CheckSelectActor(inputs);
        return true;
    }
    
    
    public bool PlayingInput(float deltaTime, InputCtrl.InputParams inputs)
    {
        CheckButtons(deltaTime, inputs);
        CheckStickCursor(inputs);
        CheckSelectActor(inputs);
        return true;
    
    }
    
    void CheckSideSelect(InputCtrl.InputParams inputs)
    {
        if( cockpit == null && inputs.button[0] )
        {
            desiredTeam = null;
            if( floorCursor.transform.position.z < -1f )
            {
                desiredTeam = Referee.Ins.northTeam;
            }
            else if( floorCursor.transform.position.z > 1f )
            {
                desiredTeam = Referee.Ins.southTeam;
            } 

            if( !desiredTeam )
            {
                if( team )
                {
                    Action<bool> floaterSucess = success => JoinTeam(null);
                    uiCtrl.ShowFloater(floorCursor.transform, 0, "Leave "+team+"?", floaterSucess);
                }
                else
                {
                    uiCtrl.HideFloater(floorCursor.transform, 0);
                }
            }
            else if( desiredTeam != team )
            {
                Action<bool> floaterSucess = success => JoinTeam(desiredTeam);
                uiCtrl.ShowFloater(floorCursor.transform, 0, "Join "+desiredTeam.teamName+"?", floaterSucess);
            }
        }
    }
    
    void JoinTeam(Team nextTeam)
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
            cockpit.DoButtons(deltaTime, inputs.button[2], inputs.button[3], inputs.button[4]);
        }
    }
    
    void CheckStickCursor(InputCtrl.InputParams inputs)
    {
        if( cockpit != null )
        {
            activeCursor.SetAxis(cockpit.transform.position - activeCursor.transform.position);
        }
        floorCursor.SetAxis(stickCursor.transform.position - floorCursor.transform.position);
        if( inputs.button[0] )
        {
            uiCtrl.ClickHeld(stickCursor.transform.position);
        }
        if( inputs.buttonDown[0] )
        {
            uiCtrl.ClickDown(stickCursor.transform.position);
        }
        if( inputs.buttonUp[0] )
        {
            uiCtrl.ClickUp(stickCursor.transform.position);
        }
    }
    
    void CheckSelectActor(InputCtrl.InputParams inputs)
    {
        if( team && inputs.buttonDown[1] )
        {
            //Pawn lastCockpit = cockpit;
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
            uiCtrl.SelectCockpit(team, cockpit);
            camCtrl.keyInterest = cockpit != null ? cockpit.transform : null;
        }
    }
    
     
}
