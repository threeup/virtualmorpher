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
    public List<Actor> towers = new List<Actor>();
    
    Vector3 spawnPosition = Vector3.zero;
    Vector3 towerSpawnPosition = Vector3.zero;
    Quaternion spawnRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
    
    public delegate void UpdateDelegate(float deltaTime);
    public UpdateDelegate ProcessUpdate;
    
    void Awake()
    {
        
        spawnRotation = this.transform.rotation;
        ProcessUpdate = NoUpdate;
    }
    public Vector3 GetPawnSpawnPosition()
    {
        Vector3 result = spawnPosition;
        spawnPosition += Vector3.right*5f;
        return result;
    }
    public Vector3 GetTowerSpawnPosition()
    {
        Vector3 result = towerSpawnPosition;
        towerSpawnPosition += Vector3.right*5f;
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
            case Referee.RefState.ROUNDSTART:
                score = 0;
                spawnPosition = this.transform.position - Vector3.right*5f;
                spawnPosition.z *= 0.5f;
                towerSpawnPosition = this.transform.position - Vector3.right*5f;
                towerSpawnPosition.z *= 0.8f;
                isReady = true;
                break;
            case Referee.RefState.SPAWNING:
                alpha = Boss.RequestPawn("PawnA",this, Pawn.PawnType.FAT);
                alpha.SetHat(Boss.RequestHat());
                pawns.Add(alpha);
                bravo = Boss.RequestPawn("PawnB",this, Pawn.PawnType.TALL);
                bravo.SetHat(Boss.RequestHat());
                pawns.Add(bravo);
                charlie = Boss.RequestPawn("PawnC",this, Pawn.PawnType.MED);
                charlie.SetHat(Boss.RequestHat());
                pawns.Add(charlie);
                isReady = true;
                ProcessUpdate = NoUpdate;
                break;
            case Referee.RefState.ACTORSELECT:
                ProcessUpdate = NoUpdate;
                break;
            case Referee.RefState.COUNTDOWN:
                for(int i=0; i<3; ++i)
                {
                     Actor tower = Boss.RequestTower(teamName+" Tower"+i, this, false);
                     tower.body.ApplyColor(teamColor);
                     towers.Add(tower);
                }
                Actor giantTower = Boss.RequestTower(teamName+" Home", this, true);
                giantTower.body.ApplyColor(teamColor);
                towers.Add(giantTower);
                
                ProcessUpdate = NoUpdate;
                break;
            case Referee.RefState.PLAYING:
                ProcessUpdate = PlayingUpdate;
                break;
            case Referee.RefState.RESETGAME:
                foreach(Pawn pawn in pawns)
                {
                    Explode(pawn);
                }
                pawns.Clear();
                alpha = null;
                bravo = null;
                charlie = null;
                foreach(Actor tower in towers)
                {
                    Explode(tower);
                }
                towers.Clear();
                isReady = true;
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
        Boss.referee.SetAutoReadyTimer(5f);
        Boss.referee.TempFloater(user+" Joined "+this.teamName);
    }
    
    public void AddAI()
    {
        if(users.Count == 0)
        {
            for(int i=0; i<Boss.referee.users.Count; ++i)
            {
                User u = Boss.referee.users[i];
                if( u.IsAI && u.team == null )
                {
                    u.JoinTeam(this);
                    return;
                }
            }
        }
    }
    
    public void Remove(User user)
    {
        users.Remove(user);
        if( users.Count == 0)
        {
            isReady = false;
        }
        Boss.referee.TempFloater(user+" Left "+this.teamName);
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
    
    public bool ExplodeTower(Actor tower)
    {
        if( !towers.Contains(tower) )
        {
            return false;
        }
        towers.Remove(tower);
        Explode(tower);
        score -= 1;
        return true;
    }
    
    public void Explode(Actor actor)
    {
        Rigidbody[] rbs = actor.gameObject.GetComponentsInChildren<Rigidbody>() ;
        foreach(Rigidbody rb in rbs)
        {
            Collider c = rb.gameObject.GetComponent<Collider>();
            c.enabled = true;
            c.isTrigger = false;
            rb.isKinematic = false;
            rb.transform.parent = null;
            rb.AddForce(new Vector3(
                UnityEngine.Random.Range(-20,20),
                UnityEngine.Random.Range(14,20),
                UnityEngine.Random.Range(-20,20)), ForceMode.Impulse);
            Boss.AddGarbage(rb.gameObject);
        }
        Boss.AddGarbage(actor.gameObject);
        
    }
    
}
