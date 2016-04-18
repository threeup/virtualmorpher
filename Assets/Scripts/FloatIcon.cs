using UnityEngine;
using UnityEngine.UI;
using System;

public class FloatIcon : MonoBehaviour
{
    public Transform target;
    public GameAbility targetAbility;
    public float offset;
    public Image image;
    public Text textField;
    public UICtrl uiCtrl;
    public RectTransform rectTransform;
    
    bool doubled = false;
    
    public static Color inactiveColor = new Color(1f,1f,1f,0.2f);
    public static Color chargedColor = new Color(0.3f,1f,0.3f,0.6f);
    public static Color activeColor = new Color(1f,1f,1f,1f);
    public static Color cooldownColor = new Color(0f,0.3f,1f,0.6f);
    void Awake()
    {
        rectTransform = this.transform as RectTransform;
    }
    
    public void Hide()
    {
        target = null;
        gameObject.SetActive(false);
        
    }
    
    public void Update()
    {
        if(target != null)
        {
            Vector2 floaterPos = uiCtrl.cam.WorldToViewportPoint(target.position);
            Vector2 anchoredPos = new Vector2(0,60);
            anchoredPos.x += offset;
            rectTransform.anchoredPosition = anchoredPos;;
            rectTransform.anchorMin = floaterPos;
            rectTransform.anchorMax = floaterPos;
            
            if( targetAbility )
            {
                switch(targetAbility.abState)
                {
                    default:
                    case GameAbility.AbilityState.NONE:
                        image.color = inactiveColor;
                        textField.text = "";
                        break;
                    case GameAbility.AbilityState.STARTING:
                    case GameAbility.AbilityState.CHARGING:
                        image.color = chargedColor;
                        textField.text = "*";
                        break;
                    case GameAbility.AbilityState.ACTIVE:
                        image.color = activeColor;
                        textField.text = "_";
                        break;
                    case GameAbility.AbilityState.RECOVERING:
                    case GameAbility.AbilityState.DOWNCOOLING:
                        image.color = cooldownColor;
                        int rounded = (int)Mathf.Round(targetAbility.lockTime*10);
                        textField.text = (rounded/10f).ToString();
                        break;
                }
            }
        }
    }
    
    public void SetDoubleScale(bool val)
    {
           if(val == doubled)
           {
               return;
           }
           doubled = val;
           if( doubled )
           {
               offset *= 2;
               this.transform.localScale *= 2;
           }
           else
           {
               offset *= 0.5f;
               this.transform.localScale *= 0.5f;
           }
    }
    
}
