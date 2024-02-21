using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiggingTest : MonoBehaviour
{
    public RobotJoint[] Joints;
    public Transform EndEffector;
    public Transform Destination;
    public float LearningRate;
    public float SamplingDistance;
    public float DistanceThreshold;
    public float DistanceWeight;
    public float RotationWeight;
    public float TorsionWeight;

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

    public float DistanceFromTarget(Vector3 target, float[] angles)
    {
        Vector3 point = ForwardKinematics(angles);
        return Vector3.Distance(point, target);
    }

    private void Update()
    {
        
    }
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
            // Clamp
            angles[i] = Mathf.Clamp(angles[i], Joints[i].MinAngle, Joints[i].MaxAngle);
            // Early termination
            if (DistanceFromTarget(target, angles) < DistanceThreshold)
                return;
        }
    }
    private void SolveIK()
    {
        
    }

    /*public float ErrorFunction(Vector3 target, float[] angles)
    {
        return
            NormalisedDistance(target, angles) * DistanceWeight +
            NormalisedRotation(target, angles) * RotationWeight +
            NormalisedTorsion(target, angles) * TorsionWeight;
    }

    private float NormalisedTorsion(Vector3 target, float[] angles)
    {
        float torsionPenalty = 0;
        for (int i = 0; i < solution.Length; i++)
            torsionPenalty += Mathf.Abs(solution[i]);
        torsionPenalty /= solution.Length;
        return torsionPenalty;
    }

    private float NormalisedRotation(Vector3 target, float[] angles)
    {
        float rotationPenalty =Mathf.Abs(
         Quaternion.Angle(EndEffector.rotation, Destination.rotation) / 180f);
        return rotationPenalty;
    }

    private float NormalisedDistance(Vector3 target, float[] angles)
    {
        Vector3 point = ForwardKinematics(angles);
        return Vector3.Distance(point, target);
    }*/
}
