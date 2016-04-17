using UnityEngine;

public class Decoration : MonoBehaviour
{
    Material primaryMaterial;
    public Team team;
    public float alpha = 0.5f;
    
    void Start()
    {
        
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if( renderers.Length > 0 )
        {
            primaryMaterial = renderers[0].material;
            foreach(Renderer rend in renderers)
            {
                rend.sharedMaterial = primaryMaterial;
            }
        }
        if( team )
        {
            ApplyColor(team.teamColor);
        }
    }
        
    public void ApplyColor(Color color)
    {
        color.a = alpha;
        //primaryMaterial.SetColor("_MainColor",color);
        primaryMaterial.SetColor("_Color",color);
    }
    
}
