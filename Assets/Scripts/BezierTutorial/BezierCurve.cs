using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    //Points that represent end, begining and middle point of the limb
    public Vector3[] points;
    public GameObject p0, p1, p2;

    //Float that represents the percentage between the end and begining of the lemb where the middle point (knee) will be situated
    [Range(0.0f,1.0f)]
    public float kneeDistance = 0.5f;

    //Max length of the limb, or max distance to which the end of the limb can be of the beginning
    public float maxDistance;

    //Layer mask for the raycast of get point
    public LayerMask layerM;

    //Previous position of the end limb (p2) to make sure that is clamped to the latest position where it didnt surpase the max distance 
    private Vector3 prevPos;

    //Vector direction in which the height of the knee (with respect to the vector between p0 and p2) will be moved
    private Vector3 kneeDirection = Vector3.forward;

    //Current distance between p0 and p2
    private float currentDistance;

    private RaycastHit[] raycastPoints;

    void Start()
    {
        //Get which is the original length of the limb
        maxDistance = Vector3.Distance(p0.transform.position,  p2.transform.position);
        //Set the initial prevPos
        prevPos = p2.transform.position;
        //Set initial knee position
        points[1] = transform.InverseTransformPoint(Vector3.Lerp(p0.transform.position, p2.transform.position, kneeDistance));
    }

    void Update()
    {
        //Calculate the current distance
        currentDistance = Vector3.Distance(p0.transform.position, p2.transform.position);
        //Update the position of the points that define the curve and the middle (knee) point.
        UpdateCurvePoints();
        //Move the end point of the limb if necessary
        DecidePointToMove();
    }

    //Get point from a cuadratic bezier curve
    public Vector3 GetPoint(float t)
    {
        return transform.TransformPoint(Bezier.GetPoint(points[0], points[1], points[2], t));
        
    }

    //Method that is in charge that both the points from the curve and the actual game objects that represent them are in the right place.
    public void UpdateCurvePoints()
    {
        //make sure that the end part of the limb doesnt go further than the max distance. We clamp it to the max distance
        if ( currentDistance > maxDistance + 0.1f)
        {
            p2.transform.position = prevPos;
        }
        else
        {
            prevPos = p2.transform.position;
        }

        //The curve points are defined in local position, so the world position has to be converted to local coordinates
        //The curve points have to match with the actual position of the beggining and end points
        points[0] = transform.InverseTransformPoint(p0.transform.position);
        points[2] = transform.InverseTransformPoint(p2.transform.position);

        //The middle point has to match with the middle of the curve (always maintain the same distance from both points)
        //As its like a knee and its supposed to maintain more or less the same distance from the other parts of the limb, when the two end points get closer the knee goes up
        //We calculate that height of the knee in the following method. 
        //Basically we calculate the knee (p1) position.
        CalcMiddlePointHeight();

        //Change to the next instruction to use the curve and not the actual curve point (p1)
        p1.transform.position = GetPoint(kneeDistance);

       //Debug.Log(Vector3.Distance(points[0], transform.InverseTransformPoint(p1.transform.position)) + Vector3.Distance(points[2], transform.InverseTransformPoint(p1.transform.position)));
    }

    //Method to calculate the middle point position
    public void CalcMiddlePointHeight()
    {
        //if it's at max distance, the knee is just in the middle, fully extended
        if (currentDistance >= maxDistance)
        {
            points[1] = transform.InverseTransformPoint(Vector3.Lerp(p0.transform.position, p2.transform.position, kneeDistance));
        }
        else
        {
            //The height is equal to the difference between the maximum extension of the leg and the minimum
            float height = maxDistance - currentDistance;

            //Calc the vector direction in which the height will be applied
            CalcKneeDirection();

            //The position of the knee is in that height. Calculate middle position between p0 and p2 and hen add height in the forward vector.
            Vector3 finalPos = Vector3.Lerp(p0.transform.position, p2.transform.position, kneeDistance) + kneeDirection.normalized * height;

            //Debug.Log(Vector3.Distance(p0.transform.position, p2.transform.position) + Vector3.Distance(p0.transform.position, finalPos) + Vector3.Distance(finalPos, p2.transform.position));
            //Debug.Log("Pos vector = " + finalPos);
            points[1] = transform.InverseTransformPoint(finalPos);
        }
    }

    //Method to calculate the direction vector in which the height of the triangle will be applied 
    public Vector3 CalcKneeDirection()
    {
        //First calculate the angle in which the vector formed by the begining and end of the limb form and the original one
        //calculate current limb angle
        Vector3 currentAngle = (p2.transform.position - p0.transform.position).normalized;

        //cross product between the current direection of the leg and the right vector of p1. Gives a perp vector to the vector formed by the begining and end limb points (p0 and 02)
        kneeDirection = Vector3.Cross(currentAngle, p1.transform.right);

        return kneeDirection;

    }

    public void DecidePointToMove()
    {
        Debug.Log(Vector3.Distance(points[0], points[2]));
        raycastPoints = Physics.SphereCastAll(transform.TransformPoint(points[0]), maxDistance / 2, -p0.transform.up, maxDistance / 2, layerM, QueryTriggerInteraction.Ignore);
        
    }

    //Draw the debug gizmos for direction vectors
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(Vector3.forward) * 5;
        Gizmos.DrawRay(p1.transform.position, kneeDirection);
    }
}