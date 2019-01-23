using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve : MonoBehaviour {

    public Vector3[] points;

    public BezierCurve()
    {
        points = new Vector3[]{ Vector3.zero, Vector3.zero, Vector3.zero };
    }

    //Get point from a cuadratic bezier curve
    public Vector3 GetPoint(float t)
    {
        return transform.TransformPoint(Bezier.GetPoint(points[0], points[1], points[2], t));

    }
}
