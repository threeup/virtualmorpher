using UnityEngine;
using System;

public class User : MonoBehaviour
{
    bool isLocal;
    public bool IsLocal { get { return isLocal; } } 
    
    CamCtrl camCtrl;
    Actor cursor;
    Actor selection;
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
    }
    
    public void HandleRefStateChange(Referee.RefState state)
    {
        switch(state)
        {
            case Referee.RefState.SIDESELECT: 
                this.cursor = camCtrl ? camCtrl.cursor : null;
                this.selection = camCtrl ? camCtrl.selection : null;
                if( isLocal )
                {
                    camCtrl.pointsOfInterest.Add(cursor.transform);
                    ProcessInput = SideSelectInput;
                } 
                break;
            case Referee.RefState.SPAWNING:
                if( isLocal )
                {
                    ProcessInput = NoInput;
                }
                break;
            case Referee.RefState.ACTORSELECT:
                if( isLocal )
                {
                    ProcessInput = ActorSelectInput;
                }
                break;
            case Referee.RefState.COUNTDOWN:
                if( isLocal )
                {
                    ProcessInput = NoInput;
                    camCtrl.HideFloater(selection.transform, 0);
                    camCtrl.pointsOfInterest.Clear();
                    camCtrl.pointsOfInterest.Add(team.alpha.transform);
                    camCtrl.pointsOfInterest.Add(team.bravo.transform);
                    camCtrl.pointsOfInterest.Add(team.charlie.transform);
                }
                break;
            case Referee.RefState.PLAYING:
                if( isLocal )
                {
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
        if( inputs.button[0] )
        {
            
        }
        return true;
    }
    public bool SideSelectInput(float deltaTime, InputCtrl.InputParams inputs)
    {
        if( inputs.button[0] )
        {
            desiredTeam = null;
            if( selection.transform.position.z < -1f )
            {
                desiredTeam = Referee.Ins.southTeam;
            }
            else if( selection.transform.position.z > 1f )
            {
                desiredTeam = Referee.Ins.northTeam;
            } 

            if( !desiredTeam )
            {
                if( team )
                {
                    Action<bool> floaterSucess = success => JoinTeam(null);
                    camCtrl.ShowFloater(selection.transform, 0, "Leave "+team+"?", floaterSucess);
                }
                else
                {
                    camCtrl.HideFloater(selection.transform, 0);
                }
            }
            else if( desiredTeam != team )
            {
                Action<bool> floaterSucess = success => JoinTeam(desiredTeam);
                camCtrl.ShowFloater(selection.transform, 0, "Join "+desiredTeam.teamName+"?", floaterSucess);
            }
            camCtrl.ClickHeld(cursor.transform.position);
        }
        if( inputs.buttonDown[0] )
        {
            camCtrl.ClickDown(cursor.transform.position);
        }
        if( inputs.buttonUp[0] )
        {
            camCtrl.ClickUp(cursor.transform.position);
        }
        cursor.SetAxis(inputs.leftAxis);
        return true;
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
        }
    }
    
    public bool ReadyInput(float deltaTime, InputCtrl.InputParams inputs)
    {
        return true;
    }
    
    public bool PlayingInput(float deltaTime, InputCtrl.InputParams inputs)
    {
        if( cockpit != null )
        {
            cockpit.SetAxis(inputs.leftAxis);
            cockpit.DoInput(deltaTime, inputs.button[0], inputs.button[1]);
            cursor.SetAxis(Vector2.zero);
        }
        else
        {
            cursor.SetAxis(inputs.leftAxis);
        }
        selection.SetAxis(cursor.transform.position - selection.transform.position);
        if( inputs.buttonDown[2] )
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
            camCtrl.keyInterest = cockpit != null ? cockpit.transform : null;
        }
        return true;
    }
     
}
