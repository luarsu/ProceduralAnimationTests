using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCustom : MonoBehaviour {

    public Vector3 p0, p1, p2;

    public GameObject p0object, p1object, p3object;
	// Use this for initialization
	void Start () {
        p0 = p0object.transform.position;
        p1 = p1object.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        //p0object.transform.position = p0;
        //p1object.transform.position = p1;

    }
}
