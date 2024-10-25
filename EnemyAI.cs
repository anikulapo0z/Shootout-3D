using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FMODUnity;

public class EnemyAI : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent navMeshAgent;

    //public
    public LayerMask whatIsGround, whatIsPlayer;
    public GameObject muzzleFlashPrefab;
    public Transform firePoint;
    public Animator enemyAnimator;
    public float inaccuracy = 0.4f;

    //patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //states
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    //reload
    public int maxBullets = 5;
    private int bulletCount;
    public float reloadTime = 3f;
    private bool isReloading = false;

    //main variables
    public float enemySpeed = 3.5f;
    public float damage;


    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Assuming the player object has the tag "Player"
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = enemySpeed;
        bulletCount = maxBullets;
    }
     void Update()
    {
        //check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }

    void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            navMeshAgent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    void SearchWalkPoint()
    {
        //calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    void ChasePlayer()
    {
        navMeshAgent.SetDestination(player.transform.position);

        // stop if the player gets too close
        if (navMeshAgent.remainingDistance < 3.5f)
        {
            navMeshAgent.isStopped = true;
        }
        else
        {
            navMeshAgent.isStopped = false;
        }
    }

    private void AttackPlayer()
    {
        ChasePlayer(); // Continue chasing the player even when in attack range
        transform.LookAt(player);

        //check if there are bullets left and not reloading
        if(bulletCount <= 0 && !isReloading)
        {
            isReloading = true;
            Invoke(nameof(Reload), reloadTime);
            return;
        }

        if(!alreadyAttacked)
        {
            // Instantiate the muzzle flash at the fire point
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);
            Destroy(muzzleFlash, 0.15f);

            // Raycast from the firePoint towards the player
            Ray ray = new Ray(firePoint.position, player.position - firePoint.position);
            
            // Add inaccuracy to the ray direction
            ray.direction += new Vector3(Random.Range(-inaccuracy, inaccuracy), Random.Range(-inaccuracy, inaccuracy), Random.Range(-inaccuracy, inaccuracy));

            RaycastHit hit;
            // play the shooting sound
            AudioManager.instance.PlayOneShot(FMODEvents.instance.playerShoot, this.transform.position);

            // Draw a line in the Scene view to visualize the aiming direction
            Debug.DrawRay(firePoint.position, ray.direction * 150f, Color.white); // Increased debug ray distance to match raycast distance

            // Check if player is within line of sight before shooting
            if (Physics.Raycast(ray, out hit, 500f)) // Adjusted maximum distance of ray to 150f
            {
                var health = hit.collider.GetComponent<Health>();
                if (health != null)
                {
                    health.ApplyDamage(damage); // Apply damage to the object hit
                    Debug.Log("Player Hit!");
                }
                else
                {
                    Debug.Log("Hit, but no Health component found on: " + hit.collider.gameObject.name);
                }
            }
            else
            {
                Debug.Log("Ray did not hit anything.");
            }

            // Play the shooting animation
            enemyAnimator.SetBool("shooting", true);

            alreadyAttacked = true;
            bulletCount--;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void Reload()
    {
        bulletCount = maxBullets;
        isReloading = false;
    }

    public void EnemyKilled()
    {
        Debug.Log("Dead");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
