using UnityEngine;

public class User : MonoBehaviour
{
    bool isLocal;
    public bool IsLocal { get { return isLocal; } } 
    
    CamCtrl camCtrl;
    Actor cursor;
    Actor selection;
    public Team team;

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
                    camCtrl.pointsOfInterest.Add(selection.transform);
                    ProcessInput = PregameInput;
                } 
                break;
            case Referee.RefState.COUNTDOWN:
                if( isLocal )
                {
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
        }
    }
    
    public bool NoInput(float deltaTime, InputCtrl.InputParams inputs)
    {
        return false;
    }
    
    
    public bool PregameInput(float deltaTime, InputCtrl.InputParams inputs)
    {
        if( inputs.primaryButton )
        {
            Team desiredTeam = null;
            if( selection.transform.position.z < -1f )
            {
                desiredTeam = Referee.Ins.southTeam;
            }
            else if( selection.transform.position.z > 1f )
            {
                desiredTeam = Referee.Ins.northTeam;
            } 
            if( team == desiredTeam )
            {
                Referee.Ins.SpeedUp();
            }  
            else
            {
                if( team != null )
                {
                    team.Remove(this);
                }
                if( desiredTeam != null )
                {
                    desiredTeam.Add(this);
                }
            }
            selection.SetAxis(cursor.transform.position - selection.transform.position);
        }
        cursor.SetAxis(inputs.leftAxis);
        return true;
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
            cockpit.DoInput(deltaTime, inputs.primaryButton, inputs.secondaryButton);
            cursor.SetAxis(Vector2.zero);
        }
        else
        {
            cursor.SetAxis(inputs.leftAxis);
        }
        selection.SetAxis(cursor.transform.position - selection.transform.position);
        if( inputs.tertiaryButton )
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
