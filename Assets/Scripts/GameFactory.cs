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
        ga.item.gameObject.SetActive(true);
        ga.lockTime = 2f; 
	}
    public static void ShieldContinue(GameAbility ga)
	{
        ga.lockTime = Mathf.Max(1f,ga.chargeTime);
	}
    
    public static void ShieldFinish(GameAbility ga)
	{
		ga.item.gameObject.SetActive(false); 
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
}
