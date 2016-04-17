using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Boss
{
    public static List<CamCtrl> camCtrls = new List<CamCtrl>();
    public static List<UICtrl> uiCtrls = new List<UICtrl>();
    public static ActorWorld actorWorld;
    public static MotorWorld motorWorld;
    public static MenuCtrl menuCtrl;
    public static Referee referee;
    
    public static Pawn RequestPawn(string name, Team team, Pawn.PawnType ptype)
    {
        return actorWorld.RequestPawn(name, team, ptype);
    }
    public static Actor RequestTower(string name, Team team, bool giant)
    {
        return actorWorld.RequestTower(name, team, giant);
    }
    public static Actor RequestActor(GameObject prototype, Transform origin = null, bool parented = false)
    {
        return actorWorld.RequestActor(prototype, origin, parented);
    }
    public static GameObject RequestObject(GameObject prototype, Transform origin = null)
    {
        if( origin == null )
        {
            return GameObject.Instantiate(prototype) as GameObject;
        }
        else
        {
            return GameObject.Instantiate(prototype, origin.position, origin.rotation) as GameObject;
        }
    }
    public static CamCtrl GetCamCtrl(int idx)
    {
        idx = Mathf.Min(idx, camCtrls.Count-1);
        if( idx >= 0 )
        {
            return camCtrls[idx];
        }
        return null;
    }

    public static void ShowFloater(Transform target, int line, string text, Action<bool> onClick)
    {
        foreach(UICtrl uiCtrl in uiCtrls)
        {
            uiCtrl.ShowFloater(target, line, text, onClick);
        }
    }
    
    public static void HideFloater(Transform target, int line)
    {
        foreach(UICtrl uiCtrl in uiCtrls)
        {
            uiCtrl.HideFloater(target, line);
        }
    }
    public static void HideFloater(Transform target)
    {
        foreach(UICtrl uiCtrl in uiCtrls)
        {
            uiCtrl.HideFloater(target);
        }
    }
    
    public static void Add(IMotor motor)
    {
        if( motorWorld != null && !motorWorld.Contains(motor) )
        {
            motorWorld.Add(motor);
        }
    }
    public static void Remove(IMotor motor)
    {
        if( motorWorld != null )
        {
            motorWorld.Remove(motor);
        }
    }
    
    public static void AddGarbage(GameObject go)
    {
        actorWorld.garbage.Add(go);
    }
    
    
}
