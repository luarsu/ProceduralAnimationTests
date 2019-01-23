using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : MonoBehaviour {

    public Vector3 target;
    public Transform hivePosition;
    public float speed;
    public float orbitalSpeed = 10;
    public bool isFollowing = false;

    //movement behaviour variables
    public Vector3 position;
    public Vector3 velocityDirection;
    public Vector3 acceleration;


    //orbitate variables
    public Vector3 axis = Vector3.up;
    public Vector3 desiredPosition;
    public float radius;
    public float radiusSpeed;
    public float rotationSpeed;


    // Use this for initialization
    void Start () {
        hivePosition = FindObjectOfType<HiveMind>().transform;
        position = transform.position;
        //transform.position = (transform.position - hivePosition.position).normalized * radius + hivePosition.position;
        velocityDirection = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), Random.Range(-3, 3));
    }
	
	// Update is called once per frame
	void Update () {

        //Orbitate2(hivePosition.position);

        MoveTowardsObjective();


    }

    public void MoveTowardsObjective()
    {
        float step = speed * Time.deltaTime;
        if (isFollowing)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, step);

            //Debug.Log(Vector3.Distance(transform.position, target));
            if (Vector3.Distance(transform.position, target) < 0.5)
            {
                //Debug.Log("Hey");
                isFollowing = false;
            }
        }
        else
        {
            position = transform.position;
            //transform.position = Vector3.MoveTowards(transform.position, target + new Vector3(Random.Range(+1, -1), Random.Range(+1, -1), Random.Range(+1, -1)), step * 0.05f);
            Orbitate(target);
        }

    }


    public void Orbitate2(Vector3 targetPos)
    {
        axis = axis + new Vector3(Random.Range(+1, -1), Random.Range(+1, -1), Random.Range(+1, -1));
        transform.RotateAround(targetPos, axis, rotationSpeed * Time.deltaTime);
        desiredPosition = (transform.position - targetPos).normalized * radius + targetPos;
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * radiusSpeed);
    }

    public void Orbitate(Vector3 targetPos)
    {
        //
        Vector3 directionToCentre = targetPos - transform.position;
        velocityDirection = (directionToCentre + transform.forward).normalized; //+ new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1))).normalized;

        //rotate to look at the direction where it's moving to
        Vector3 positionToLookAt = transform.position + velocityDirection;
        transform.LookAt(positionToLookAt);


        position = position + velocityDirection * orbitalSpeed * Time.deltaTime;

        //Debug.DrawRay(position, position + positionToLookAt);

        transform.position = position;

    }

    //Update the target of the bee
    public void UpdateTarget(Vector3 newTarget)
    {
        isFollowing = true;
        target = newTarget;

    }
}
