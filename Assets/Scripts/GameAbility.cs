using UnityEngine;

public class GameAbility : MonoBehaviour
{
    public enum AbilityState
	{
		NONE,
		STARTING,
		CHARGING,
		ACTIVE,
		RECOVERING,
		DOWNCOOLING,

	}
    
    public Actor owner;
    public Actor item;
    public Transform bone;
    
    public char abName = '-';
	public char abStateName = '-';
	public bool isPressed = false;
	public bool wasPressed = false;
	public AbilityState abState = AbilityState.NONE;
	public float chargeTime = -1f;
	public float lockTime = -1f;
	public float totalLockTime = -1f;
	public bool IsActive { get { return abState != AbilityState.NONE; } }

	public float CooldownPercent { 
		get 
		{
			if( abState == AbilityState.RECOVERING || abState == AbilityState.DOWNCOOLING )
			{
				return lockTime;
			}
			return 0f;
		}
	}
	
	public delegate void AbilityEvent(GameAbility ga);
	public AbilityEvent Startup;
	public AbilityEvent Charge;
	public AbilityEvent Activate;
	public AbilityEvent Recover;
	public AbilityEvent Cooldown;
	public AbilityEvent Finish;

	void Awake()
	{

	}

	
	public void ActionUpdate(float deltaTime, bool isPressed) 
	{
		this.isPressed = isPressed;
		if( abState == AbilityState.NONE && !isPressed )
		{
			return;
		}
		if( abState == AbilityState.CHARGING )
		{
			if( isPressed )
			{
				chargeTime += deltaTime;
			}
			else
			{
				lockTime = 0f;
			}
		}
		
		if( lockTime > 0f )
		{
			lockTime -= deltaTime;
		}
		if( lockTime < 0.001f )
		{
			Advance();
			wasPressed = isPressed;
		}
	}

	public void Advance()
	{
		switch(abState)
		{
			case AbilityState.NONE:  		SetState(AbilityState.STARTING); break;
			case AbilityState.STARTING: 	SetState(AbilityState.CHARGING); break;
			case AbilityState.CHARGING: 	SetState(AbilityState.ACTIVE); break;
			case AbilityState.ACTIVE: 		SetState(AbilityState.RECOVERING); break;
			case AbilityState.RECOVERING: 	SetState(AbilityState.DOWNCOOLING); break;
			case AbilityState.DOWNCOOLING: 	SetState(AbilityState.NONE); break;
		}
	}


	public void JumpToState(char stateName)
	{
		int attempts = 0;
		while(abStateName != stateName && attempts++ < 100)
		{
			Advance();
		}
	}

	public void SetState(AbilityState nextState)
	{
		if( abState == nextState )
		{
			return;
		}
		abState = nextState;
		switch(abState)
		{
			case AbilityState.STARTING: 
				abStateName = 'S';
				chargeTime = 0f;
				Startup(this);
				break;
			case AbilityState.CHARGING: 
				abStateName = 'C';
				chargeTime = 0f;
				Charge(this);
				break;
			case AbilityState.ACTIVE: 
				abStateName = 'A';
				Activate(this);
				break;
			case AbilityState.RECOVERING: 
				abStateName = 'R';
				Recover(this);
				break;
			case AbilityState.DOWNCOOLING: 
				abStateName = 'D';
				Cooldown(this);
				break;
			case AbilityState.NONE: 
				abStateName = '-';
				Finish(this);
				break;
		}
	}

	public void Interrupt()
	{
		SetState(AbilityState.NONE);
	}

	public void SetLock(float val)
	{
		lockTime = val;
		totalLockTime = val;
	}

}
