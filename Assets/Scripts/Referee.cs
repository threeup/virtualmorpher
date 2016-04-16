using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Referee : MonoBehaviour {

    public static Referee Ins;
    
    public enum RefState
    {
        NONE,
        SIDESELECT,
        SPAWNING,
        COUNTDOWN,
        PLAYING,
        RESETGAME,
        FINISHED,
    }
    
    RefState refState = RefState.NONE;
    
    public Team northTeam;
    public Team southTeam;
    public List<User> users = new List<User>();
    public Transform orbDropPoint;
    public Actor orb;
    
    void Awake()
    {
        Ins = this;
    }
    
    void Start()
    {
        StartCoroutine(SetupRoutine());
        
    }
    
    IEnumerator SetupRoutine()
    {
        yield return null;
        GotoState(RefState.SIDESELECT);
    }
    
    void GotoState(RefState state)
    {
        if( refState == state)
        {
            return;
        }
        refState = state;
        switch(refState)
        {
            case RefState.SIDESELECT:
                northTeam.isReady = false;
                southTeam.isReady = false;
                int localUsers = 0;
                foreach(User user in users)
                {
                    if( user.IsLocal )
                    {
                        user.AssignCamera(localUsers==0 ? CamCtrl.First : CamCtrl.Second);
                        localUsers++;
                    }
                    else
                    {
                        user.AssignCamera(null);
                    }
                }
                MenuCtrl.Ins.GoSideSelect(true);
                MenuCtrl.Ins.GoEndOfGame(false);
                break;
            case RefState.SPAWNING:
                MenuCtrl.Ins.GoSideSelect(false);
                northTeam.isReady = false;
                southTeam.isReady = false;
                break;
            case RefState.COUNTDOWN:
                
                break;
            case RefState.PLAYING:
                orb = ActorWorld.Ins.RequestOrb(orbDropPoint);
                break;    
            case RefState.FINISHED:
                northTeam.isReady = false;
                southTeam.isReady = false;
                MenuCtrl.Ins.GoEndOfGame(true);
                break;
        }
        northTeam.HandleRefStateChange(refState);
        southTeam.HandleRefStateChange(refState);
        foreach(User user in users)
        {
            user.HandleRefStateChange(refState);
        }
    }
    
    void Update()
    {
        switch(refState)
        {
            case RefState.SIDESELECT:
                if(northTeam.isReady && southTeam.isReady)
                {
                    GotoState(RefState.SPAWNING);
                }
                break;
            case RefState.SPAWNING:
                if(northTeam.isReady && southTeam.isReady && orb)
                {
                    GotoState(RefState.COUNTDOWN);
                }
                break;
            case RefState.COUNTDOWN:
                GotoState(RefState.PLAYING);
                break;
            case RefState.PLAYING:
                if(!orb)
                {
                    if(northTeam.score > 2 || southTeam.score > 2)
                    {
                        GotoState(RefState.FINISHED);
                    }
                    else
                    {
                        GotoState(RefState.RESETGAME);    
                    }
                }
                break;
            case RefState.FINISHED:
                if( northTeam.isReady && southTeam.isReady)
                {
                    GotoState(RefState.SIDESELECT);
                }
                break;
        }
    }
    
    public void SpeedUp()
    {
        
    }
}
