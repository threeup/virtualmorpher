using UnityEngine;

public class Pawn : MonoBehaviour
{
    public enum PawnType
    {
        NONE,
        TOOL,
        SHIELD,
        CANNON,
    }
    
    public User owner;
    public ActorBody body;
    public PawnType pawnType = PawnType.NONE;
    public ActorMotor motor;
    
    public void GoAlive()
    {
        ActorWorld.Ins.Add(this);
    }
    
    public void GoDead()
    {
        ActorWorld.Ins.Remove(this);
    }
    
    public void SetAxis(Vector2 axis)
    {
        motor.SetVelocity(axis);
    }
    
    public void DoInput(bool primary, bool secondary)
    {
        //
    }
}
