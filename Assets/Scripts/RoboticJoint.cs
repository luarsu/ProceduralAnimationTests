using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboticJoint : MonoBehaviour {

    //Axis in which the joint moves
    public Vector3 Axis;
    //The offset in which it starts
    public Vector3 StartOffset;

    void Awake()
    {
        StartOffset = transform.localPosition;
    }
}
