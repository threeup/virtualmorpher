using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

public class Pawn : Actor
{
    public enum PawnType
    {
        NONE,
        FAT,
        TALL,
        MED,
    }
    
    public PawnType pawnType = PawnType.NONE;
    
    public GameAbility leftHandAbility;
    public Actor leftHandItem;
    public GameAbility rightHandAbility;
    public Actor rightHandItem;
    public GameAbility wheelAbility;
    
    public List<IntVec2> path;
    public VectorLine line;
    
    public override void Awake()
    {
        GameAbility[] abs = GetComponents<GameAbility>();
        if( abs.Length > 0 )
        {
            leftHandAbility = abs[0];
        }
        if( abs.Length > 1 )
        {
            rightHandAbility = abs[1];
        }
        if( abs.Length > 2 )
        {
            wheelAbility = abs[2];
        }
        base.Awake();
    }
    
    public override void GoAlive()
    {
        if( leftHandAbility )
        {
            leftHandItem = Boss.RequestActor(Boss.actorWorld.cannonPrototype, body.leftHand, true);
            GameFactory.SetupBullet(leftHandAbility, this, leftHandItem);  
        }
        if( rightHandAbility )
        {
            rightHandItem = Boss.RequestActor(Boss.actorWorld.shieldPrototype, body.rightHand, true);  
            rightHandItem.body.transform.localScale = Vector3.one*0.25f;
            rightHandItem.body.coreCollision.GetComponent<Collider>().enabled = false;   
            GameFactory.SetupShield(rightHandAbility, this, rightHandItem);  
        }
        if( wheelAbility )
        {
            GameFactory.SetupNitro(wheelAbility, this, this);  
        }
        
        Boss.actorWorld.Add(this);
        
        base.GoAlive();
    }
    
    public override void GoDead()
    {
        Boss.actorWorld.Remove(this);
        base.GoDead();
    }
    
    
    public void DoButtons(float deltaTime, bool primary, bool secondary, bool tertiary)
    {
        if( leftHandAbility )
        {
            leftHandAbility.ActionUpdate(deltaTime, primary);
        }
        if( rightHandAbility )
        {
            rightHandAbility.ActionUpdate(deltaTime, secondary);
        }
        if( wheelAbility )
        {
            wheelAbility.ActionUpdate(deltaTime, tertiary);
        }
    }
    
    public void UpdateMotion(float deltaTime)
    {    
        if( path != null )
        {
            IntVec2 vec = motor.transform.position.ToIntVec2();
            while( path.Count > 0 && vec == path[0] )
            {
                path.RemoveAt(0);
            }
            if( path.Count > 0 )
            {
                IntVec2 next = path[0];
                motor.SetDestination(next.ToVector3());
                if( (line.points3.Count - 3) > path.Count )
                {
                    Texture lineTex = line.texture;
                    float lineWidth = line.lineWidth;
                    Color lc = line.color;
                    LineUtils.BuildLine(this, lineTex, lineWidth);
                    line.color = lc;
                }
            } 
            
        }
    }
    
    
}
