using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowablePickup : MonoBehaviour
{
    // pickup variables 
    public GameObject pickup;
    public GameObject player;
    public GameObject playerCam;
    public float throwForce = 10f;
    public bool beingCarried = false;
    public GameObject currentlyHeldObject;

    private bool hasBeenThrown = false;

    // explosion variables
    public GameObject explosionPrefab;
    public float explosionForce = 50f;
    public float explosionRadius = 15f;
    public float explosionDamage = 15f;


    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().isKinematic = false;

    }

    // when the player is looking at the pickup and clicks the left mouse button, the pickup is picked up
    void OnMouseOver()
    {
        if (Vector3.Distance(pickup.transform.position, player.transform.position) <= 3f)
        {
            if (Input.GetMouseButtonUp(0))
            {
                GetComponent<Rigidbody>().isKinematic = true;
                transform.parent = playerCam.transform;
                beingCarried = true;
                player.GetComponent<FPSController>().CurrentlyHeldObject = gameObject; // Add this line

                // move pickup directly to firepoint
                transform.position = player.GetComponent<FPSController>().firePoint.transform.position;
                transform.rotation = player.GetComponent<FPSController>().firePoint.transform.rotation;

            }
        }
    }

    // when the player is carrying the pickup and presses E, the pickup is thrown
    void Update()
    {
        if (beingCarried)
        {
            if (Input.GetMouseButtonDown(0)) // Changed from Input.GetKeyDown(KeyCode.E)
            {
                GetComponent<Rigidbody>().isKinematic = false;
                transform.parent = null;
                beingCarried = false;
                player.GetComponent<FPSController>().CurrentlyHeldObject = null; // Add this line
                GetComponent<Rigidbody>().velocity = playerCam.transform.forward * player.GetComponent<FPSController>().bulletSpeed; // Use bulletSpeed for throw speed
                hasBeenThrown = true; // Set the flag to true when the object is thrown
            }
        }
    }

    //when the object collides with anything after it is picked up and shot, it explodes and has knockback on enemy objects
    void explode()
    {
        // Instantiate the explosion prefab
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // Get all the colliders within the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        // For each collider in the array
        foreach (Collider nearbyObject in colliders)
        {
            // Get the Rigidbody component from the collider we're currently looking at
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            // If the object we're looking at doesn't have a Rigidbody component, go on to the next collider
            if (rb == null) continue;

            // Apply explosion force to the object we're looking at
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }

        // Destroy the explosion after 1 seconds
        Destroy(explosion, 1f);

        // Destroy the grenade
        Destroy(gameObject);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (hasBeenThrown) // Only explode if the object has been thrown
        {
            // Explode on any impact
            explode();

            // Get the Health component from the object we collided with
            Health health = collision.gameObject.GetComponent<Health>();
            // If the object we collided with has a Health component, deal damage to it
            if (health != null)
            {
                health.ApplyDamage(explosionDamage);
            }
        }
    }

}
