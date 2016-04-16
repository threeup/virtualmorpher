using UnityEngine;

public class ActorBody : MonoBehaviour
{
    
    [SerializeField]
    Transform leftArm;
    
    
    [SerializeField]
    Transform rightArm;
    
    
    [SerializeField]
    Transform wheel;
    
    float wheelSpeed = 80f;
    
    public void AimTowards(Vector3 direction)
    {
        leftArm.LookAt(direction);
        rightArm.LookAt(direction);
    }
    
    public void SetWheelSpeed(float amount)
    {
        wheelSpeed = amount;
    }
    
    void Update()
    {
        wheel.Rotate(-Vector3.forward, wheelSpeed * Time.deltaTime);
    }
    
}
