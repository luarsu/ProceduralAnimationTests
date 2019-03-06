using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralLeg : MonoBehaviour
{
    //Points that represent end, begining and middle point of the limb
    public Vector3[] points;
    public GameObject p0, p1, p2;
    public float movementSpeed;

    //bool that indicates if the leg is moving or stationary
    public bool isMoving;

    //Float that represents the percentage between the end and begining of the lemb where the middle point (knee) will be situated
    [Range(0.0f,1.0f)]
    public float kneeDistance = 0.5f;

    //Max length of the limb, or max distance to which the end of the limb can be of the beginning
    public float maxDistance;

    //max height of the feet when it performs the step movement
    public float stepHeight;

    //Layer mask for the raycast of get point
    public LayerMask layerM;

    //Previous position of the end limb (p2) to make sure that is clamped to the latest position where it didnt surpase the max distance 
    private Vector3 prevPos;

    //Vector direction in which the height of the knee (with respect to the vector between p0 and p2) will be moved
    private Vector3 kneeDirection = Vector3.forward;

    //Current distance between p0 and p2
    private float currentDistance;

    //float to indicate the lerp value for the leg movement
    private float lerp = 0; 

    //raycast to detect where to move
    private RaycastHit[] raycastPoints;

    private Vector3 targetPoint;

    private Vector3 lookTargetDirection;

    //bezier curve that defines the movement of the leg
    public BezierCurve movementCurve;

    void Start()
    {
        //Get which is the original length of the limb
        maxDistance = Vector3.Distance(p0.transform.position,  p2.transform.position);
        //Set the initial prevPos
        prevPos = p2.transform.position;
        //Set initial knee position
        points[1] = transform.InverseTransformPoint(Vector3.Lerp(p0.transform.position, p2.transform.position, kneeDistance));

        //Calculate the direction vector that the look for next point raycast will have (This direction will rotate based on the character direction)
        //lookTargetDirection = new Vector3(p0.transform.position.x, p0.transform.position.y - maxDistance, p0.transform.position.z + (maxDistance * 0.4f)) - p0.transform.position;
        lookTargetDirection = (new Vector3(p0.transform.position.x, p0.transform.position.y - maxDistance, p0.transform.position.z) + (p0.transform.forward * (maxDistance * 0.4f))) - p0.transform.position;

        UpdateMovementCurvePoints();
    }

    void Update()
    {
        
        //Calculate the current distance
        currentDistance = Vector3.Distance(p0.transform.position, p2.transform.position);
        //Update the position of the points that define the curve and the middle (knee) point.
        UpdateLegCurvePoints();

        if (isMoving)
        {
            MoveLegToTarget();
        }
        /*
        //Move the end point of the limb if necessary
        targetPoint = DecidePointToMove();

        //Assign the points for the bezier curve of the movement. Still have to work in this and write in in another function.
        Vector3 directionBezierMovement = targetPoint - p2.transform.position;
        Vector3 p1Height = Vector3.Cross(directionBezierMovement, p2.transform.right).normalized;
        directionBezierMovement += p1Height * maxDistance * kneeDistance;
        //AssignBezierPoints(p2.transform.position, directionBezierMovement, targetPoint);
        */
    }

    //Get point from a cuadratic bezier curve
    public Vector3 GetPoint(float t)
    {
        return transform.TransformPoint(Bezier.GetPoint(points[0], points[1], points[2], t));
        
    }

    //moves the end leg (feet) to the point indicated by the movement bezier curve
    void MoveLegToTarget()
    {
        p2.transform.position = movementCurve.GetPoint(lerp);


        if (lerp >= 1)
        {
            isMoving = false;
        }
        else
        {
            //update the lerp
            lerp += movementSpeed * Time.deltaTime;
        }
    }


    void UpdateMovementCurvePoints()
    {
        //indicate the leg that it should move/is moving
        isMoving = true;
        
        //Get the points for the movement bezier curve
        movementCurve.points[0] = p2.transform.position;
        movementCurve.points[2] = DecidePointToMove();

        //Make sure that a point to move is found
        if (movementCurve.points[2] == Vector3.zero)
        {
            Debug.Log("No point found");
        }
        if (Vector3.Distance(movementCurve.points[0], movementCurve.points[2]) > maxDistance)
        {
            Debug.Log("Point further than it should");
        }

        //Add a step max height?

        Vector3 midPointDirection = (p2.transform.position - p0.transform.position).normalized;
        midPointDirection = Vector3.Cross(midPointDirection, transform.right);

        //movementCurve.points[1] = movementCurve.GetPoint(0.5f); //+ (Vector3.up * stepHeight);
        //Get the midpoint between the start and the end
        movementCurve.points[1] = ((movementCurve.points[2] + movementCurve.points[0]) / 2) + (Vector3.up * stepHeight);
    }

    //Method that is in charge that both the points from the curve and the actual game objects that represent them are in the right place.
    public void UpdateLegCurvePoints()
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

    public Vector3 DecidePointToMove()
    {
        //Debug.Log(Vector3.Distance(points[0], points[2]));

        lookTargetDirection = (new Vector3(p0.transform.position.x, p0.transform.position.y - maxDistance, p0.transform.position.z) + (p0.transform.forward * (maxDistance * 0.4f))) - p0.transform.position;

        RaycastHit hit;
        if (Physics.Raycast(p0.transform.position, lookTargetDirection, out hit, maxDistance * 1.5f, layerM, QueryTriggerInteraction.Ignore))
        {
            return hit.point;
        }
        else
        {
            return Vector3.zero;
        }
        /*
         //Second option, doing it with a sphere raycast (maybe the radious just has to be the alcance of the step?) racyast the sphere till it hits the floor and then get points and angles there
        raycastPoints = Physics.SphereCastAll(p0.transform.position, maxDistance, -p0.transform.up, maxDistance / 2, layerM, QueryTriggerInteraction.Ignore);

        if (raycastPoints.Length > 0)
        {
            foreach (RaycastHit hit in raycastPoints)
            {
                Vector3 hitPoint = hit.point;
                Vector3 hitDirectionFromLeg = p0.transform.position;
                hitDirectionFromLeg.y = hitPoint.y;
                hitDirectionFromLeg = hitPoint - hitDirectionFromLeg;

                if (Vector3.Angle(p0.transform.forward, hitDirectionFromLeg) < 25)
                {
                    return hitPoint;
                }
            }
        }

    */
    }

    //Draw the debug gizmos for direction vectors
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 direction = transform.TransformDirection(Vector3.forward) * 5;
        Gizmos.DrawRay(p1.transform.position, kneeDirection);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(p0.transform.position, lookTargetDirection);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(DecidePointToMove(), (DecidePointToMove() + p0.transform.up));
    }

}