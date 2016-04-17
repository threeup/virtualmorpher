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
        ACTORSELECT,
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
    
    float autoReadyTimer = -1f;
    float tempFloaterTimer = -1f;
    bool isReady = true;
    
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
        CamCtrl.First.ShowFloater(this.transform, 0, refState.ToString(), null);
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
            case RefState.ACTORSELECT:
                northTeam.isReady = false;
                southTeam.isReady = false;
                SetAutoReadyTimer(5f);
                break;
            case RefState.COUNTDOWN:
                this.isReady = false;
                SetAutoReadyTimer(5f);
                break;
            case RefState.PLAYING:
                isReady = false;
                orb = ActorWorld.Ins.RequestOrb(orbDropPoint);
                isReady = true;
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
                if(isReady && northTeam.isReady && southTeam.isReady)
                {
                    GotoState(RefState.SPAWNING);
                }
                break;
            case RefState.SPAWNING:
                if(isReady && northTeam.isReady && southTeam.isReady)
                {
                    GotoState(RefState.ACTORSELECT);
                }
                break;
            case RefState.ACTORSELECT:
                if(isReady && northTeam.isReady && southTeam.isReady)
                {
                    GotoState(RefState.COUNTDOWN);
                }
                break;
            case RefState.COUNTDOWN:
                if(isReady)
                {
                    GotoState(RefState.PLAYING);
                }
                break;
            case RefState.PLAYING:
                if(isReady && !orb)
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
        
        if( autoReadyTimer > 0 )
        {
            autoReadyTimer -= Time.deltaTime;
            if( autoReadyTimer <= 0f )
            {
                northTeam.isReady = true;
                southTeam.isReady = true;
                isReady = true;
                autoReadyTimer = -1f;
                CamCtrl.First.HideFloater(this.transform, 1);
            }
            else
            {
                int rounded = (int)Mathf.Round(autoReadyTimer*10);
                CamCtrl.First.ShowFloater(this.transform, 1, "Starting in "+rounded/10f, null);
            }
            
        }
        if( tempFloaterTimer > 0 )
        {
            tempFloaterTimer -= Time.deltaTime;
            if( tempFloaterTimer <= 0f )
            {
                CamCtrl.First.HideFloater(this.transform, 2);
            }
        }
    }
    
    public void SetAutoReadyTimer(float amount)
    {
        if( autoReadyTimer < 0 || autoReadyTimer > amount )
        {
            autoReadyTimer = amount;
        }
    }
    
    public void TempFloater(string txt)
    {
        CamCtrl.First.ShowFloater(this.transform, 2, txt, null);
        tempFloaterTimer = 3f;
    }
}
