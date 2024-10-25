using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRaycast : MonoBehaviour
{
    public GameObject muzzleFlashPrefab;
    public Transform firePoint;
    public float shootInterval = 2f;
    public float damage = 10f; // Damage inflicted by the enemy

    private Transform player;

    public Animator enemyAnimator; // Enemy Animator

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(ShootAtPlayer());
        enemyAnimator = GetComponentInChildren<Animator>();
    }

    /*
    IEnumerator ShootAtPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootInterval);


            // Instantiate the muzzle flash at the fire point
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);
            Destroy(muzzleFlash, 0.15f);

            // Raycast from the firePoint towards the player
            Ray ray = new Ray(firePoint.position, player.position - firePoint.position);
            RaycastHit hit;

            // Draw a line in the Scene view to visualize the aiming direction
            Debug.DrawRay(firePoint.position, ray.direction * 100f, Color.white);

            if (Physics.Raycast(ray, out hit, 150f)) // 100f is the maximum distance of the ray
            {
                var health = hit.collider.GetComponent<Health>();
                if (health != null)
                {
                    health.ApplyDamage(damage); // Apply damage to the object hit
                }
            }
            // Play the shooting animation
            enemyAnimator.SetBool("shooting", true);

            // Wait for a short time
            yield return new WaitForSeconds(0.1f);

            //check if the player is within the enemy's line of sight
            //play the shooting sound
            GetComponent<AudioSource>().Play();
        }
    }
    */

    IEnumerator ShootAtPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootInterval);

            // Instantiate the muzzle flash at the fire point
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);
            Destroy(muzzleFlash, 0.15f);

            // Raycast from the firePoint towards the player
            Ray ray = new Ray(firePoint.position, player.position - firePoint.position);
            RaycastHit hit;

            // Draw a line in the Scene view to visualize the aiming direction
            Debug.DrawRay(firePoint.position, ray.direction * 150f, Color.white); // Increased debug ray distance to match raycast distance

            // Check if player is within line of sight before shooting
            if (Physics.Raycast(ray, out hit, 150f)) // Adjusted maximum distance of ray to 150f
            {
                var health = hit.collider.GetComponent<Health>();
                if (health != null)
                {
                    health.ApplyDamage(damage); // Apply damage to the object hit

                    // Play the shooting animation
                    enemyAnimator.SetBool("shooting", true);

                    // Play the shooting sound
                    AudioSource audioSource = GetComponent<AudioSource>();
                    if (audioSource != null) // Check if AudioSource component exists
                    {
                        audioSource.Play();
                    }
                }
            }

            // Wait for a short time
            yield return new WaitForSeconds(0.1f);

            // Reset shooting animation
            enemyAnimator.SetBool("shooting", false);
        }
    }

    



}
