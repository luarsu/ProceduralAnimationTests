using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboticMecha : MonoBehaviour {

    //Aray of joints of one leg of the mecha
    public RoboticJoint [] Joints;

    public float SamplingDistance;
    public float LearningRate;
    public float DistanceThreshold;

    public float speed;

    public Transform target;

    public float[] angles;

    public bool active;

    void Update()
    {
        if (active)
        {
            InverseKinematics(target.position, angles);
            UpdateAngles();
        }
    }

    private void Start()
    {
        //Debug.Log(Joints[2].Axis * 5);
    }

    //Calculates the point where the last join is based on the angle and position of the other ones
    public Vector3 ForwardKinematics(float[] angles)
    {
        Vector3 prevPoint = Joints[0].transform.position;
        Quaternion rotation = Quaternion.identity;
        for (int i = 1; i < Joints.Length; i++)
        {
            // Rotates around a new axis
            rotation *= Quaternion.AngleAxis(angles[i - 1], Joints[i - 1].Axis);
            Vector3 nextPoint = prevPoint + rotation * Joints[i].StartOffset;

            prevPoint = nextPoint;
        }
        return prevPoint;
    }

    //Function that estimates the partial gradient of the ith joints
    //Returns a single number that indicates how the distance from our target changes as a function of the joint rotation.
    public float PartialGradient(Vector3 target, float[] angles, int i)
    {
        // Saves the angle,
        // it will be restored later
        float angle = angles[i];

        // Gradient : [F(x+SamplingDistance) - F(x)] / h
        float f_x = DistanceFromTarget(target, angles);

        angles[i] += SamplingDistance;
        float f_x_plus_d = DistanceFromTarget(target, angles);

        float gradient = (f_x_plus_d - f_x) / SamplingDistance;

        // Restores
        angles[i] = angle;

        return gradient;
    }

    //Loops over all the joints, calculating its contribution to the gradient
    //Invoking InverseKinematics repeatedly move the robotic arm closer to the target point
    public void InverseKinematics(Vector3 target, float[] angles)
    {
        if (DistanceFromTarget(target, angles) < DistanceThreshold)
            return;

        for (int i = Joints.Length - 1; i >= 0; i--)
        {
            // Gradient descent
            // Update : Solution -= LearningRate * Gradient
            float gradient = PartialGradient(target, angles, i);
            angles[i] -= LearningRate * gradient;

            // Early termination
            if (DistanceFromTarget(target, angles) < DistanceThreshold)
                return;
        }
    }

    //Function to calculate the discance from the target to the end joint so we can calculate the gradient
    public float DistanceFromTarget(Vector3 target, float[] angles)
    {
        Vector3 point = ForwardKinematics(angles);
        return Vector3.Distance(point, target);
    }

    public void UpdateAngles()
    {
        /*
        for (int i = 0; i < angles.Length; i++)
        {
            //Joints[i].transform.Rotate(Joints[i].Axis * angles[i], Time.deltaTime * speed);
            Joints[i].transform.localEulerAngles = (Joints[i].Axis * angles[i]);
        }
        
        for (int i = angles.Length-1; i > 0; i--)
        {
            Debug.Log(Joints[i].Axis * angles[i]);
            Joints[i].transform.Rotate(Joints[i].Axis * angles[i], Time.deltaTime * speed);
            //Joints[i].transform.localEulerAngles = (Joints[i].Axis * angles[i]);
        }
        */

        for (int i = 0; i < angles.Length; i++)
        {
            /*
            Vector3 Euler0 = Joints[i].transform.localEulerAngles;
            Vector3 sum = Joints[i].Axis * angles[i];
            Joints[i].transform.localEulerAngles = Euler0 + sum;
            */

            Vector3 Euler0 = Joints[i].transform.localEulerAngles;
            if (Joints[i].Axis == new Vector3(1, 0, 0))
            {
                Euler0.x = angles[i];
            }
            else if (Joints[i].Axis == new Vector3(0, 1, 0))
            {
                Euler0.y = angles[i];
            }
            else
            {
                Euler0.z = angles[i];
            }

            Joints[i].transform.localEulerAngles = Euler0;
        }

        

    }

}
