using UnityEngine;

public class ActorMotor : MonoBehaviour, IMotor
{
    public float topSpeed = 20f;
    float motorSpeed = 0f;
    public float GetSpeed() { return motorSpeed; }
    Vector3 motorDirection = Vector3.forward;
    public Vector3 GetDirection() { return motorDirection; }
    
    Vector3 destination = Vector3.zero;
    
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
        Vector3 diff = destination - this.transform.position;
        
        float magn = diff.magnitude;
        float nextStep = magn;
        nextStep = Mathf.Min(nextStep, topSpeed*deltaTime);
        motorSpeed = nextStep/deltaTime;
        motorDirection = diff.normalized;  
        
        Vector3 nextPosition = this.transform.position + GetDirection()*GetSpeed()*deltaTime;
        this.transform.position = nextPosition;
        
    }
}
