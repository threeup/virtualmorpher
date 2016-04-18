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
    
    
    public static void TowerBounce(Limb src, Limb other, Vector3 diff)
    {
        if( other.limbType != Limb.LimbType.BALL )
        {
            return;
        }
        float bounceAway = 3f;
        
        ActorMotor otherMotor = other.owner.motor;
        if( otherMotor.possessTeam != null && 
            otherMotor.possessTeam != other.owner.team )
        {
            src.owner.team.ExplodeTower(src.owner);
        }
        
        Vector3 bounceOut = Vector3.up*10f-src.transform.position;
        otherMotor.RigidAdd(bounceOut.normalized*bounceAway);
    }
    
    public static void BallBounce(Limb src, Limb other, Vector3 diff)
    {
        ActorMotor srcMotor = src.owner.motor;
        if( srcMotor.possessActor == other.owner )
        {
            return;
        }
        
        float catchSpeed = 5f;
        float bounceAway = 6f;
        
        float dotp = Vector3.Dot(other.owner.transform.forward, -diff.normalized);
        if( dotp > 0.2 && srcMotor.GetSpeed() < catchSpeed )
        {
            //check to see if it is a pop-up
            srcMotor.Possess(other.owner.transform, other.owner);
            return;
        } 
        else
        {
            //Debug.Log("cant "+dotp+" "+srcMotor.GetSpeed());
        }
        srcMotor.RigidAdd(-diff.normalized*bounceAway);
    }
    public static void BulletBounce(Limb src, Limb other, Vector3 diff)
    {
        float bounceAway = 2f;
        ActorMotor otherMotor = other.owner.motor;
        otherMotor.RigidAdd(diff.normalized*bounceAway);
    }
    public static void ShieldBounce(Limb src, Limb other, Vector3 diff)
    {
        float bounceAway = 4f;
        if( other.limbType == Limb.LimbType.BALL)
        {
            ActorMotor srcMotor = src.owner.motor;
            float slowSpeed = 4f;
            bool isThrow = srcMotor.GetSpeed() < slowSpeed;
            if( isThrow )
            {
                other.owner.motor.SetPossessTeam(src.owner.team);
            }
            if( other.owner.motor.possessActor != null )
            {
                other.owner.motor.Possess(null, null);
            }
        }
        
        //dont use origin of shield
        diff = other.transform.position - src.owner.transform.position;
        
        ActorMotor otherMotor = other.owner.motor;
        otherMotor.RigidAdd(diff.normalized*bounceAway);
    }
    public static void HeadBounce(Limb src, Limb other, Vector3 diff)
    {
        float bounceAway = 2f;
        ActorMotor otherMotor = other.owner.motor;
        otherMotor.RigidAdd(diff.normalized*bounceAway);
    }
    public static void TorsoBounce(Limb src, Limb other, Vector3 diff)
    {
        float bounceAway = 2f;
        ActorMotor otherMotor = other.owner.motor;
        otherMotor.RigidAdd(diff.normalized*bounceAway);
    }
    public static void LeftArmBounce(Limb src, Limb other, Vector3 diff)
    {
        float bounceAway = 2f;
        ActorMotor otherMotor = other.owner.motor;
        otherMotor.RigidAdd(diff.normalized*bounceAway);
    }
    public static void RightArmBounce(Limb src, Limb other, Vector3 diff)
    {
        float bounceAway = 2f;
        ActorMotor otherMotor = other.owner.motor;
        otherMotor.RigidAdd(diff.normalized*bounceAway);
    }
    public static void WheelBounce(Limb src, Limb other, Vector3 diff)
    {
        float bounceAway = 2f;
        ActorMotor otherMotor = other.owner.motor;
        otherMotor.RigidAdd(diff.normalized*bounceAway);
    }
    
    public static void NoopBounce(Limb src, Limb other, Vector3 diff)
    {
        
    }
}
