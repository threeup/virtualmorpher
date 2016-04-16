using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActorWorld : MonoBehaviour {

    public static ActorWorld Ins;
    
    public List<Pawn> activePawns = new List<Pawn>();
    public List<Actor> activeActors = new List<Actor>();
    
    public GameObject pawnPrototype;
    public GameObject bulletPrototype;
    public GameObject cannonPrototype;
    public GameObject shieldPrototype;
    public GameObject toolPrototype;
    public GameObject roboBodyPrototype;
    

	void Awake () 
    {
	    Ins = this;
	}
    
    public Pawn RequestPawn(string name, User owner, Pawn.PawnType ptype)
    {
        GameObject pawnObject = GameObject.Instantiate(pawnPrototype, owner.GetSpawnPosition(), owner.GetSpawnRotation()) as GameObject;
        pawnObject.name = name;
        Pawn pawn = pawnObject.GetComponent<Pawn>();
        pawn.owner = owner;
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
        pawn.GoAlive();
        return pawn;
    }
    
    public Actor RequestBullet(Transform origin)
    {
        GameObject bulletObject = GameObject.Instantiate(bulletPrototype, origin.position, origin.rotation) as GameObject;
        bulletObject.name = bulletPrototype.name+"-"+Time.frameCount;
        Actor bullet = bulletObject.GetComponent<Actor>();
        return bullet;
    }
    
    public Actor CreateItem(GameObject prototype, Transform origin)
    {
        GameObject itemObject = GameObject.Instantiate(prototype, Vector3.zero, Quaternion.identity) as GameObject;
        itemObject.transform.SetParent(origin, false);
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
    

}
