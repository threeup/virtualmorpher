using UnityEngine;

public class Limb : MonoBehaviour
{
    public enum LimbType
    {
        TOWER = 0,
        BULLET = 1,
        SHIELD = 2,
        BALL = 3,
        HEAD = 4,
        LEFTARM = 5,
        RIGHTARM = 6,
        WHEEL = 7,
        TORSO = 8, 
    }
    
    public LimbType limbType;
    public Actor owner;
    public Collider coll;
    
    public delegate void BounceDelegate(Limb limb, Limb other, Vector3 diff);
    public BounceDelegate ProcessBounce;
    
    void Awake()
    {
        coll = GetComponent<Collider>();
        ProcessBounce = GameFactory.NoopBounce;
    }
    
    void Start()
    {
        owner = this.transform.root.gameObject.GetComponent<Actor>();
        if( owner != null )
        {
            owner.RegisterLimb(this, limbType);    
        }
        
    }
    
    public void OnCollisionEnter(Collision collision)
    {
        Limb otherLimb = collision.collider.gameObject.GetComponent<Limb>();
        if( otherLimb == null || otherLimb.owner == owner)
        {
            return;
        }
        Vector3 diff = otherLimb.transform.position - this.transform.position;
        if( (int)limbType <= (int)otherLimb.limbType )
        {
            ProcessBounce(this, otherLimb, diff);
        }
        else
        {
            otherLimb.ProcessBounce(otherLimb, this, diff);
        }
    }
    
    public void OnTriggerEnter(Collider other)
    {
        Limb otherLimb = other.gameObject.GetComponent<Limb>();
        if( otherLimb == null || otherLimb.owner == owner)
        {
            return;
        }
        if( otherLimb.owner == null )
        {
            Debug.LogError("Detached limb "+other);
            return;
        }
        Vector3 diff = otherLimb.transform.position - this.transform.position;
        
        if( (int)limbType <= (int)otherLimb.limbType )
        {
            ProcessBounce(this, otherLimb, diff);
        }
        else
        {
            otherLimb.ProcessBounce(otherLimb, this, diff);
        }
    }
    
    
}
