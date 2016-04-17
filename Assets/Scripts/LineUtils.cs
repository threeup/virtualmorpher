using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

public class LineUtils : MonoBehaviour 
{
    public static void BuildLine(Pawn pawn, Texture lineTex, float lineWidth)
    {        
        IntVec2 step = pawn.transform.position.ToIntVec2();
        List<Vector3> lineSteps = new List<Vector3>();
        lineSteps.Add(step.ToVector3());
        
        foreach(IntVec2 pathStep in pawn.path)
        {
            lineSteps.Add(pathStep.ToVector3());
        }
        
        VectorLine.Destroy(ref pawn.line);
        
        pawn.line = new VectorLine("Line"+pawn.name, lineSteps, lineTex, lineWidth, LineType.Continuous, Joins.Fill);
        pawn.line.color = pawn.team.teamColor;
        pawn.line.Draw3DAuto();
    }
}
