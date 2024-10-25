using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 50f;
    public float shootInterval = 2f; // The time interval between each shot

    private Transform player;

    // Enemy Animator
    public Animator enemyAnimator;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Assuming the player object has the tag "Player"
        StartCoroutine(ShootAtPlayer());
        enemyAnimator = GetComponentInChildren<Animator>();
    }

    IEnumerator ShootAtPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootInterval);

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Vector3 direction = (player.position - firePoint.position).normalized;
            bullet.GetComponent<Rigidbody>().velocity = direction * bulletSpeed;

            // Destroy the bullet after 5 seconds if it didn't hit anything
            Destroy(bullet, 3f);
            // Play the shooting animation
            enemyAnimator.SetBool("shooting", true);
        }
    }
}
