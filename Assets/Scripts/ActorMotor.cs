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
    
    Rigidbody rigbody;
    
    Vector3 destination = Vector3.zero;
    
    void Awake()
    {
        currentTopSpeed = defaultTopSpeed;
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
        if( rigbody == null || rigbody.isKinematic )
        {
            Vector3 diff = destination - this.transform.position;
            
            float magn = diff.magnitude;
            float nextStep = magn;
            nextStep = Mathf.Min(nextStep, currentTopSpeed*deltaTime);
            motorSpeed = nextStep/deltaTime;
            motorDirection = diff.normalized;  
            
            Vector3 nextPosition = this.transform.position + GetDirection()*GetSpeed()*deltaTime;
            this.transform.position = nextPosition;
            
            float step = rotationSpeed * deltaTime;
            Vector3 nextDir = Vector3.RotateTowards(this.transform.forward, motorDirection, step, 0.0F);
            this.transform.rotation = Quaternion.LookRotation(nextDir);
        }

        
    }
}
