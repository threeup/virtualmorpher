using UnityEngine;

public class ActorBody : MonoBehaviour
{
    
    public Transform actionPoint;
    
    [SerializeField]
    Transform leftArm;
    
    public Transform leftHand;
    
    
    [SerializeField]
    Transform rightArm;
    
    public Transform rightHand;
    
    
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
        if( wheel )
        {
            wheel.Rotate(-Vector3.forward, wheelSpeed * Time.deltaTime);
        }
    }
    
}
