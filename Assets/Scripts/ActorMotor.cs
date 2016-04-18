using UnityEngine;

public class ActorMotor : MonoBehaviour, IMotor
{
    public Actor actor;
    public float defaultTopSpeed = 1f;
    public float currentTopSpeed = 1f;
    public float rotationSpeed = 2f;
    float motorSpeed = 0f;
    public float GetSpeed() { return motorSpeed; }
    Vector3 motorDirection = Vector3.forward;
    public Vector3 GetDirection() { return motorDirection; }
    
    CharacterController cc;
    Rigidbody rigbody;
    
    Vector3 destination = Vector3.zero;
    
    public Transform possessBone = null;
    public Actor possessActor = null;
    public Team possessTeam = null;
    
    float possessionLockTimer = -1f;
    float possessTeamTimer = -1f;
    
    Vector3 impactForce = Vector3.zero;
    float impactDuration = -1f;
    
    void Awake()
    {
        currentTopSpeed = defaultTopSpeed;
        actor = GetComponent<Actor>();
        cc = GetComponent<CharacterController>();
        rigbody = GetComponent<Rigidbody>();
    }
    void OnEnable()
    {
        Boss.Add(this);
        destination = this.transform.position;
    }
    
    void OnDisable()
    {
        Boss.Remove(this);
    }
    
    public void SetRelativeDestination(Vector2 amount)
    {
        SetRelativeDestination(new Vector3(amount.x, 0, amount.y));
    }
    
    public void SetRelativeDestination(Vector3 amount)
    {
        destination = this.transform.position + amount;  
    }
    
    public void SetDestination(Vector2 position)
    {
        destination = new Vector3(position.x, 0, position.y);  
    }
    public void SetDestination(Vector3 position)
    {
        destination = new Vector3(position.x, 0, position.z);  
    }
    
    
    
    public void UpdateMotion(float deltaTime)
    {
        if( possessionLockTimer > 0 )
        {
            possessionLockTimer -= deltaTime;
        } 
        if( impactDuration > 0 )
        {
            impactDuration -= deltaTime;
        }
        if( possessTeamTimer > 0 )
        {
            possessTeamTimer -= deltaTime;
            if( possessTeamTimer <= 0f )
            {
                SetPossessTeam(null);
            }
        }
        if( cc )
        { 
            destination.y = 0;
        }
        Vector3 diff = destination - this.transform.position;
        if( rigbody != null )
        {
            if( possessBone != null )
            {
                float offset = 1.7f;
                destination = possessBone.position + offset*possessBone.forward;
                destination.y = 1f;
                diff = destination - this.transform.position;
                if( diff.magnitude > 4f )
                {
                    Possess(null, null);
                }
                else
                {
                    rigbody.AddForce(diff.normalized*30f);    
                }
            }
            motorSpeed = rigbody.velocity.magnitude;
            return;
        }
        
        float magn = diff.magnitude;
        float nextStep = magn;
        nextStep = Mathf.Min(nextStep, currentTopSpeed*deltaTime);
        motorSpeed = nextStep/deltaTime;
        motorDirection = diff.normalized;  
        
        Vector3 delta = GetDirection()*GetSpeed()*deltaTime;
        if( cc )
        {
            if( impactDuration > 0f )
            {
                delta = impactForce*deltaTime;
            }
            cc.Move(delta);
        }
        else if( rigbody )
        {
            rigbody.MovePosition(this.transform.position + delta);
        }
        else
        {
            this.transform.position = this.transform.position + delta;    
        }
        
        
        float step = rotationSpeed * deltaTime;
        motorDirection.y = 0;
        Vector3 nextDir = Vector3.RotateTowards(this.transform.forward, motorDirection, step, 0.0F);
        if( cc )
        {
            this.transform.rotation = Quaternion.LookRotation(nextDir);
        }
        else if( rigbody )
        {
            rigbody.MoveRotation(Quaternion.LookRotation(nextDir));
        }
        else
        {
            this.transform.rotation = Quaternion.LookRotation(nextDir);
        }

        
    }
    
    public void RigidGo(Vector3 endpoint)
    {
        if( cc )
        {
            impactForce = endpoint - this.transform.position;
            impactDuration = 0.2f;
        }
        else
        {
            rigbody.AddForce(endpoint - this.transform.position, ForceMode.Impulse);
        }
    }
    
    public void RigidAdd(Vector3 added)
    {
        if( cc )
        {
            impactForce = added;
            impactDuration = 0.2f;
        }
        else
        {
            rigbody.AddForce(added, ForceMode.Impulse);
        }
    }
    
    public void Possess(Transform possessBone, Actor possessActor)
    {
        if( this.possessBone != possessBone && possessionLockTimer < 0f )
        { 
            this.possessBone = possessBone;
            this.possessActor = possessActor;
            if( possessBone )
            {
                SetPossessTeam(possessActor.team);
                Boss.referee.TempFloater("Grabbed by "+possessActor.name);
                actor.body.transform.localScale = 0.5f*Vector3.one;
            }
            else
            {
                Boss.referee.TempFloater("Dropped!");
                actor.body.transform.localScale = 1f*Vector3.one;
                possessTeamTimer = 2f;
            }
        }
    }
    
    public void SetPossessionLock(bool val)
    {
        if( val )
        {
            possessionLockTimer = 2f;
        }
    }
    
    public void SetPossessTeam(Team team)
    {
        if( possessTeam == team )
        {
            return;
        }
        possessTeam = team;
        if( team )
        {
            actor.body.ApplyColor(Color.Lerp(Color.white, team.teamColor, 0.5f));
            actor.body.ApplyEmission(Color.Lerp(Color.black, team.teamColor, 0.5f));
        }
        else
        {
            actor.body.ApplyColor(Color.white);
            actor.body.ApplyEmission(Color.Lerp(Color.black, Color.white, 0.5f));
            possessTeamTimer = 2f;
        }
    }
}
