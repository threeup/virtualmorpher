using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Team : MonoBehaviour
{
    public string teamName = "Team";
    public Color teamColor = Color.white;
    public bool isReady = false;
    public int score;
    
    public List<User> users = new List<User>();

    public Pawn alpha;
    public Pawn bravo;
    public Pawn charlie;
    public List<Pawn> pawns = new List<Pawn>();
    
    Vector3 spawnPosition = Vector3.zero;
    Quaternion spawnRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
    
    public delegate void UpdateDelegate(float deltaTime);
    public UpdateDelegate ProcessUpdate;
    
    void Awake()
    {
        spawnPosition = this.transform.position - Vector3.right*5f;
        spawnRotation = this.transform.rotation;
        ProcessUpdate = NoUpdate;
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
                ProcessUpdate = NoUpdate;
                break;
            case Referee.RefState.SPAWNING:
                alpha = Boss.RequestPawn("PawnA",this, Pawn.PawnType.FAT);
                alpha.body.ApplyColor(teamColor);
                pawns.Add(alpha);
                bravo = Boss.RequestPawn("PawnB",this, Pawn.PawnType.TALL);
                bravo.body.ApplyColor(teamColor);
                pawns.Add(bravo);
                charlie = Boss.RequestPawn("PawnC",this, Pawn.PawnType.MED);
                charlie.body.ApplyColor(teamColor);
                pawns.Add(charlie);
                isReady = true;
                ProcessUpdate = NoUpdate;
                break;
            case Referee.RefState.ACTORSELECT:
                ProcessUpdate = NoUpdate;
                break;
            case Referee.RefState.COUNTDOWN:
                ProcessUpdate = NoUpdate;
                break;
            case Referee.RefState.PLAYING:
                ProcessUpdate = PlayingUpdate;
                break;
            case Referee.RefState.FINISHED:
                ProcessUpdate = NoUpdate;
                break;
        }
    }
    
    public void Add(User user)
    {
        users.Add(user);
        isReady = true;
        Referee.Ins.SetAutoReadyTimer(5f);
        Referee.Ins.TempFloater(user+" Joined "+this.teamName);
    }
    
    public void Remove(User user)
    {
        users.Remove(user);
        if( users.Count == 0)
        {
            isReady = false;
        }
        Referee.Ins.TempFloater(user+" Left "+this.teamName);
    }
    
    public void NoUpdate(float deltaTime)
    {
        return;
    }
    
    public void PlayingUpdate(float deltaTime)
    {
        foreach(Pawn pawn in pawns)
        {
            pawn.UpdateMotion(deltaTime);
        }
    }
    
    void Update()
    {
        ProcessUpdate(Time.deltaTime);
    }
    
}
