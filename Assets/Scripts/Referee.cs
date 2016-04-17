using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Referee : MonoBehaviour {

    
    public enum RefState
    {
        NONE,
        ROUNDSTART,
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
    
    float defaultAdvanceTimer = 0.2f;
    
    void Awake()
    {
        Boss.referee = this;
    }
    
    void Start()
    {
        StartCoroutine(SetupRoutine());
        
    }
    
    IEnumerator SetupRoutine()
    {
        yield return null;
        GotoState(RefState.ROUNDSTART);
    }
    
    void GotoState(RefState state)
    {
        if( refState == state)
        {
            return;
        }
        refState = state;
        Boss.ShowFloater(this.transform, 0, refState.ToString(), null);
        switch(refState)
        {
            case RefState.ROUNDSTART:
                northTeam.isReady = false;
                southTeam.isReady = false;
                this.isReady = false;
                SetAutoReadyTimer(defaultAdvanceTimer);
                break;
            case RefState.SPAWNING:
                Boss.menuCtrl.GoSideSelect(false);
                northTeam.isReady = false;
                southTeam.isReady = false;
                break;
            case RefState.ACTORSELECT:
                northTeam.isReady = false;
                southTeam.isReady = false;
                int localUsers = 0;
                foreach(User user in users)
                {
                    if( user.IsLocal )
                    {
                        user.AssignCamera(Boss.GetCamCtrl(localUsers));
                        localUsers++;
                    }
                    else
                    {
                        user.AssignCamera(null);
                    }
                }
                Boss.menuCtrl.GoSideSelect(true);
                Boss.menuCtrl.GoEndOfGame(false);
                break;
            case RefState.COUNTDOWN:
                this.isReady = false;
                Boss.menuCtrl.GoSideSelect(false);
                SetAutoReadyTimer(defaultAdvanceTimer);
                break;
            case RefState.PLAYING:
                isReady = false;
                northTeam.AddAI();
                southTeam.AddAI();
                orb = Boss.RequestActor(Boss.actorWorld.orbPrototype, orbDropPoint, false);
                isReady = true;
                break; 
            case RefState.RESETGAME:
                this.isReady = false;
                northTeam.isReady = false;
                southTeam.isReady = false;
                Boss.AddGarbage(orb.gameObject);
                orb = null;
                SetAutoReadyTimer(5f);
                break;   
            case RefState.FINISHED:
                northTeam.isReady = false;
                southTeam.isReady = false;
                Boss.menuCtrl.GoEndOfGame(true);
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
            case RefState.ROUNDSTART:
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
                if(isReady && (northTeam.towers.Count == 0 || southTeam.towers.Count == 0) )
                {
                    /*if(northTeam.score == 3 || southTeam.score == 3)
                    {
                        GotoState(RefState.FINISHED);
                    }
                    else*/
                    {
                        GotoState(RefState.RESETGAME);   
                    }
                }
                break;
            case RefState.RESETGAME:
                //kill everything
                if( isReady && northTeam.isReady && southTeam.isReady )
                {
                    Boss.actorWorld.CleanGarbage();
                    GotoState(RefState.ROUNDSTART);
                }  
                break;
            case RefState.FINISHED:
                
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
                Boss.HideFloater(this.transform, 1);
            }
            else
            {
                int rounded = (int)Mathf.Round(autoReadyTimer*10);
                Boss.ShowFloater(this.transform, 1, "Starting in "+rounded/10f, null);
            }
            
        }
        if( tempFloaterTimer > 0 )
        {
            tempFloaterTimer -= Time.deltaTime;
            if( tempFloaterTimer <= 0f )
            {
                Boss.HideFloater(this.transform, 2);
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
        Boss.ShowFloater(this.transform, 2, txt, null);
        tempFloaterTimer = 3f;
    }
}
