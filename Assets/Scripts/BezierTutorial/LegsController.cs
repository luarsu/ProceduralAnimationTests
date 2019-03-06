using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegsController : MonoBehaviour {

    //array of the legs of the character
    public ProceduralLeg[] legs;

   public int numberOfLegsMoving;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        for (int i = 0; i < legs.Length; i++)
        {
            //if it's not moving already
            if (!legs[i].isMoving)
            {
                //i have to check also if the body has just started moving (you cant move the body first without moving at least one leg first)
                Debug.Log(legs[i].ShouldMoveLeg());
                //if the end point of the leg is getting too far from the body and at least half of the leg are static
                if (legs[i].ShouldMoveLeg() && (numberOfLegsMoving < legs.Length/2))
                {
                    legs[i].UpdateMovementCurvePoints();
                    numberOfLegsMoving++;
                }
                    
            }
            
        }
		
	}
}
