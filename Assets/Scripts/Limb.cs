using UnityEngine;

public class Limb : MonoBehaviour
{
    public enum LimbType
    {
        BALL = 0,
        BULLET = 1,
        SHIELD = 2,
        HEAD = 3,
        TORSO = 4,
        LEFTARM = 5,
        RIGHTARM = 6,
        WHEEL = 7,
    }
    
    public LimbType limbType;
    public Actor owner;
    public Collider coll;
    
    public delegate void BounceDelegate(Limb limb, Limb other);
    public BounceDelegate ProcessBounce;
    
    void Awake()
    {
        coll = GetComponent<Collider>();
        ProcessBounce = GameFactory.NoopBounce;
    }
    
    void Start()
    {
        owner = this.transform.root.gameObject.GetComponent<Actor>();
        if( owner == null )
        {
            Debug.Log(this.transform.root.gameObject, this);
        }
        else
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
        if( (int)limbType <= (int)otherLimb.limbType )
        {
            ProcessBounce(this, otherLimb);
        }
        else
        {
            otherLimb.ProcessBounce(otherLimb, this);
        }
    }
    
    public void OnTriggerEnter(Collider other)
    {
        Limb otherLimb = other.gameObject.GetComponent<Limb>();
        if( otherLimb == null || otherLimb.owner == owner)
        {
            return;
        }
        if( (int)limbType <= (int)otherLimb.limbType )
        {
            ProcessBounce(this, otherLimb);
        }
        else
        {
            otherLimb.ProcessBounce(otherLimb, this);
        }
    }
    
    
}
