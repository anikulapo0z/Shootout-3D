using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class TriggerPlatform : MonoBehaviour
{
    PlatformMovement platformMovement;
    public NavMeshSurface sewerNavMesh;

    // Start is called before the first frame update
    void Start()
    {
        platformMovement = GetComponentInParent<PlatformMovement>();

        // deactivate navmesh component
        NavMeshSurface navMeshSurface = GetComponentInParent<NavMeshSurface>();
        navMeshSurface.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            platformMovement.canMove = true;
            NavMeshSurface navMeshSurface = GetComponentInParent<NavMeshSurface>();
            // activate navmesh component
            navMeshSurface.enabled = true;
            sewerNavMesh.enabled = false;

        }
    }
}
