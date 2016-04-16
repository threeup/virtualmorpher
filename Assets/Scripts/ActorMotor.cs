using UnityEngine;

public class ActorMotor : MonoBehaviour, IMotor
{
    
    float motorSpeed = 0f;
    public float GetSpeed() { return motorSpeed; }
    Vector3 motorDirection = Vector3.forward;
    public Vector3 GetDirection() { return motorDirection; }
    
    void OnEnable()
    {
        MotorWorld.Ins.Add(this);
    }
    
    void OnDisable()
    {
        MotorWorld.Ins.Remove(this);
    }
    
    public void SetVelocity(Vector2 amount)
    {
        SetVelocity(new Vector3(amount.x, 0, amount.y));
    }
    
    public void SetVelocity(Vector3 amount)
    {
        float magn = amount.magnitude;
        motorSpeed = Mathf.Min(magn, 20);
        motorDirection = amount.normalized;    
    }
    
    public void UpdateMotion(float deltaTime)
    {
        Vector3 nextPosition = this.transform.position + GetDirection()*GetSpeed()*deltaTime;
        this.transform.position = nextPosition;
        
    }
}
