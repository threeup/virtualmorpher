using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AICtrl : MonoBehaviour 
{
    User user;
    
    float newPawnTimer = 1f;
    float defaultNewPawnTimer = 1f;
    
    float pathTimer = 0.4f;
    float defaultPathTimer = 0.4f;
    
    float actionTimer = 2f;
    float defaultActionTimer = 2f;
    
    bool primaryButton;
    bool secondaryButton;
    bool tertiaryButton;
    
    void Awake()
    {
        user = GetComponent<User>();
    }

    void Update () 
    {
        if( user.team == null )
        {
            return;
        }
        float deltaTime = Time.deltaTime;
        newPawnTimer -= deltaTime;
        if( newPawnTimer < 0)
        {
            user.SelectNextActor();
            newPawnTimer = defaultNewPawnTimer;
        }
        
        pathTimer -= deltaTime;
        if( pathTimer < 0 )
        {
            if( user.cockpit )
            {
                user.MakePath(GetDestination(user.cockpit));
            }
            pathTimer = defaultPathTimer;
        }
        
        actionTimer -= deltaTime;
        primaryButton = false;
        secondaryButton = false;
        tertiaryButton = false;
        if( actionTimer < 0 )
        {
            int rand = UnityEngine.Random.Range(0,5);
            primaryButton = rand==2;
            secondaryButton = rand==3;
            tertiaryButton = rand==4;
            actionTimer = defaultActionTimer;
        }
        user.DoButtons(deltaTime, primaryButton, secondaryButton, tertiaryButton);
	}
    
    Vector3 GetDestination(Pawn pawn)
    {
        Actor orb = Boss.referee.orb;
        float bestDistSqr = 999999;
        Pawn bestPawn = null;
        Team team = pawn.team;
        foreach(Pawn p in team.pawns)
        {
            if( (p.transform.position - orb.transform.position).sqrMagnitude < bestDistSqr )
            {
                bestDistSqr = (p.transform.position - orb.transform.position).sqrMagnitude;
                bestPawn = p;
            }
        }
        if( bestPawn == this )
        {
            return orb.transform.position;
        }
        Vector3 pos = team.GetPawnCoveragePosition(pawn.idx);
        if( orb.motor.possessTeam == team )
        {
            pos += team.transform.forward*3;
            pos.z *= 1.2f;
        }
        else if(orb.motor.possessTeam != null )
        {
            if(pawn.idx != 1 )
            {
                pos -= team.transform.forward*3;
                pos.z *= 0.8f;
            }
        }
        int rand = UnityEngine.Random.Range(0,3);
        switch(rand)
        {
            default:
            case 0: return pos+Vector3.zero;
            case 1: return pos+team.transform.forward*3;
            case 2: return pos+team.transform.right*3;
            case 3: return pos-team.transform.right*3;
        }
        
    }
    
}
