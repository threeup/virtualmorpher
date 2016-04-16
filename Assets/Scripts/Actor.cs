using UnityEngine;

public class Actor : MonoBehaviour
{
    public ActorBody body;
    public ActorMotor motor;
    
    public virtual void GoAlive()
    {
        ActorWorld.Ins.Add(this);
    }
    
    public virtual void GoDead()
    {
        ActorWorld.Ins.Remove(this);
    }
    
    public void SetAxis(Vector2 axis)
    {
        motor.SetVelocity(axis);
    }
    
    public void SetVelocityForward(float amount)
    {
        Vector2 forward = new Vector2(this.transform.forward.x, this.transform.forward.z);
        motor.SetVelocity(forward * amount);
    }

}
