using UnityEngine;

public class Actor : MonoBehaviour
{
    public ActorBody body;
    public ActorMotor motor;
    
    public virtual void Awake()
    {
        motor = GetComponent<ActorMotor>();
    }
    
    public virtual void GoAlive()
    {
        Boss.actorWorld.Add(this);
    }
    
    public virtual void GoDead()
{
        Boss.actorWorld.Remove(this);
    }
    
    public void SetAxis(Vector2 axis)
    {
        
        motor.SetRelativeDestination(axis);
    }
    public void SetAxis(Vector3 axis)
    {
        Vector2 adjusted = new Vector2(axis.x, axis.z);
        motor.SetRelativeDestination(adjusted);
    }
    
    public void SetDestination(Vector2 axis)
    {
        motor.SetDestination(axis);
    }
    public void SetDestination(Vector3 axis)
    {
        Vector2 adjusted = new Vector2(axis.x, axis.z);
        motor.SetDestination(adjusted);
    }
    
    public void SetForwardDestination()
    {
        motor.SetRelativeDestination(this.transform.forward * 999f);
    }

}
