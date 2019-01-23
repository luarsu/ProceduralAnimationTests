using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public static class Bezier
{

    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        //To get the curve we have to lerp between the lerp of p0 to p1 and the lerp of p1 to p2
        //We cal also use the cuadratic formula instead of the two lerps B(t) = (1 - t)2 P0 + 2 (1 - t) t P1 + t2 P2.
        //return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);

        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return oneMinusT * oneMinusT * p0 +  2f * oneMinusT * t * p1 +  t * t * p2;
    }
}
