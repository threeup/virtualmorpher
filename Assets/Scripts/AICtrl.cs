using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AICtrl : MonoBehaviour 
{
    User user;
    
    float newPawnTimer = 2f;
    float defaultNewPawnTimer = 2f;
    
    float pathTimer = 2f;
    float defaultPathTimer = 2f;
    
    float actionTimer = 2f;
    float defaultActionTimer = 2f;
    
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
            user.SelectActor();
            newPawnTimer = defaultNewPawnTimer;
        }
        
        pathTimer -= deltaTime;
        if( pathTimer < 0 )
        {
            user.MakePath(GetDestination(user.cockpit));
            pathTimer = defaultPathTimer;
        }
        
        actionTimer -= deltaTime;
        if( actionTimer < 0 )
        {
            int rand = UnityEngine.Random.Range(0,5);
            user.cockpit.DoButtons(deltaTime, rand==2, rand==3, rand==4);
            actionTimer = defaultActionTimer;
        }
	}
    
    Vector3 GetDestination(Pawn pawn)
    {
        int rand = UnityEngine.Random.Range(0,6);
        switch(rand)
        {
            default:
            case 0: return Vector3.zero;
            case 1: return Vector3.right*5;
            case 2: return Vector3.right*5+Vector3.forward*5;
            case 3: return Vector3.right*5-Vector3.forward*5;
            case 4: return -Vector3.right*5-Vector3.forward*5;
            case 5: return -Vector3.right*5+Vector3.forward*5;
            case 6: return -Vector3.right;
        }
    }
    
}
