using UnityEngine;
using System.Collections;
public class GameEffect : MonoBehaviour 
{

	public enum EffectState
	{
		NONE,
		STARTING,
		ACTIVE,
	}
    
    public Actor actor;
    public Transform bone;


	public string displayName = "-";
	public char effName = '-';
	public char effStateName = '-';
	private EffectState effState = EffectState.NONE;
	public float tickTime = 0f;
	public float lockTime = -1f;
	public float totalLockTime = -1f;
	public float duration = 0f;
	public bool IsActive { get { return effState != EffectState.NONE; } } 
	private bool isPaused = false;
	public bool showProgress = false;
	public int priority = 5;
	
	public delegate void EffectEvent(GameEffect ge);
	public EffectEvent StartUp;
	public EffectEvent Activate;	
	public EffectEvent Finish;

	public delegate void EffectTick(GameEffect ge, float deltaTime);
	public EffectTick Tick;

	void Awake()
	{

	}

	
	public void EffectUpdate(float deltaTime) 
	{

		if( lockTime > 0f)
		{
			lockTime -= deltaTime;
		}
		if( lockTime < 0.001f )
		{
			if( IsActive )
			{
				Advance();
			}
		}
		if( !isPaused && IsActive )
		{
			Tick(this, deltaTime);
		}
	}


	public void Advance()
	{
		switch(effState)
		{
			case EffectState.NONE:  	SetState(EffectState.STARTING); break;
			case EffectState.STARTING: 	SetState(EffectState.ACTIVE); break;
			case EffectState.ACTIVE: 	SetState(EffectState.NONE); break;
		}
	}

	public void JumpToState(char stateName)
	{
		int attempts = 0;
		while(effStateName != stateName && attempts++ < 100)
		{
			Advance();
		}
	}

	public void SetState(EffectState nextState)
	{
		if( effState == nextState )
		{
			return;
		}
		effState = nextState;
		switch(effState)
		{
			case EffectState.STARTING: 
				effStateName = 'S';
				tickTime = 0f;
				StartUp(this);
				break;
			case EffectState.ACTIVE: 
				effStateName = 'A';
				Activate(this);
				break;
			case EffectState.NONE: 
				effStateName = '-';
				tickTime = 0f;
				Finish(this);
				break;
		}
	}


	public void Interrupt()
	{
		SetState(EffectState.NONE);
	}

	public void SetDuration(float val)
	{
		duration = val;
	}

	public void SetLock(float val)
	{
		lockTime = val;
		totalLockTime = val;
	}

	public void SetPause(bool val)
	{
		isPaused = val;
	}


}
