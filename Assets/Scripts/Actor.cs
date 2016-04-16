using UnityEngine;

public class Actor : MonoBehaviour
{
    public ActorBody body;
    ActorMotor motor;
    
    void Awake()
    {
        motor = GetComponent<ActorMotor>();
    }
    
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
    public void SetAxis(Vector3 axis)
    {
        Vector2 adjusted = new Vector2(axis.x, axis.z);
        motor.SetVelocity(adjusted);
    }
    
    public void SetVelocityForward(float amount)
    {
        
        motor.SetVelocity(this.transform.forward * amount);
    }

}
