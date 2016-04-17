using System;
using UnityEngine;

[System.Serializable]
public struct IntVec2
{
    public int x;
    public int y;

    public IntVec2(int x, int y)
        : this()
    {
        this.x = x;
        this.y = y;
    }

    public IntVec2(float x, float y)
        : this((int)Mathf.RoundToInt(x), (int)Mathf.RoundToInt(y))
    {
    }
    
    public static bool operator == (IntVec2 a, IntVec2 b)
    {
        return a.x == b.x && a.y == b.y;
    }
    
    public static bool operator != (IntVec2 a, IntVec2 b)
    {
        return a.x != b.x || a.y != b.y;
    }
    
    public override bool Equals (System.Object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        IntVec2 other = (IntVec2)obj;
        if ((System.Object)other == null)
        {
            return false;
        }

        return x == other.x && y == other.y;
    }
    
    public override int GetHashCode()
    {
        return x*10000+ y;
    }
    
    public static IntVec2 operator - (IntVec2 a, IntVec2 b)
    {
        return new IntVec2(a.x-b.x,a.y-b.y);
    }
    public static IntVec2 operator + (IntVec2 a, IntVec2 b)
    {
        return new IntVec2(a.x+b.x,a.y+b.y);
    }
    
    public float Magnitude() 
    {
        return Mathf.Sqrt(x * x + y * y);
    }
    public int SqrMagnitude() 
    {
        return (x * x + y * y);
    }

    public void SetAngleRadians(float radians)
    {
        float length = Magnitude();
        this.x = (int)Mathf.RoundToInt(length * Mathf.Cos(radians));
        this.y = (int)Mathf.RoundToInt(length * Mathf.Sin(radians));
    }

    public void SetAngleDegrees(float degrees)
    {
        SetAngleRadians(degrees * (float)Mathf.PI / 180);
    }

    public float GetAngleRadians()
    {
        return Mathf.Atan2(this.y, this.x);
    }

    public float GetAngleDegrees()
    {
        return GetAngleRadians() * (180 / Mathf.PI);
    }
    
    public override string ToString()
    {
        return string.Format("{0} {1}", x, y);
    }
}

public static class IntVecExtensions
{

    public static IntVec2 ToIntVec2(this Vector3 vec)
    {
        return new IntVec2(vec.x, vec.z);
    }
    
    public static Vector3 ToVector3(this IntVec2 vec)
    {
        return new Vector3(vec.x, 0.25f, vec.y);
    }
    
    public static bool NotAdjacent(this IntVec2 a, IntVec2 b) 
    {
        return Mathf.Abs(a.x - b.x) > 1 || Mathf.Abs(a.y - b.y) > 1; 
    }
    
    public static IntVec2 StepTowards(this IntVec2 start, IntVec2 end) 
    {
        int diffX = end.x-start.x;
        int diffY = end.y-start.y;
        return new IntVec2(
            start.x + Mathf.Clamp(diffX, -1,1),
            start.y + Mathf.Clamp(diffY, -1,1));
    }
}