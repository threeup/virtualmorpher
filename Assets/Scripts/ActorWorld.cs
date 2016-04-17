using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActorWorld : MonoBehaviour {

    public List<Pawn> activePawns = new List<Pawn>();
    public List<Actor> activeActors = new List<Actor>();
    
    public GameObject pawnPrototype;
    public GameObject orbPrototype;
    public GameObject bulletPrototype;
    public GameObject cannonPrototype;
    public GameObject shieldPrototype;
    public GameObject toolPrototype;
    public GameObject roboBodyPrototype;
    
    public GameObject towerPrototype;
    public GameObject towerBodyPrototype;
    public GameObject giantTowerBodyPrototype;  
    
    public List<GameObject> garbage = new List<GameObject>(); 
    public float garbageTimer = 7f; 

	void Awake() 
    {
	    Boss.actorWorld = this;
	}
    
    public Pawn RequestPawn(string name, Team team, Pawn.PawnType ptype)
    {
        GameObject pawnObject = GameObject.Instantiate(pawnPrototype, team.GetPawnSpawnPosition(), team.GetSpawnRotation()) as GameObject;
        pawnObject.name = name;
        Pawn pawn = pawnObject.GetComponent<Pawn>();
        pawn.team = team;
        pawn.pawnType = ptype;
        GameObject bodyObject = null;
        switch(ptype)
        {
            case Pawn.PawnType.FAT: bodyObject = GameObject.Instantiate(roboBodyPrototype) as GameObject; break;
            case Pawn.PawnType.TALL: bodyObject = GameObject.Instantiate(roboBodyPrototype) as GameObject; break;
            case Pawn.PawnType.MED:   bodyObject = GameObject.Instantiate(roboBodyPrototype) as GameObject; break;
            default: break;
        }
        pawn.body = bodyObject != null ? bodyObject.GetComponent<ActorBody>() : null;
        if( pawn.body != null )
        {
            pawn.body.transform.SetParent(pawn.transform, false);
        }
        pawn.body.ApplyColor(team.teamColor);
        pawn.GoAlive();
        return pawn;
    }
    
    public Actor RequestTower(string name, Team team, bool giant)
    {
        Vector3 spawnPos = giant ? team.transform.position : team.GetTowerSpawnPosition();
        GameObject towerObject = GameObject.Instantiate(towerPrototype, spawnPos, team.GetSpawnRotation()) as GameObject;
        towerObject.name = name;
        Actor actor = towerObject.GetComponent<Actor>();
        actor.team = team;
        GameObject bodyObject = GameObject.Instantiate(giant ? giantTowerBodyPrototype : towerBodyPrototype) as GameObject;
        actor.body = bodyObject != null ? bodyObject.GetComponent<ActorBody>() : null;
        if( actor.body != null )
        {
            actor.body.transform.SetParent(actor.transform, false);
        }
        actor.GoAlive();
        return actor;
    }
    
    public Actor RequestBullet(Transform origin)
    {
        GameObject bulletObject = GameObject.Instantiate(bulletPrototype, origin.position, origin.rotation) as GameObject;
        bulletObject.name = bulletPrototype.name+"-"+Time.frameCount;
        Actor bullet = bulletObject.GetComponent<Actor>();
        return bullet;
    }
    
    
    public Actor RequestActor(GameObject prototype, Transform origin = null, bool parented = false)
    {
        GameObject itemObject;
        if( origin == null )
        {
            itemObject = GameObject.Instantiate(prototype, Vector3.zero, Quaternion.identity) as GameObject;
        }
        else if( parented )
        {
            itemObject = GameObject.Instantiate(prototype, Vector3.zero, Quaternion.identity) as GameObject;
            itemObject.transform.SetParent(origin, false);
        }
        else
        {
            itemObject = GameObject.Instantiate(prototype, origin.position, origin.rotation) as GameObject;
        }
        itemObject.name = prototype.name+"-"+Time.frameCount;
        Actor item = itemObject.GetComponent<Actor>();
        return item;
    }
    
    public void Add(Pawn pawn)
    {
        activePawns.Add(pawn);
    }
    public void Add(Actor actor)
    {
        activeActors.Add(actor);
    }
    
    public void Remove(Pawn pawn)
    {
        activePawns.Remove(pawn);
    }
    public void Remove(Actor actor)
    {
        activeActors.Remove(actor);
    }
    
    public void Update()
    {
        garbageTimer -= Time.deltaTime;
        if( garbageTimer < 0 )
        {
            garbageTimer = 5f;
            CleanGarbage();
        }
    }
    
    public void CleanGarbage()
    {
        for(int i=garbage.Count -1; i>=0 ; --i)
        {
            Destroy(garbage[i]);
        }
        garbage.Clear();
    }

}
