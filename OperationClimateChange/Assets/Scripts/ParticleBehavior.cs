using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ParticleBehavior : MonoBehaviour
{
    private System.Random rnd = new System.Random();
    private int locationIndex = 0;
    private NavMeshAgent[] agents;

    public Transform route;
    public List<Transform> locations;
    private NavMeshHit closestHit;

    // Start is called before the first frame update
    void Start()
    {
        agents = GetComponents<NavMeshAgent>();

        InitializeRoute();
        MoveToNextLocation();
    }

    private void Update()
    {
        MoveToNextLocation();
    }

    void InitializeRoute()
    {
        foreach (Transform child in route)
        {
            locations.Add(child);
            NavMesh.SamplePosition(child.position, out closestHit, 500, 1);
        }
    }

    void MoveToNextLocation()
    {
        if (locations.Count == 0)
        { return; }

        foreach (NavMeshAgent agent in agents)
        {
            if (agent.isOnNavMesh
                && agent.remainingDistance < 0.2f
                && !agent.pathPending )
            {
                agent.destination =
                    locations[locationIndex].position;
                locationIndex = rnd.Next(0, locations.Count - 1);
            }
        }
    }
}
