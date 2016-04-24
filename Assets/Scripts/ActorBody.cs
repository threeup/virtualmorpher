using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActorBody : MonoBehaviour
{
    
    public Transform actionPoint;
    public Transform coreCollision;
    public Transform head;
    
    [SerializeField]
    Transform leftArm;
    
    public Transform leftHand;
    
    
    [SerializeField]
    Transform rightArm;
    
    public Transform rightHand;
    
    
    [SerializeField]
    Transform wheel;
    
    float wheelSpeed = 80f;
    
    Material primaryMaterial;
    Material secondaryMaterial;
    public List<Renderer> secondary = new List<Renderer>();
    public List<Renderer> excluded = new List<Renderer>();
    
    void Start()
    {
        if( primaryMaterial == null )
        {
            SetupMaterials();
        }
    }
    
    void SetupMaterials()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if( renderers.Length > 0 )
        {
            for(int i=0; i<renderers.Length; ++i)
            {
                if( excluded.Contains(renderers[i]) )
                {
                    
                }
                else if( secondary.Contains(renderers[i]) )
                {
                    if( secondaryMaterial == null )
                    {
                        secondaryMaterial = renderers[i].material;
                    }
                    renderers[i].sharedMaterial = secondaryMaterial;
                }
                else
                {
                    if( primaryMaterial == null )
                    {
                        primaryMaterial = renderers[i].material;
                    }
                    renderers[i].sharedMaterial = primaryMaterial;
                }
            }
        }
    }
    
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
    
    public void ApplyColor(Color color)
    {
        if( primaryMaterial == null )
        {
            SetupMaterials();
        }
        if( primaryMaterial )
        {
            primaryMaterial.SetColor("_Color",color);
        }
        if( secondaryMaterial )
        {
            secondaryMaterial.SetColor("_EmissionColor",Color.Lerp(Color.black, color, 0.5f));
        }
    }
    
    public void ApplyEmission(Color color)
    {
        if( primaryMaterial == null )
        {
            SetupMaterials();
        }
        if( primaryMaterial )
        {
            primaryMaterial.SetColor("_EmissionColor",color);
        }
        if( secondaryMaterial )
        {
            secondaryMaterial.SetColor("_EmissionColor",Color.Lerp(Color.black, color, 0.5f));
        }
    }
    
}
