using UnityEngine;

public class GameFactory : MonoBehaviour
{
    
    public static void Noop(GameAbility ga)
	{
		return;
	}
    
    public static void Noop(GameEffect ga)
	{
		return;
	}

	public static void NoopTick(GameEffect ge, float deltaTime)
	{
		return;
	}
    
    public static void SetupDefault(GameAbility ga, Actor owner, Actor item)
    {
        ga.owner = owner;
        ga.item = item;
        ga.bone = item.body.actionPoint;
        ga.Startup = Noop;
        ga.Charge = Noop;
        ga.Activate = Noop;
        ga.Recover = Noop;
        ga.Cooldown = Noop;
        ga.Finish = Noop;
    }
    
    public static void SetupBullet(GameAbility ga, Actor owner, Actor item)
    {
        SetupDefault(ga, owner, item);
        ga.abName = 'B';
        ga.Charge = BulletCharge;
        ga.Activate = BulletActivate;
        ga.Cooldown = ThreeSecond;
        
    }
    
    public static void SetupShield(GameAbility ga, Actor owner, Actor item)
    {
        SetupDefault(ga, owner, item);
        ga.abName = 'S';
        ga.Charge = ShieldStart;
        ga.Activate = ShieldContinue;
        ga.Recover = ShieldFinish;  
        ga.Cooldown = FiveSecond; 
    }
    
    public static void SetupNitro(GameAbility ga, Actor owner, Actor item)
    {
        SetupDefault(ga, owner, item);
        ga.abName = 'N';
        ga.Charge = TwoSecond;
        ga.Activate = NitroStart;
        ga.Recover = NitroFinish;   
    }
    
    public static void OneSecond(GameAbility ga)
	{
		ga.lockTime = 2f;
	}
    
    public static void TwoSecond(GameAbility ga)
	{
		ga.lockTime = 2f;
	}
    
    public static void ThreeSecond(GameAbility ga)
	{
		ga.lockTime = 3f;
	}
    
    public static void FiveSecond(GameAbility ga)
	{
		ga.lockTime = 3f;
	}
    
    public static void BulletCharge(GameAbility ga)
	{
		ga.lockTime = 2f;
    }
    
    public static void BulletActivate(GameAbility ga)
	{
		Actor bullet = Boss.actorWorld.RequestBullet(ga.bone);
        bullet.SetForwardDestination();
        ActorMotor motor = bullet.motor;
        float chargeMaxTime = 2f;
        float factor = Mathf.Lerp(0.5f, 1.5f, ga.chargeTime/chargeMaxTime);
		motor.currentTopSpeed = motor.defaultTopSpeed*factor; 
	}
    
    public static void ShieldStart(GameAbility ga)
	{
        ga.item.body.coreCollision.GetComponent<Collider>().enabled = true;
        ga.item.body.transform.localScale = Vector3.one*1f;
        ga.lockTime = 2f; 
	}
    public static void ShieldContinue(GameAbility ga)
	{
        ga.lockTime = Mathf.Max(1f,ga.chargeTime);
        ga.item.body.transform.localScale = Vector3.one*1.5f;
	}
    
    public static void ShieldFinish(GameAbility ga)
	{
        ga.item.body.coreCollision.GetComponent<Collider>().enabled = false;
        ga.item.body.transform.localScale = Vector3.one*0.25f;
	}
    
    public static void NitroStart(GameAbility ga)
	{
		ActorMotor motor = ga.owner.motor;
        motor.currentTopSpeed = motor.defaultTopSpeed * 1.6f;
        ga.lockTime = ga.chargeTime*3f;
	}
    
    public static void NitroFinish(GameAbility ga)
	{
        ActorMotor motor = ga.owner.motor;
		motor.currentTopSpeed = motor.defaultTopSpeed; 
	}
    
    
    public static void BallBounce(Limb victim, Limb other)
    {
        
        Vector3 diff = other.transform.position - victim.transform.position;
        
        float bounceAway = 3f;
        bool catchAttempt = false;
        switch(other.limbType)
        {
            case Limb.LimbType.SHIELD:
                bounceAway = 12f;
                break;
            case Limb.LimbType.BULLET:
                bounceAway = 3f;
                break;
            case Limb.LimbType.HEAD:
            case Limb.LimbType.TORSO:
            case Limb.LimbType.LEFTARM:
            case Limb.LimbType.RIGHTARM:
            case Limb.LimbType.WHEEL:
                catchAttempt = true;
                break;
            default:
                break;
        }
        ActorMotor victimMotor = victim.owner.motor;
        float catchSpeed = 5f;
        if( catchAttempt )
        {   
            if( victimMotor.possessActor == other.owner )
            {
                return;
            }
            float dotp = Vector3.Dot(other.owner.transform.forward, -diff.normalized);
            if( dotp > 0.2 && victimMotor.GetSpeed() < catchSpeed )
            {
                
                victimMotor.Possess(other.owner.transform, other.owner);
                return;
            } 
            else
            {
                Debug.Log("cant "+dotp+" "+victimMotor.GetSpeed());
            }
        }
        Debug.Log("BallBounce "+other.limbType);
        victimMotor.RigidAdd(-diff*bounceAway);
    }
    public static void BulletBounce(Limb victim, Limb other)
    {
        Debug.Log("BulletBounce "+other.limbType);
    }
    public static void ShieldBounce(Limb victim, Limb other)
    {
        Debug.Log("ShieldBounce "+other.limbType);
    }
    public static void HeadBounce(Limb victim, Limb other)
    {
        Debug.Log("HeadBounce "+other.limbType);
    }
    public static void TorsoBounce(Limb victim, Limb other)
    {
        Debug.Log("TorsoBounce "+other.limbType);
    }
    public static void LeftArmBounce(Limb victim, Limb other)
    {
        Debug.Log("LeftArmBounce "+other.limbType);
    }
    public static void RightArmBounce(Limb victim, Limb other)
    {
        Debug.Log("RightArmBounce "+other.limbType);
    }
    public static void WheelBounce(Limb victim, Limb other)
    {
        Debug.Log("WheelBounce "+other.limbType);
    }
    
    public static void NoopBounce(Limb limb, Limb other)
    {
        
    }
}
