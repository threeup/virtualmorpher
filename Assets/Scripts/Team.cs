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
    public Vector3 GetPawnSpawnPosition(int idx)
    {
        return spawnPosition + (idx-1)*Vector3.right*5f;
    }
    public Vector3 GetPawnCoveragePosition(int idx)
    {
        if( idx == 1 )
        {
            return spawnPosition + this.transform.forward*5f;    
        }
        float factor = idx == 0 ? -1 : 1;
        return spawnPosition + this.transform.forward*13f+Vector3.right*4f*factor;  
    }
    public Vector3 GetTowerSpawnPosition(int idx)
    {
        return towerSpawnPosition + idx*Vector3.right*5f;
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
                spawnPosition = this.transform.position;
                spawnPosition.z *= 0.5f;
                towerSpawnPosition = this.transform.position - Vector3.right*5f;
                towerSpawnPosition.z *= 0.8f;
                isReady = true;
                break;
            case Referee.RefState.SPAWNING:
                alpha = Boss.RequestPawn("PawnA",this, 0, Pawn.PawnType.FAT);
                alpha.SetHat(Boss.RequestHat());
                pawns.Add(alpha);
                bravo = Boss.RequestPawn("PawnB",this, 1, Pawn.PawnType.TALL);
                bravo.SetHat(Boss.RequestHat());
                pawns.Add(bravo);
                charlie = Boss.RequestPawn("PawnC",this, 2, Pawn.PawnType.MED);
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
                     Actor tower = Boss.RequestTower(teamName+" Tower"+i, this, i, false);
                     tower.body.ApplyColor(teamColor);
                     towers.Add(tower);
                }
                Actor giantTower = Boss.RequestTower(teamName+" Home", this, 0, true);
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
        Boss.referee.SetAutoReadyTimer(3f);
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
    
    public void DieAndReplace(Actor actor)
    {
        bool isTower = ExplodeTower(actor);
        if(!isTower)
        {
            Pawn removedPawn = actor as Pawn;
            pawns.Remove(removedPawn);
            Explode(removedPawn);
            Pawn nextPawn = null;
            if( removedPawn.idx == 0 )
            {
                alpha = Boss.RequestPawn("PawnA",this, 0, Pawn.PawnType.FAT);
                nextPawn = alpha;
            }
            if( removedPawn.idx == 1 )
            {
                bravo = Boss.RequestPawn("PawnB",this, 1, Pawn.PawnType.FAT);
                nextPawn = charlie;
            }
            if( removedPawn.idx == 2 )
            {
                charlie = Boss.RequestPawn("PawnC",this, 2, Pawn.PawnType.FAT);
                nextPawn = charlie;
            }
            foreach(User u in users)
            {
                if (u.cockpit == removedPawn)
                {
                    u.cockpit = nextPawn;
                }
            }
            if(nextPawn)
            {
                nextPawn.SetHat(Boss.RequestHat());
                pawns.Add(nextPawn);
            }
        }
    }
    
}
