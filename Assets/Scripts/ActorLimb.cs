using UnityEngine;

public class ActorLimb : MonoBehaviour
{
    public enum LimbType
    {
        HEAD,
        TORSO,
        LEFTARM,
        RIGHTARM,
        WHEEL,
    }
    
    public LimbType limbType;
    
    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision");
    }
}
