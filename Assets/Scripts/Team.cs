using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Team : MonoBehaviour
{
    public bool isReady = false;
    public int score;
    
    public List<User> users = new List<User>();

    public Pawn alpha;
    public Pawn bravo;
    public Pawn charlie;
    
    Vector3 spawnPosition = Vector3.zero;
    Quaternion spawnRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
    
    void Awake()
    {
        spawnPosition = this.transform.position;
        spawnRotation = this.transform.rotation;
    }
    public Vector3 GetSpawnPosition()
    {
        Vector3 result = spawnPosition;
        spawnPosition += Vector3.right*5f;
        return result;
    }
    
    public Quaternion GetSpawnRotation()
    {
        return spawnRotation;
    }
    

    public void HandleRefStateChange(Referee.RefState state)
    {
        switch(state)
        {
            case Referee.RefState.SIDESELECT: 
                score = 0;
                break;
            case Referee.RefState.SPAWNING:
                alpha = ActorWorld.Ins.RequestPawn("PawnA",this, Pawn.PawnType.FAT);
                bravo = ActorWorld.Ins.RequestPawn("PawnB",this, Pawn.PawnType.TALL);
                charlie = ActorWorld.Ins.RequestPawn("PawnC",this, Pawn.PawnType.MED);
                isReady = true;
                break;
            case Referee.RefState.COUNTDOWN:
                break;
            case Referee.RefState.PLAYING:
                break;
            case Referee.RefState.FINISHED:
                break;
        }
    }
    
    public void Add(User user)
    {
        users.Add(user);
        isReady = true;
    }
    
    public void Remove(User user)
    {
        users.Remove(user);
        if( users.Count == 0)
        {
            isReady = false;
        }
    }
    
    
}
