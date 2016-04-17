using UnityEngine;

public class ActorMotor : MonoBehaviour, IMotor
{
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
    
    float possessionLockTimer = -1f;
    
    void Awake()
    {
        currentTopSpeed = defaultTopSpeed;
        cc = GetComponent<CharacterController>();
        rigbody = GetComponent<Rigidbody>();
    }
    void OnEnable()
    {
        MotorWorld.Ins.Add(this);
        destination = this.transform.position;
    }
    
    void OnDisable()
    {
        MotorWorld.Ins.Remove(this);
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
        Vector3 diff = destination - this.transform.position;
        if( rigbody != null )
        {
            if( possessBone != null )
            {
                float offset = 1f;
                destination = possessBone.position + offset*possessBone.forward;
                destination.y = 1f;
                diff = destination - this.transform.position;
                if( diff.magnitude > 4f )
                {
                    Debug.Log("Drop"+diff.magnitude);
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
        
        diff.y = 0; 
        float magn = diff.magnitude;
        float nextStep = magn;
        nextStep = Mathf.Min(nextStep, currentTopSpeed*deltaTime);
        motorSpeed = nextStep/deltaTime;
        motorDirection = diff.normalized;  
        
        Vector3 delta = GetDirection()*GetSpeed()*deltaTime;
        if( cc )
        {
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
        rigbody.AddForce(endpoint - this.transform.position, ForceMode.Impulse);
    }
    
    public void RigidAdd(Vector3 added)
    {
        rigbody.AddForce(added, ForceMode.Impulse);
    }
    
    public void Possess(Transform possessBone, Actor possessActor)
    {
        if( this.possessBone != possessBone && possessionLockTimer < 0f )
        { 
            this.possessBone = possessBone;
            if( possessBone )
            {
                Referee.Ins.TempFloater("Grabbed by "+possessActor+" "+possessBone);
            }
            else
            {
                Referee.Ins.TempFloater("Dropped!");
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
}
