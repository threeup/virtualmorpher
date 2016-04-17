using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Cheats : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Team northTeam = Boss.referee.northTeam;
            northTeam.Explode(northTeam.towers[0]);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Boss.actorWorld.CleanGarbage();
        }
    }  
}
