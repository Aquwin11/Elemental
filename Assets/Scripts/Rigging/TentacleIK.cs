using System.Collections.Generic;
using UnityEngine;

public class TentacleIK : MonoBehaviour
{
    public Transform[] tentacleSegments; // Assign all tentacle segments in order, from base to tip
    public Transform target; // The player or object the tentacle should follow
    public float speed = 5f; // How quickly the tentacle tip tries to reach the target
    public float drag = 10f; // How much the other tentacle segments 'drag' behind

    private void Update()
    {
        // Move the tip of the tentacle towards the target
        Vector3 directionToTarget = (target.position - tentacleSegments[tentacleSegments.Length - 1].position).normalized;
        tentacleSegments[tentacleSegments.Length - 1].position += directionToTarget * speed * Time.deltaTime;

        // Make each previous segment follow the one in front of it
        for (int i = tentacleSegments.Length - 2; i >= 0; i--)
        {
            Vector3 dir = (tentacleSegments[i + 1].position - tentacleSegments[i].position).normalized;
            float dist = Vector3.Distance(tentacleSegments[i + 1].position, tentacleSegments[i].position);
            tentacleSegments[i].position = tentacleSegments[i + 1].position - dir * dist;
            tentacleSegments[i].position = Vector3.Lerp(tentacleSegments[i].position, tentacleSegments[i + 1].position, drag * Time.deltaTime);
        }
    }
}

