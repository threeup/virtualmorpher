using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActorWorld : MonoBehaviour {

    public static ActorWorld Ins;
    
    public List<Pawn> activePawns = new List<Pawn>();
    
    public GameObject pawnPrototype;
    public GameObject bodyCannonPrototype;
    public GameObject bodyShieldPrototype;
    public GameObject bodyToolPrototype;

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
            case Pawn.PawnType.CANNON: bodyObject = GameObject.Instantiate(bodyCannonPrototype) as GameObject; break;
            case Pawn.PawnType.SHIELD: bodyObject = GameObject.Instantiate(bodyShieldPrototype) as GameObject; break;
            case Pawn.PawnType.TOOL:   bodyObject = GameObject.Instantiate(bodyToolPrototype) as GameObject; break;
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
    
    public void Add(Pawn pawn)
    {
        activePawns.Add(pawn);
    }
    
    public void Remove(Pawn pawn)
    {
        activePawns.Remove(pawn);
    }
    

}
