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
        ga.Activate = BulletActivate;
        
    }
    
    public static void SetupShield(GameAbility ga, Actor owner, Actor item)
    {
        SetupDefault(ga, owner, item);
        ga.abName = 'S';
        ga.Startup = ShieldStart;
        ga.Recover = ShieldFinish;   
    }
    
    public static void BulletActivate(GameAbility ga)
	{
		Actor bullet = ActorWorld.Ins.RequestBullet(ga.bone);
        bullet.SetForwardDestination(20f);
	}
    
    public static void ShieldStart(GameAbility ga)
	{
		
        ga.item.gameObject.SetActive(true); 
	}
    
    public static void ShieldFinish(GameAbility ga)
	{
		ga.item.gameObject.SetActive(false); 
	}
}
