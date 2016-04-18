using UnityEngine;

public class Actor : MonoBehaviour
{
    public ActorBody body;
    public ActorMotor motor;
    public Team team;
    GameObject hat;
    
    public float GetSpeed() { return motor.GetSpeed(); }
    
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
        Destroy(this.hat);
        this.hat = null;
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
    
    public void RegisterLimb(Limb limb, Limb.LimbType limbType)
    {
        switch(limbType)
        {
            case Limb.LimbType.TOWER: limb.ProcessBounce = GameFactory.TowerBounce; break;
            case Limb.LimbType.BULLET: limb.ProcessBounce = GameFactory.BulletBounce; break;
            case Limb.LimbType.SHIELD: limb.ProcessBounce = GameFactory.ShieldBounce; break;
            case Limb.LimbType.BALL: limb.ProcessBounce = GameFactory.BallBounce; break;
            case Limb.LimbType.HEAD: limb.ProcessBounce = GameFactory.HeadBounce; break;
            case Limb.LimbType.TORSO: limb.ProcessBounce = GameFactory.TorsoBounce; break;
            case Limb.LimbType.LEFTARM: limb.ProcessBounce = GameFactory.LeftArmBounce; break;
            case Limb.LimbType.RIGHTARM: limb.ProcessBounce = GameFactory.RightArmBounce; break;
            case Limb.LimbType.WHEEL: limb.ProcessBounce = GameFactory.WheelBounce; break;
            default: break;
        }
    }
    
    public void SetHat(GameObject hat)
    {
        this.hat = hat;
        hat.transform.parent = body.head;
        hat.transform.localPosition = Vector3.up*0.5f;
    }
    

}
