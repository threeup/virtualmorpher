using UnityEngine;

public class Pawn : Actor
{
    public enum PawnType
    {
        NONE,
        FAT,
        TALL,
        MED,
    }
    
    public User owner;
    public PawnType pawnType = PawnType.NONE;
    
    public GameAbility leftHandAbility;
    public Actor leftHandItem;
    public GameAbility rightHandAbility;
    public Actor rightHandItem;
    
    void Awake()
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
    }
    
    public override void GoAlive()
    {
        if( leftHandAbility )
        {
            leftHandItem = ActorWorld.Ins.CreateItem(ActorWorld.Ins.cannonPrototype, body.leftHand);
            GameFactory.SetupBullet(leftHandAbility, this, leftHandItem);  
        }
        if( rightHandAbility )
        {
            rightHandItem = ActorWorld.Ins.CreateItem(ActorWorld.Ins.shieldPrototype, body.rightHand);    
            GameFactory.SetupShield(rightHandAbility, this, rightHandItem);  
        }
        
        ActorWorld.Ins.Add(this);
        
        base.GoAlive();
    }
    
    public override void GoDead()
    {
        ActorWorld.Ins.Remove(this);
        base.GoDead();
    }
    
    
    public void DoInput(float deltaTime, bool primary, bool secondary)
    {
        leftHandAbility.ActionUpdate(deltaTime, primary);
        rightHandAbility.ActionUpdate(deltaTime, secondary);
        
    }
}
