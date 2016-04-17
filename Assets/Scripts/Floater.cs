using UnityEngine;
using UnityEngine.UI;
using System;

public class Floater : MonoBehaviour
{
    public Transform target;
    public int line;
    public Text textField;
    public CamCtrl camCtrl;
    public RectTransform rectTransform;
    
    public Action<bool> onClick;
    public bool hideOnClick = true;
    
    void Awake()
    {
        rectTransform = this.transform as RectTransform;
        onClick = Noop;
    }
    
    public void Hide()
    {
        target = null;
        onClick = Noop;
        gameObject.SetActive(false);
        
    }
    
    public void Update()
    {
        if(target != null)
        {
            Vector2 floaterPos = camCtrl.cam.WorldToViewportPoint(target.position);
            Vector2 anchoredPos = new Vector2(0,60);
            anchoredPos.y -= line*20;
            rectTransform.anchoredPosition = anchoredPos;;
            rectTransform.anchorMin = floaterPos;
            rectTransform.anchorMax = floaterPos;
        }
    }
    
    public void Noop(bool value)
    {
        
    }
    
    public void Click()
    {
        onClick(true);
        if( hideOnClick )
        {
            Hide();
        }
    }
}
