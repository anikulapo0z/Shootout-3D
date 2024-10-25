using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlatformMovement : MonoBehaviour
{
    public bool canMove;
    [SerializeField] private float moveSpeed;
    [SerializeField] int startPoint;
    [SerializeField] Transform[] points;

    int i;

    void Start()
    {
        transform.position = points[startPoint].position;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, points[i].position) < 0.1f)
        {
            canMove = false;

            // If the platform reaches the second position, stop moving
            if (i == 1)
            {
                return;
            }

            // If the platform is at the initial point, move towards the second point
            else if (i == 0)
            {
                i++;
            }
        }
        // If the platform hasn't reached the second point, keep moving
        if (canMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, points[i].position, moveSpeed * Time.deltaTime);
        }
    }
}
