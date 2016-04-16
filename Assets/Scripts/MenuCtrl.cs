using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuCtrl : MonoBehaviour {

    public static MenuCtrl Ins = null;
    
    public GameObject sideSelectOverlay;
    public GameObject endOfGameOverlay;
    
    void Awake()
    {
        Ins = this;
    }
    
    public void GoSideSelect(bool enabled)
    {
        sideSelectOverlay.gameObject.SetActive(enabled);
    } 
    
    public void GoEndOfGame(bool enabled)
    {
        endOfGameOverlay.gameObject.SetActive(enabled);
    }

}
